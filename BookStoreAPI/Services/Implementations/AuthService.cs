using BookStoreAPI.Common;
using BookStoreAPI.DTOs.Authentication;
using BookStoreAPI.Entities;
using BookStoreAPI.Repositories.Interfaces;
using BookStoreAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BookStoreAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly int verifyTokenAttemps = 5;
        private readonly DateTime verifyTokenExpiry = DateTime.UtcNow.AddMinutes(15);
        private readonly DateTime refreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        private readonly DateTime expired = DateTime.UtcNow.AddDays(-1);

        private readonly string verifyEndpointLink = "/auth/verify";
        private readonly string userProfileEndpointLink = "/account/me";
        private readonly string provide_phoneNumberEndpointLink = "/auth/provide-phoneNumber";
        private readonly string loginEndpointLink = "/auth/login";

        private readonly IVerifyTokenRepository verifyTokenRepo;
        private readonly IOptions<DataProtectionTokenProviderOptions> tokenOp;
        private readonly IConfiguration configuration;

        private readonly UserManager<AppUser> userManager;

        private readonly ITokenService tokenService;
        private readonly IEmailService emailService;

        public AuthService(IVerifyTokenRepository verifyTokenRepo, ITokenService tokenService, IEmailService emailService, 
            UserManager<AppUser> userManager, IOptions<DataProtectionTokenProviderOptions> tokenOp, IConfiguration configuration)
        {
            this.verifyTokenRepo = verifyTokenRepo;
            this.tokenService = tokenService;
            this.emailService = emailService;
            this.userManager = userManager;
            this.tokenOp = tokenOp;
            this.configuration = configuration;
        }


        public async Task<(ServiceResult<LoginUserResponseDto> Result, string? RefreshToken, DateTime? Expires)> LoginAsync(LoginUserDto loginUserDto)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(loginUserDto.Email);

                if (user == null)
                    return (ServiceResult<LoginUserResponseDto>.Fail("Invalid email or password", StatusCodes.Status401Unauthorized), null, null);



                if (!await userManager.CheckPasswordAsync(user, loginUserDto.Password))
                    return (ServiceResult<LoginUserResponseDto>.Fail("Invalid email or password", StatusCodes.Status401Unauthorized), null, null);

                if (!user.EmailConfirmed)
                {
                    var errorResponse = new LoginUserResponseDto(Email: user.Email!)
                    {
                        Links = new LoginLinksDto(Verification: verifyEndpointLink)
                    };
                    return (ServiceResult<LoginUserResponseDto>.Fail(errorResponse, "Email isn't verified", StatusCodes.Status403Forbidden), null, null);
                }

                var newAccessToken = tokenService.GenerateAccessToken(user);
                var newRefreshToken = tokenService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                user.LastLogin = DateTime.UtcNow;

                await userManager.UpdateAsync(user);

                var response = new LoginUserResponseDto(AccessToken: newAccessToken)
                {
                    Links = new LoginLinksDto(GetProfile: userProfileEndpointLink)
                };

                return (ServiceResult<LoginUserResponseDto>.Ok(response, "Login successful"), newRefreshToken, user.RefreshTokenExpiry);
            }
            catch (Exception ex)
            {
                return (ServiceResult<LoginUserResponseDto>.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError), null, null);
            }

        }

        public async Task<(ServiceResult<RegisterResponseDto> Result, string? RefreshToken, DateTime? Expires)> RegisterAsync(RegisterUserDto registerUserDto)
        {
            try
            {
                if (await userManager.FindByEmailAsync(registerUserDto.Email) != null)
                    return (ServiceResult<RegisterResponseDto>.Fail("User with this email already exists", StatusCodes.Status400BadRequest), null, null);

                var refreshToken = tokenService.GenerateRefreshToken();

                var newUser = new AppUser()
                {
                    UserFirstName = registerUserDto.UserFirstName,
                    UserLastName = registerUserDto.UserLastName,
                    PhoneNumber = registerUserDto.PhoneNumber,
                    Email = registerUserDto.Email,
                    UserName = registerUserDto.Email,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiry = refreshTokenExpiry,
                };

                var result = await userManager.CreateAsync(newUser, registerUserDto.Password);

                if (!result.Succeeded)
                {
                    var errormsg = string.Join("; ", result.Errors.Select(e => e.Description));
                    return (ServiceResult<RegisterResponseDto>.Fail(errormsg, StatusCodes.Status400BadRequest), null, null);
                }

                var accessToken = tokenService.GenerateAccessToken(newUser);

                // create Verify token

                var code = tokenService.GenerateVerifyToken();

                if (code == null)
                    return (ServiceResult<RegisterResponseDto>.Fail("Failed to generate verify token", StatusCodes.Status500InternalServerError), null, null);

                VerifyEmailToken verifyToken = new()
                {
                    AppUserId = newUser.Id,
                    IsUsed = false,
                    Attemps = verifyTokenAttemps,
                    Code = code,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = verifyTokenExpiry
                };



                await verifyTokenRepo.AddAsync(verifyToken);
                await verifyTokenRepo.SaveChangesAsync();

                // send code
                await emailService.SendVerificationCodeAsync(newUser.Email, code, verifyTokenExpiry);

                var response = new RegisterResponseDto(newUser.Email, accessToken)
                {
                    Links = new RegisterLinksDto(verifyEndpointLink)
                };

                return (ServiceResult<RegisterResponseDto>.Ok(response, "User is registreted", StatusCodes.Status202Accepted), newUser.RefreshToken, newUser.RefreshTokenExpiry); 
            }
            catch (Exception ex)
            {
                return (ServiceResult<RegisterResponseDto>.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError), null, null);
            }



        }

        public async Task<ServiceResult<string>> RefreshAccessTokenAsync(string refreshtoken)
        {
            try
            {
                var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshtoken);

                if (user == null)
                    return ServiceResult<string>.Fail("Invalid refresh token", StatusCodes.Status401Unauthorized);

                if (user.RefreshTokenExpiry <= DateTime.UtcNow)
                    return ServiceResult<string>.Fail("Refresh token is expired", StatusCodes.Status401Unauthorized);

                var newAccessToken = tokenService.GenerateAccessToken(user);

                return ServiceResult<string>.Ok(newAccessToken, "Access token is refreshed");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError);
            }

        }

        public async Task<ServiceResult> VerifyAsync(VerifyDto verifyDto, int userId)
        {
            try
            {
                var verifytoken = await verifyTokenRepo.GetByUserIdAsync(userId);

                if (verifytoken == null)
                    return ServiceResult.Fail("No user found", StatusCodes.Status404NotFound);

                if (verifytoken.Attemps <= 0 || verifytoken.ExpiresAt <= DateTime.UtcNow || verifytoken.IsUsed)
                    return ServiceResult.Fail("Verify token is expired", StatusCodes.Status400BadRequest);

                if (verifytoken.Code != verifyDto.Code)
                {
                    verifytoken.Attemps--;
                    await verifyTokenRepo.SaveChangesAsync();

                    return ServiceResult.Fail("Not valid code", StatusCodes.Status400BadRequest);
                }
                else
                {
                    verifytoken.IsUsed = true;
                    await verifyTokenRepo.SaveChangesAsync();


                    var user = await userManager.FindByIdAsync(userId.ToString());

                    if (user == null)
                        return ServiceResult.Fail("User not found", StatusCodes.Status404NotFound);


                    user.EmailConfirmed = true;
                    user.LastLogin = DateTime.UtcNow;

                    await userManager.UpdateAsync(user);


                    return ServiceResult.Ok("User is verified", StatusCodes.Status202Accepted);
                }
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError);
            }

            
        }

        public async Task<ServiceResult> ResendVerifyCodeAsync(int userId)
        {

            if (!await verifyTokenRepo.ExistsByIdAsync(userId))
                return ServiceResult.Fail("Unauthorized", StatusCodes.Status401Unauthorized);

            var newCode = tokenService.GenerateVerifyToken();

            if (newCode == null)
                return ServiceResult.Fail("Failed to generate verify token", StatusCodes.Status500InternalServerError);

            var token = await verifyTokenRepo.GetByUserIdAsync(userId);
            token!.ExpiresAt = verifyTokenExpiry;
            token.Attemps = verifyTokenAttemps;
            token.CreatedAt = DateTime.UtcNow;
            token.IsUsed = false;
            token.Code = newCode;

            await verifyTokenRepo.SaveChangesAsync();

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return ServiceResult.Fail("Unauthorized", StatusCodes.Status401Unauthorized);


            await emailService.SendVerificationCodeAsync(user.Email!, newCode, verifyTokenExpiry);

            return ServiceResult.Ok();

        }

        public async Task<ServiceResult<ForgotPasswordResponseDto>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(forgotPasswordDto.Email);

                if (user == null)
                    return ServiceResult<ForgotPasswordResponseDto>.Fail("Invalid data", StatusCodes.Status400BadRequest);

                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var token_lifespan = tokenOp.Value.TokenLifespan;

                var link = $"{forgotPasswordDto.UserURI}?token={Uri.EscapeDataString(token)}&email={Uri.UnescapeDataString(user.Email!)}";

                await emailService.SendResetPasswordLinkAsync(forgotPasswordDto.Email, link, DateTime.UtcNow.Add(token_lifespan));

                var response = new ForgotPasswordResponseDto(forgotPasswordDto.Email);

                return ServiceResult<ForgotPasswordResponseDto>.Ok(response, "Reset password link is sent");
            }
            catch (Exception ex)
            {
                return ServiceResult<ForgotPasswordResponseDto>.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError);
            }

        }

        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);

                if (user == null)
                    return ServiceResult.Fail("Invalid request data", StatusCodes.Status400BadRequest);

                var result = await userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return ServiceResult<ServiceResult>.Fail(errors, StatusCodes.Status400BadRequest);
                }

                return ServiceResult.Ok("Password is reset");

            }
            catch (Exception ex)
            {
                return ServiceResult.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError);
            }


        }

        public async Task<(ServiceResult<GoogleLoginUserResponseDto> Result, string? RefreshToken, DateTime? Expires)> GoogleLoginAsync(GoogleLoginUserDto googleLoginUserDto)
        {
            try
            {
                var result = await GoogleOAuthService.GetUserInfoAsync(
                googleLoginUserDto.Code,
                configuration["Authentication:Google:ClientId"]!,
                configuration["Authentication:Google:ClientSecret"]!,
                configuration["Authentication:Google:RedirectURL"]!
                );

                var payload = result.Data;

                if (payload == null)
                    return (ServiceResult<GoogleLoginUserResponseDto>.Fail(), null, null);

                var user = await userManager.FindByLoginAsync("Google", payload.Subject); // payload.Subject = Google UserId
                if (user == null)
                {
                    user = await userManager.FindByEmailAsync(payload.Email);

                    if (user == null)
                    {
                        user = new AppUser
                        {
                            UserName = payload.Email,
                            Email = payload.Email,
                            EmailConfirmed = true,
                            UserFirstName = payload.GivenName,
                            UserLastName = payload.FamilyName,
                            LastLogin = DateTime.UtcNow,
                            RefreshToken = tokenService.GenerateRefreshToken(),
                            RefreshTokenExpiry = refreshTokenExpiry,

                        };

                        await userManager.CreateAsync(user);
                    }

                    var info = new UserLoginInfo("Google", payload.Subject, "Google");
                    await userManager.AddLoginAsync(user, info);
                }
                else if (user.RefreshTokenExpiry <= DateTime.UtcNow)
                {
                    var newRefreshToken = tokenService.GenerateRefreshToken();
                    user.RefreshToken = newRefreshToken;
                    user.RefreshTokenExpiry = refreshTokenExpiry;

                    await userManager.UpdateAsync(user);
                }

                var link = user.PhoneNumber != null ? userProfileEndpointLink : provide_phoneNumberEndpointLink;

                var accessToken = tokenService.GenerateAccessToken(user);

                var response = new GoogleLoginUserResponseDto(accessToken, user.PhoneNumber != null, Link: link);

                return (ServiceResult<GoogleLoginUserResponseDto>.Ok(response, "User is found"), user.RefreshToken, user.RefreshTokenExpiry);
            }
            catch (Exception ex)
            {
                return (ServiceResult<GoogleLoginUserResponseDto>.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError), null, null);
            }

        }

        public async Task<ServiceResult> ProvidePhoneNumberAsync(ProvidePhoneNumberDto dto, string  userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);

                if (user == null)
                    return ServiceResult.Fail("User not found", StatusCodes.Status404NotFound);

                user.PhoneNumber = dto.PhoneNumber;
                var updateResult = await userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(",", updateResult.Errors.Select(e => e.Description));
                    return ServiceResult.Fail($"Failed to update phone number: {errors}", StatusCodes.Status400BadRequest);
                }

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError);
            }
            
        }

        public async Task<ServiceResult> LogoutAsync(string userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);

                if (user == null)
                    return ServiceResult.Fail("User not found", StatusCodes.Status404NotFound);

                user.RefreshTokenExpiry = expired;

                var updateResult = await userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(",", updateResult.Errors.Select(e => e.Description));
                    return ServiceResult.Fail($"Failed to expire refresh token: {errors}", StatusCodes.Status400BadRequest);
                }

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError);
            }
        }
    }
}

