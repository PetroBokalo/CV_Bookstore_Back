using BookStoreAPI.Common;
using BookStoreAPI.DTOs;
using BookStoreAPI.Models;
using BookStoreAPI.Repositories.Interfaces;
using BookStoreAPI.Services.Interfaces;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace BookStoreAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepo;
        private readonly TokenService tokenService;

        public AuthService(IUserRepository userRepo, TokenService tokenService)
        {
            this.userRepo = userRepo;
            this.tokenService = tokenService;
        }


        public async Task<(ServiceResult<AuthUserResponceDto> Result, string? RefreshToken, DateTime? Expires)> LoginAsync(LoginUserDto loginUserDto)
        {
            try
            {
                var user = await userRepo.GetByEmailAsync(loginUserDto.Email);

                if (user == null)
                    return (ServiceResult<AuthUserResponceDto>.Fail("Invalid email or password", StatusCodes.Status401Unauthorized), null, null);


                using var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt);

                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginUserDto.Password));
                if (!computedHash.SequenceEqual(user.PasswordHash))
                    return (ServiceResult<AuthUserResponceDto>.Fail("Invalid email or password", StatusCodes.Status401Unauthorized), null, null);

                var newAccessToken = tokenService.GenerateAccessToken(user);
                var newRefreshToken = tokenService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                user.LastLogin = DateTime.UtcNow;
                await userRepo.SaveChangesAsync();

                var response = new AuthUserResponceDto(user.Id, user.Email, user.PhoneNumber, user.UserFirstName!, user.UserLastName!, newAccessToken);

                return (ServiceResult<AuthUserResponceDto>.Ok(response, "Login successful"), newRefreshToken, user.RefreshTokenExpiry);
            }
            catch (Exception ex)
            {
                return (ServiceResult<AuthUserResponceDto>.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError), null, null);
            }
            
        }

        public async Task<(ServiceResult<AuthUserResponceDto> Result, string? RefreshToken, DateTime? Expires)> RegisterAsync(RegisterUserDto registerUserDto)
        {
            try
            {
                if (await userRepo.ExistsByEmail(registerUserDto.Email))
                {
                    return (ServiceResult<AuthUserResponceDto>.Fail("User with this email already exists"),
                            null,
                            null);
                }

                using var hmac = new System.Security.Cryptography.HMACSHA512();

                var refreshToken = tokenService.GenerateRefreshToken();

                User newUser = new User()
                {
                    UserFirstName = registerUserDto.UserFirstName,
                    UserLastName = registerUserDto.UserLastName,
                    PhoneNumber = registerUserDto.PhoneNumber,
                    Email = registerUserDto.Email,
                    PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerUserDto.Password)),
                    PasswordSalt = hmac.Key,
                    LastLogin = DateTime.UtcNow,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
                };

                await userRepo.AddAsync(newUser);
                await userRepo.SaveChangesAsync();

                var token = tokenService.GenerateAccessToken(newUser);


                var userData = new AuthUserResponceDto(newUser.Id, newUser.Email, newUser.PhoneNumber, newUser.UserFirstName, newUser.UserLastName,
                    token);


                return (ServiceResult<AuthUserResponceDto>.Ok(userData, "User registred succesfully"),
                    newUser.RefreshToken,
                    newUser.RefreshTokenExpiry);
            }
            catch (Exception ex)
            {
                return (ServiceResult<AuthUserResponceDto>.Fail("Internal server error: " + ex.Message),
                    null,
                    null);
            }


            
        }


        public async Task<ServiceResult<string>> RefreshTokenAsync(string refreshtoken)
        {
            try
            {
                var user = await userRepo.GetByRefreshTokenAsync(refreshtoken);

                if (user == null)
                    return ServiceResult<string>.Fail("Invalid refresh token", StatusCodes.Status404NotFound);

                if (user.RefreshTokenExpiry <= DateTime.UtcNow)
                    return ServiceResult<string>.Fail("Refresh token is expired", StatusCodes.Status401Unauthorized);

                var newAccessToken = tokenService.GenerateAccessToken(user);

                return ServiceResult<string>.Ok(newAccessToken, "Access token is refreshed");
            }
            catch(Exception ex)
            {
                return ServiceResult<string>.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError);
            }
                           
        }



    }
}

