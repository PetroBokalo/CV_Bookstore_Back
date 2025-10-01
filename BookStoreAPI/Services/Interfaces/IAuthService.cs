
using BookStoreAPI.Common;
using BookStoreAPI.DTOs;

namespace BookStoreAPI.Services.Interfaces
{
    public interface IAuthService
    {

        Task<ServiceResult<LoginResponseDto>> RegisterAsync(RegisterUserDto registerUserDto);

        Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginUserDto loginUserDto);

    }
}
 