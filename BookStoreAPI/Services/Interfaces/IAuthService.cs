
using BookStoreAPI.Common;
using BookStoreAPI.DTOs;

namespace BookStoreAPI.Services.Interfaces
{
    public interface IAuthService
    {

        Task<ServiceResult<RegisterUserResponceDto>> RegisterAsync(RegisterUserDto registerUserDto);

        Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginUserDto loginUserDto);

        Task<ServiceResult<TokenDto>> RefreshTokenAsync(TokenDto tokenDto);

    }
}
 