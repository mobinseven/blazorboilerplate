using Logistique.CommonUI.Services.Contracts;
using BlazorBoilerplate.Shared;
using BlazorBoilerplate.Shared.Dto;
using BlazorBoilerplate.Shared.Dto.Account;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Logistique.CommonUI.Services.Implementations
{
    public class AuthorizeApi : IAuthorizeApi
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;

        public AuthorizeApi(NavigationManager navigationManager, HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _navigationManager = navigationManager;
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> PhoneNumberExists(string PhoneNumber)
        {
            var apiResponse = await _httpClient.GetJsonAsync<ApiResponseDto>($"api/Account/PhoneNumberExists/{PhoneNumber}");
            if (apiResponse.StatusCode == Status200OK)
            {
                BoolDto exists = Newtonsoft.Json.JsonConvert.DeserializeObject<BoolDto>(apiResponse.Result.ToString());
                return exists.Boolean;
            }
            return false;
        }

        public async Task<ApiResponseDto> SendVerificationCode(LoginDto loginParameters)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto>($"api/Sms/SendVerificationCode", loginParameters);
        }

        public async Task<ApiResponseDto> VerifyRegistrationCode(RegisterDto parameters)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto>($"api/Sms/VerifyRegistrationCode", parameters);
        }

        public async Task<ApiResponseDto> VerifyLoginCode(LoginDto loginParameters)
        {
            ApiResponseDto resp;

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "api/Sms/VerifyLoginCode");
            httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(loginParameters));
            httpRequestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var response = await _httpClient.SendAsync(httpRequestMessage))
            {
                response.EnsureSuccessStatusCode();

#if ServerSideBlazor

                if (response.Headers.TryGetValues("Set-Cookie", out var cookieEntries))
                {
                    var uri = response.RequestMessage.RequestUri;
                    var cookieContainer = new CookieContainer();

                    foreach (var cookieEntry in cookieEntries)
                    {
                        cookieContainer.SetCookies(uri, cookieEntry);
                    }

                    var cookies = cookieContainer.GetCookies(uri).Cast<Cookie>();

                    foreach (var cookie in cookies)
                    {
                        await _jsRuntime.InvokeVoidAsync("jsInterops.setCookie", cookie.ToString());
                    }
                }
#endif

                var content = await response.Content.ReadAsStringAsync();
                resp = JsonConvert.DeserializeObject<ApiResponseDto>(content);
            }

            return resp;
        }
    }
}