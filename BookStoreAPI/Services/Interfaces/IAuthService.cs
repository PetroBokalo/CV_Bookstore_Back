
using BookStoreAPI.Common;
using BookStoreAPI.DTOs;

namespace BookStoreAPI.Services.Interfaces
{
    public interface IAuthService
    {

        Task<ServiceResult<RegisterResponseDto>> RegisterAsync(RegisterUserDto registerUserDto);

        Task<(ServiceResult<AuthUserResponseDto> Result, string? RefreshToken, DateTime? Expires)> LoginAsync(LoginUserDto loginUserDto);

        Task<(ServiceResult<AuthUserResponseDto> Result, string? RefreshToken, DateTime? Expires)> VerifyAsync(VerifyDto verifyDto);

        Task<ServiceResult<string>> Resend(ResendDto resendDto);

        Task<ServiceResult<string>> RefreshTokenAsync(string refreshtoken);

    }
}
 