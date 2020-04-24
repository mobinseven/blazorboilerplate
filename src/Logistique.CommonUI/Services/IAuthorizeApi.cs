using System.Threading.Tasks;
using BlazorBoilerplate.Shared.Dto;
using BlazorBoilerplate.Shared.Dto.Account;

namespace Logistique.CommonUI.Services.Contracts
{
    public interface IAuthorizeApi
    {
        Task<bool> PhoneNumberExists(string PhoneNumber);

        Task<ApiResponseDto> SendVerificationCode(LoginDto loginParameters);

        Task<ApiResponseDto> VerifyLoginCode(LoginDto loginParameters);

        Task<ApiResponseDto> VerifyRegistrationCode(RegisterDto parameters);
    }
}