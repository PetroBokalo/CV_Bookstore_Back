using BookStoreAPI.Common;
using BookStoreAPI.DTOs.Account;

namespace BookStoreAPI.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<ServiceResult<UserDataResponseDto>> GetUserDataAsync(string userId);


    }
}
