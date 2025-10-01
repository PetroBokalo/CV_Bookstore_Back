using BookStoreAPI.Common;
using BookStoreAPI.DTOs;
using BookStoreAPI.Models;
using BookStoreAPI.Repositories.Interfaces;
using BookStoreAPI.Services.Interfaces;

namespace BookStoreAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepo;

        public AuthService(IUserRepository userRepo)
        {
            this.userRepo = userRepo;
        }


        public async Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginUserDto loginUserDto)
        {
            var user = await userRepo.GetByEmailAsync(loginUserDto.Email);

            if (user == null)           
                return ServiceResult<LoginResponseDto>.Fail("Invalid email or password");
            

            using var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginUserDto.Password));
            if (!computedHash.SequenceEqual(user.PasswordHash))
                return ServiceResult<LoginResponseDto>.Fail("Invalid email or password");


            user.LastLogin = DateTime.UtcNow;
            await userRepo.SaveChangesAsync();

            var response = new LoginResponseDto(user.UserFirstName ?? "", user.CreatedAt);

            return ServiceResult<LoginResponseDto>.Ok(response, "Login successful");
        }

        public async Task<ServiceResult<LoginResponseDto>> RegisterAsync(RegisterUserDto registerUserDto)
        {

            if (await userRepo.ExistsByEmail(registerUserDto.Email))
            {
                return ServiceResult<LoginResponseDto>.Fail("User with this email already exists");
            }

            using var hmac = new System.Security.Cryptography.HMACSHA512();

            User newUser = new User()
            {
                UserFirstName = registerUserDto.UserFirstName,
                UserLastName = registerUserDto.UserLastName,
                PhoneNumber = registerUserDto.PhoneNumber,
                Email = registerUserDto.Email,
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerUserDto.Password)),
                PasswordSalt = hmac.Key
            };

            await userRepo.AddAsync(newUser);
            await userRepo.SaveChangesAsync();

            var responce = new LoginResponseDto(newUser.UserFirstName ?? "", newUser.CreatedAt);

            return ServiceResult<LoginResponseDto>.Ok(responce, "User registred succesfully");
                
        }
    }
}

