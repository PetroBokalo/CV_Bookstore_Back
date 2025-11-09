using BookStore.Application.Common;
using BookStore.Application.DTOs.Account;

namespace BookStore.Application.Interfaces
{
    public interface IAccountService
    {
        public Task<ServiceResult<UserDataResponseDto>> GetUserDataAsync(string userId);


    }
}
