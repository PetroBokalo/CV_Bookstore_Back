
using BookStoreAPI.Common;
using BookStoreAPI.DTOs.Authentication;

namespace BookStoreAPI.Services.Interfaces
{
    public interface IAuthService
    {

        Task<ServiceResult<RegisterResponseDto>> RegisterAsync(RegisterUserDto registerUserDto);

        Task<(ServiceResult<AuthUserResponseDto> Result, string? RefreshToken, DateTime? Expires)> LoginAsync(LoginUserDto loginUserDto);

        Task<(ServiceResult<AuthUserResponseDto> Result, string? RefreshToken, DateTime? Expires)> VerifyAsync(VerifyDto verifyDto);

        Task<ServiceResult> ResendVerifyCodeAsync(ResendVerifyCodeDto resendDto);

        Task<ServiceResult<string>> RefreshAccessTokenAsync(string refreshtoken);

        Task<ServiceResult<ForgotPasswordResponseDto>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);

        Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

        Task<(ServiceResult<GoogleLoginUserResponseDto> Result, string? RefreshToken, DateTime? Expires)> GoogleLoginAsync(GoogleLoginUserDto googleLoginUserDto);

        Task<ServiceResult> ProvidePhoneNumberAsync(ProvidePhoneNumberDto dto, string userId);

    }
}
 