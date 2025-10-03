
using BookStoreAPI.Common;
using BookStoreAPI.DTOs;

namespace BookStoreAPI.Services.Interfaces
{
    public interface IAuthService
    {

        Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterUserDto registerUserDto);

        Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginUserDto loginUserDto);

    }
}
 