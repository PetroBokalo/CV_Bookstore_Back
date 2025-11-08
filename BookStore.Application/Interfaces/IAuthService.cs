
using BookStore.Application.Common;
using BookStore.Application.DTOs.Authentication;

namespace BookStore.Application.Interfaces
{
    public interface IAuthService
    {

        Task<(ServiceResult<RegisterResponseDto> Result, string? RefreshToken, DateTime? Expires)> RegisterAsync(RegisterUserDto registerUserDto);

        Task<(ServiceResult<LoginUserResponseDto> Result, string? RefreshToken, DateTime? Expires)> LoginAsync(LoginUserDto loginUserDto);

        Task<ServiceResult> VerifyAsync(VerifyDto verifyDto, int userId);

        Task<ServiceResult> ResendVerifyCodeAsync(int userId);

        Task<ServiceResult<string>> RefreshAccessTokenAsync(string refreshtoken);

        Task<ServiceResult<ForgotPasswordResponseDto>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);

        Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

        Task<(ServiceResult<GoogleLoginUserResponseDto> Result, string? RefreshToken, DateTime? Expires)> GoogleLoginAsync(GoogleLoginUserDto googleLoginUserDto);

        Task<ServiceResult> ProvidePhoneNumberAsync(ProvidePhoneNumberDto dto, string userId);

        Task<ServiceResult> LogoutAsync(string userId);

    }
}
 