using BlazorBoilerplate.Server.Managers;
using BlazorBoilerplate.Server.Middleware.Wrappers;
using BlazorBoilerplate.Shared.AuthorizationDefinitions;
using BlazorBoilerplate.Shared.Dto.Account;
using Logistique.Shared.AuthorizationDefinitions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace BlazorBoilerplate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        private readonly SmsManager _smsManager;
        private readonly IAccountManager _accountManager;

        public SmsController(SmsManager smsManager, IAccountManager accountManager)
        {
            _smsManager = smsManager;
            _accountManager = accountManager;
        }

        // POST: api/Sms/SendVerificationCode
        [HttpPost("SendVerificationCode")]
        [AllowAnonymous]
        public async Task<ApiResponse> SendVerificationCode(LoginDto parameters)
            => await _smsManager.SendVerificationCode(parameters.PhoneNumber);

        // POST: api/Sms/VerifyLoginCode
        [HttpPost("VerifyLoginCode")]
        [AllowAnonymous]
        public async Task<ApiResponse> VerifyLoginCode(LoginDto parameters)
        {
            ApiResponse apiResponse = await _smsManager.VerifyCode(parameters.PhoneNumber, parameters.VerificationCode);
            if (apiResponse.StatusCode == Status200OK)
            {
                return await _accountManager.LoginWithoutPassword(parameters);
            }
            else
            {
                return apiResponse;
            }
        }

        // POST: api/Sms/VerifyRegistrationCode
        [HttpPost("VerifyRegistrationCode")]
        [AllowAnonymous]
        public async Task<ApiResponse> VerifyRegistrationCode(RegisterDto parameters)
            => await _smsManager.VerifyCode(parameters.PhoneNumber, parameters.VerificationCode);

        // POST: api/Sms/SendInvitationCode
        [HttpPost("SendInvitationCode")]
        [Authorize]
        public async Task<ApiResponse> SendInvitationCode(LoginDto parameters)
            => await _smsManager.SendInvitationCode(parameters.PhoneNumber);

        // POST: api/Sms/VerifyInvitationCode
        [HttpPost("VerifyInvitationCode")]
        [Authorize]
        public async Task<ApiResponse> VerifyInvitationCode(LoginDto parameters)
            => await _smsManager.VerifyInvitationCode(parameters.PhoneNumber, parameters.VerificationCode);

        [HttpGet("Invitations")]
        public async Task<ApiResponse> Get() => await _smsManager.GetInvitations();

        [HttpDelete("CancelInvitation/{id}")]
        public async Task<ApiResponse> CancelInvitation(int id) => await _smsManager.DeleteInvitation(id);
    }
}