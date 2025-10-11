
using BookStoreAPI.Common;
using BookStoreAPI.DTOs;

namespace BookStoreAPI.Services.Interfaces
{
    public interface IAuthService
    {

        Task<(ServiceResult<AuthUserResponceDto> Result, string? RefreshToken, DateTime? Expires)> RegisterAsync(RegisterUserDto registerUserDto);

        Task<(ServiceResult<AuthUserResponceDto> Result, string? RefreshToken, DateTime? Expires)> LoginAsync(LoginUserDto loginUserDto);

        Task<ServiceResult<string>> RefreshTokenAsync(string refreshtoken);

    }
}
 