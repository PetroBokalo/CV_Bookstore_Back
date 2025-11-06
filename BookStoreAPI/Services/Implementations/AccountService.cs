using BookStoreAPI.Common;
using BookStoreAPI.DTOs.Account;
using BookStoreAPI.Entities;
using BookStoreAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BookStoreAPI.Services.Implementations
{
    public class AccountService : IAccountService
    {

        private readonly UserManager<AppUser> userManager;

        public AccountService(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }


        public async Task<ServiceResult<UserDataResponseDto>> GetUserDataAsync(string userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);

                if (user == null)
                    return ServiceResult<UserDataResponseDto>.Fail("User not found", StatusCodes.Status404NotFound);

                var response = new UserDataResponseDto(user.Id, user.Email ?? "", user.PhoneNumber ?? "", user.UserFirstName, user.UserLastName);

                return ServiceResult<UserDataResponseDto>.Ok(response, "User data sent", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserDataResponseDto>.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError);
            }
            
        }


    }
}
