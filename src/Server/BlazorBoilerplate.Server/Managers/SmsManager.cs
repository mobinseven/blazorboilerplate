using BlazorBoilerplate.Server.Middleware.Wrappers;
using BlazorBoilerplate.Shared;
using BlazorBoilerplate.Shared.AuthorizationDefinitions;
using BlazorBoilerplate.Shared.DataModels;
using BlazorBoilerplate.Shared.Dto.Account;
using BlazorBoilerplate.Storage;
using Finbuckle.MultiTenant;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace BlazorBoilerplate.Server.Managers
{
    public static class SmsCodeType
    {
        public const string Verification = "VerificationCode";
        public const string Invitation = "InvitationCode";
    }

    public class SmsManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<AccountManager> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ITenantManager _tenantManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SmsManager(UserManager<ApplicationUser> userManager,
            IMemoryCache memoryCache,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountManager> logger,
            ApplicationDbContext applicationDbContext,
            ITenantManager tenantManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _memoryCache = memoryCache;
            _context = applicationDbContext;
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("apikeys.json", optional: false)
                .Build();
            _tenantManager = tenantManager;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<ApiResponse> SendVerificationCode(string PhoneNumber)
        {
            Random r = new Random();
            int rInt = r.Next(100000, 999999);
            string Code = rInt.ToString();
            var client = new RestClient("https://api.ghasedak.io/v2/verification/send/simple");
            var request = new RestRequest(Method.POST);
            request.AddHeader("apikey", _configuration["ApiKeys:Ghasedak"]);
            request.AddParameter("receptor", PhoneNumber);
            request.AddParameter("type", 1);
            request.AddParameter("template", "Test");
            request.AddParameter("param1", Code); ;
            IRestResponse response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                string CacheKey = $"{SmsCodeType.Verification}:{PhoneNumber}";
                _memoryCache.Set(CacheKey, Code, DateTime.Now.AddMinutes(5)); // TODO expire cache
                return new ApiResponse(Status200OK, $"SendVerificationCode to {PhoneNumber}: Success");
            }
            return new ApiResponse(Status500InternalServerError, $"SendVerificationCode to {PhoneNumber}: Error");
        }

        public async Task<ApiResponse> VerifyCode(string PhoneNumber, string ReceivedCode)
        {
            string CacheKey = $"{SmsCodeType.Verification}:{PhoneNumber}";
            if (_memoryCache.TryGetValue(CacheKey, out string CacheValue))
            {
                if (!string.IsNullOrEmpty(CacheValue) && CacheValue == ReceivedCode)
                {
                    _memoryCache.Remove(CacheKey);
                    return new ApiResponse(Status200OK, $"VerifyCode of {PhoneNumber}: Success");
                }
                else
                {
                    return new ApiResponse(Status409Conflict, $"VerifyCode of {PhoneNumber}: Wrong Code");
                }
            }
            else
            {
                return new ApiResponse(Status409Conflict, $"VerifyCode of {PhoneNumber}: Expired");
            }
        }

        public async Task<ApiResponse> SendInvitationCode(string PhoneNumber)
        {
            Random r = new Random();
            int rInt = r.Next(100000, 999999);
            string Code = rInt.ToString();
            var client = new RestClient("https://api.ghasedak.io/v2/verification/send/simple");
            var request = new RestRequest(Method.POST);
            request.AddHeader("apikey", _configuration["ApiKeys:Ghasedak"]);
            request.AddParameter("receptor", PhoneNumber);
            request.AddParameter("type", 1);
            request.AddParameter("template", "Test");
            request.AddParameter("param1", Code); ;
            IRestResponse response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                string CacheKey = $"{SmsCodeType.Invitation}:{PhoneNumber}";
                _memoryCache.Set(CacheKey, $"{Code}:{_httpContextAccessor.HttpContext.GetMultiTenantContext().TenantInfo.Identifier}"); // TODO expire cache , DateTime.Now.AddMinutes(5)
                SmsInvitation invitaion = _context.SmsInvitations.SingleOrDefault(i => i.PhoneNumber == PhoneNumber);
                if (invitaion == null)
                {
                    _context.SmsInvitations.Add(new SmsInvitation { PhoneNumber = PhoneNumber, VerificationCode = Code });
                }
                else
                {
                    invitaion.SentAt = DateTime.Now;
                    invitaion.VerificationCode = Code;
                    _context.Entry(invitaion).State = EntityState.Modified;
                }
                _context.SaveChanges();
                return new ApiResponse(Status200OK, $"SendInvitationCode to {PhoneNumber}: Success", _context.SmsInvitations.SingleOrDefault(i => i.PhoneNumber == PhoneNumber));
            }
            return new ApiResponse(Status500InternalServerError, $"SendInvitationCode to {PhoneNumber}: Error");
        }

        public async Task<ApiResponse> VerifyInvitationCode(string PhoneNumber, string ReceivedCode)
        {
            SmsInvitation invitation = await _context.SmsInvitations.IgnoreQueryFilters().SingleOrDefaultAsync(i => i.PhoneNumber == PhoneNumber);
            if (invitation != null)
            {
                if (ReceivedCode == invitation.VerificationCode)
                {
                    string tenantId = await _context.SmsInvitations.IgnoreQueryFilters().Where(i => i == invitation).Select(bo => EF.Property<string>(bo, "TenantId")).SingleAsync();
                    ApiResponse apiResponse = await _tenantManager.AddToTenant(_httpContextAccessor.HttpContext.User.Identity.Name, tenantId);
                    if (apiResponse.StatusCode == Status200OK)
                    {
                        _context.SmsInvitations.Remove(_context.SmsInvitations.IgnoreQueryFilters().Single(i => i.PhoneNumber == PhoneNumber));
                        _context.SaveChanges();
                        return new ApiResponse(Status200OK, $"VerifyInvitationCode of {PhoneNumber}: Success");
                    }
                    return new ApiResponse(Status500InternalServerError, $"VerifyInvitationCode of {PhoneNumber}: {apiResponse.Message}");
                }
                return new ApiResponse(Status409Conflict, $"VerifyInvitationCode of {PhoneNumber}: Wrong Code");
            }
            return new ApiResponse(Status404NotFound, $"VerifyInvitationCode of {PhoneNumber}: No such invitation"); ;
        }

        public async Task<ApiResponse> GetInvitations() => new ApiResponse(Status200OK, "GetInvitations : Success", await _context.SmsInvitations.ToListAsync());

        public async Task<ApiResponse> DeleteInvitation(int id)
        {
            SmsInvitation invitation = await _context.SmsInvitations.FindAsync(id);
            if (invitation != null)
            {
                _context.SmsInvitations.Remove(invitation);
                _context.SaveChanges();
                return new ApiResponse(Status200OK, "CancelInvitation: Success");
            }
            return new ApiResponse(Status404NotFound, $"DeleteInvitation : No such invitation"); ;
        }

        public async Task<ApiResponse> LoginWithoutPassword(LoginDto parameters)
        {
            try
            {
                ApplicationUser user = await _context.Users.SingleAsync(u => u.UserName == parameters.UserName);

                // If lock out activated and the max. amounts of attempts is reached.
                if (!await _signInManager.CanSignInAsync(user))
                {
                    _logger.LogInformation("User not allowed to log in: {0}", parameters.UserName);
                    return new ApiResponse(Status401Unauthorized, "Login not allowed!");
                }

                await _signInManager.SignInAsync(user, parameters.RememberMe);

                _logger.LogInformation("Logged In: {0}", parameters.UserName);
                return new ApiResponse(Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Login Failed: " + ex.Message);
            }

            _logger.LogInformation("Invalid Password for user {0}}", parameters.UserName);
            return new ApiResponse(Status401Unauthorized, "Login Failed");
        }

        public async Task<ApplicationUser> RegisterNewUserAsync(RegisterDto registerDto)
        {
            var user = new ApplicationUser
            {
                UserName = registerDto.UserName,
                PhoneNumber = registerDto.PhoneNumber,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                FullName = registerDto.FirstName + " " + registerDto.LastName,
                PhoneNumberConfirmed = true
            };

            var createUserResult = registerDto.Password == null ?
                await _userManager.CreateAsync(user) :
                await _userManager.CreateAsync(user, registerDto.Password);

            if (!createUserResult.Succeeded)
                throw new DomainException(string.Join(",", createUserResult.Errors.Select(i => i.Description)));

            await _userManager.AddClaimsAsync(user, new Claim[]{
                    new Claim(Policies.IsUser, string.Empty),
                    new Claim(JwtClaimTypes.Name, user.UserName),
                    new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                    new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString(), ClaimValueTypes.Boolean),
                });

            //Role - Here we tie the new user to the "User" role
            await _userManager.AddToRoleAsync(user, DefaultRoleNames.User);

            _logger.LogInformation("New user registered: {0}", user);

            return user;
        }
    }
}