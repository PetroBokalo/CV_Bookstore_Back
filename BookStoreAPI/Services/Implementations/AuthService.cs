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


        public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginUserDto loginUserDto)
        {
            var user = await userRepo.GetByEmailAsync(loginUserDto.Email);

            if (user == null)
                return ServiceResult<AuthResponseDto>.Fail("Invalid email or password");


            using var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginUserDto.Password));
            if (!computedHash.SequenceEqual(user.PasswordHash))
                return ServiceResult<AuthResponseDto>.Fail("Invalid email or password");


            user.LastLogin = DateTime.UtcNow;
            await userRepo.SaveChangesAsync();

            var response = new AuthResponseDto(user.UserFirstName ?? "", user.CreatedAt);

            return ServiceResult<AuthResponseDto>.Ok(response, "Login successful");
        }

        public async Task<(ServiceResult<RegisterUserResponceDto> Result, string? RefreshToken, DateTime? Expires)> RegisterAsync(RegisterUserDto registerUserDto)
        {
            try
            {
                if (await userRepo.ExistsByEmail(registerUserDto.Email))
                {
                    return (ServiceResult<RegisterUserResponceDto>.Fail("User with this email already exists"),
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


                var userData = new RegisterUserResponceDto(newUser.Id, newUser.Email, newUser.PhoneNumber, newUser.UserFirstName, newUser.UserLastName,
                    token);


                return (ServiceResult<RegisterUserResponceDto>.Ok(userData, "User registred succesfully"),
                    newUser.RefreshToken,
                    newUser.RefreshTokenExpiry);
            }
            catch (Exception ex)
            {
                return (ServiceResult<RegisterUserResponceDto>.Fail("Internal server error: " + ex.Message),
                    null,
                    null);
            }


            
        }


        public async Task<ServiceResult<TokenDto>> RefreshTokenAsync(TokenDto tokenDto)
        {
            try
            {
                if (tokenDto == null)
                    return ServiceResult<TokenDto>.Fail("Invalid client request");

                var result_principal = tokenService.GetPrincipalFromExpiredToken(tokenDto.AccessToken);
                if (!result_principal.Success)
                    return ServiceResult<TokenDto>.Fail(result_principal.Message);

                var principal = result_principal.Data;

                var userEmail = principal?.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
                if (string.IsNullOrEmpty(userEmail))
                    return ServiceResult<TokenDto>.Fail("Unauthorized", 401);

                var user = await userRepo.GetByEmailAsync(userEmail);
                if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
                    return ServiceResult<TokenDto>.Fail("Unauthorized", 401);

                var newAccessToken = tokenService.GenerateAccessToken(user);
                var newRefreshToken = tokenService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

                await userRepo.SaveChangesAsync();

                var response = new TokenDto(newAccessToken, newRefreshToken);

                return ServiceResult<TokenDto>.Ok(response, "Refreshed");
            }
            catch(Exception ex)
            {
                return ServiceResult<TokenDto>.Fail("Internal server error: " + ex.Message);
            }
                           
        }



    }
}

