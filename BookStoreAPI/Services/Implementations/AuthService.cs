using BookStoreAPI.Common;
using BookStoreAPI.DTOs;
using BookStoreAPI.Entities;
using BookStoreAPI.Repositories.Interfaces;
using BookStoreAPI.Services.Interfaces;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace BookStoreAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly int verifyTokenAttemps = 5;
        private readonly DateTime verifyTokenExpiry = DateTime.UtcNow.AddMinutes(15);
        private readonly DateTime resetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(30);

        private readonly IUserRepository userRepo;
        private readonly IVerifyTokenRepository verifyTokenRepo;
        private readonly IResetPasswordTokenRepository resetPasswordTokenRepo;
        private readonly TokenService tokenService;
        private readonly EmailService emailService;

        public AuthService(IUserRepository userRepo, IVerifyTokenRepository verifyTokenRepo, TokenService tokenService, EmailService emailService, IResetPasswordTokenRepository resetPasswordTokenRepo)
        {
            this.userRepo = userRepo;
            this.verifyTokenRepo = verifyTokenRepo;
            this.tokenService = tokenService;
            this.emailService = emailService;
            this.resetPasswordTokenRepo = resetPasswordTokenRepo;
        }


        public async Task<(ServiceResult<AuthUserResponseDto> Result, string? RefreshToken, DateTime? Expires)> LoginAsync(LoginUserDto loginUserDto)
        {
            try
            {
                var user = await userRepo.GetByEmailAsync(loginUserDto.Email);

                if (user == null)
                    return (ServiceResult<AuthUserResponseDto>.Fail("Invalid email or password", StatusCodes.Status401Unauthorized), null, null);


                using var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt);

                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginUserDto.Password));
                if (!computedHash.SequenceEqual(user.PasswordHash))
                    return (ServiceResult<AuthUserResponseDto>.Fail("Invalid email or password", StatusCodes.Status401Unauthorized), null, null);

                if (!user.IsEmailConfirmed)
                {
                    var errorResponse = new AuthUserResponseDto(user.Id, user.Email, string.Empty, string.Empty, string.Empty, string.Empty);
                    return (ServiceResult<AuthUserResponseDto>.Fail(errorResponse,"Email isn't verified", StatusCodes.Status403Forbidden), null, null);
                }
                    
                var newAccessToken = tokenService.GenerateAccessToken(user);
                var newRefreshToken = tokenService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                user.LastLogin = DateTime.UtcNow;
                await userRepo.SaveChangesAsync();

                var response = new AuthUserResponseDto(user.Id, user.Email, user.PhoneNumber, user.UserFirstName!, user.UserLastName!, newAccessToken);

                return (ServiceResult<AuthUserResponseDto>.Ok(response, "Login successful"), newRefreshToken, user.RefreshTokenExpiry);
            }
            catch (Exception ex)
            {
                return (ServiceResult<AuthUserResponseDto>.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError), null, null);
            }

        }

        public async Task<ServiceResult<RegisterResponseDto>> RegisterAsync(RegisterUserDto registerUserDto)
        {
            try
            {
                if (await userRepo.ExistsByEmail(registerUserDto.Email))
                {
                    return ServiceResult<RegisterResponseDto>.Fail("User with this email already exists", StatusCodes.Status400BadRequest);
                }

                using var hmac = new System.Security.Cryptography.HMACSHA512();

                User newUser = new User()
                {
                    UserFirstName = registerUserDto.UserFirstName,
                    UserLastName = registerUserDto.UserLastName,
                    PhoneNumber = registerUserDto.PhoneNumber,
                    Email = registerUserDto.Email,
                    PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerUserDto.Password)),
                    PasswordSalt = hmac.Key,
                };

                await userRepo.AddAsync(newUser);
                await userRepo.SaveChangesAsync();

                // create Verify token

                var code = tokenService.GenerateVerifyToken();

                if (code == null)
                    return ServiceResult<RegisterResponseDto>.Fail("Failed to generate verify token", StatusCodes.Status500InternalServerError);

                VerifyEmailToken verifyToken = new()
                {
                    //UserId = newUser.Id,
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

                var response = new RegisterResponseDto(newUser.Id, newUser.Email);

                return ServiceResult<RegisterResponseDto>.Ok(response, "User is registreted", StatusCodes.Status201Created); // code here just to see if it works
            }
            catch (Exception ex)
            {
                return ServiceResult<RegisterResponseDto>.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError);
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
            catch (Exception ex)
            {
                return ServiceResult<string>.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError);
            }

        }

        public async Task<(ServiceResult<AuthUserResponseDto> Result, string? RefreshToken, DateTime? Expires)> VerifyAsync(VerifyDto verifyDto)
        {
            if (verifyDto == null)
                return (ServiceResult<AuthUserResponseDto>.Fail("Bad data requested", StatusCodes.Status400BadRequest), null, null);

            var verifytoken = await verifyTokenRepo.GetByIdAsync(verifyDto.UserId);

            if (verifytoken == null)
                return (ServiceResult<AuthUserResponseDto>.Fail("No user found", StatusCodes.Status404NotFound), null, null);

            if (verifytoken.Attemps <= 0 || verifytoken.ExpiresAt <= DateTime.UtcNow || verifytoken.IsUsed)
                return (ServiceResult<AuthUserResponseDto>.Fail("Verify token is expired", StatusCodes.Status400BadRequest), null, null);

            if (verifytoken.Code != verifyDto.Code)
            {
                verifytoken.Attemps--;
                await verifyTokenRepo.SaveChangesAsync();

                return (ServiceResult<AuthUserResponseDto>.Fail("Not valid code", StatusCodes.Status403Forbidden), null, null);
            }
            else
            {
                verifytoken.IsUsed = true;
                await verifyTokenRepo.SaveChangesAsync();

                // generate tokens and fill response dto

                var user = await userRepo.GetByIdAsync(verifyDto.UserId);

                var accessToken = tokenService.GenerateAccessToken(user!);
                var refreshToken = tokenService.GenerateRefreshToken();
                var expiry = DateTime.UtcNow.AddDays(7);

                user!.EmailConfirmedAt = DateTime.UtcNow;
                user.LastLogin = DateTime.UtcNow;
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = expiry;
                await userRepo.SaveChangesAsync();

                

                AuthUserResponseDto response = new(user.Id, user.Email, user.PhoneNumber, user.UserFirstName!, user.UserLastName!, accessToken);

                return (ServiceResult<AuthUserResponseDto>.Ok(response, "User is verified"), refreshToken, expiry);
            }
                
        }

        public async Task<ServiceResult<string>> Resend(ResendDto resendDto)
        {
            if (resendDto.UserId < 0)
                return ServiceResult<string>.Fail("Invalid data", StatusCodes.Status400BadRequest);

            if (!await verifyTokenRepo.ExistsByIdAsync(resendDto.UserId))
                return ServiceResult<string>.Fail("Unauthorized", StatusCodes.Status401Unauthorized);

            var newCode = tokenService.GenerateVerifyToken();

            if (newCode == null)
                return ServiceResult<string>.Fail("Failed to generate verify token", StatusCodes.Status500InternalServerError);

            var token = await verifyTokenRepo.GetByIdAsync(resendDto.UserId);
            token!.ExpiresAt = verifyTokenExpiry;
            token.Attemps = verifyTokenAttemps;
            token.CreatedAt = DateTime.UtcNow;
            token.IsUsed = false;
            token.Code = newCode;

            await verifyTokenRepo.SaveChangesAsync();

            var user = await userRepo.GetByIdAsync(resendDto.UserId);
            if (user == null)
                return ServiceResult<string>.Fail("Unauthorized", StatusCodes.Status401Unauthorized);


            await emailService.SendVerificationCodeAsync(user.Email, newCode, verifyTokenExpiry);

            return ServiceResult<string>.Ok(string.Empty); 

        }

        public async Task<ServiceResult<ForgotPasswordResponseDto>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var user = await userRepo.GetByEmailAsync(forgotPasswordDto.Email);

                if (user == null)
                    return ServiceResult<ForgotPasswordResponseDto>.Fail("Invalid data", StatusCodes.Status400BadRequest);

                // TODO: generate 

                var token = "new token";

                var resetPasswordToken = new ResetPasswordToken
                {
                    Token = token,
                    ExpiresAt = resetPasswordTokenExpiry,
                    IsUsed = false,
                    //UserId = user.Id
                };



                // TODO: generate link 

                var link = "Here will be your link";
                var link_expiry = DateTime.UtcNow.AddMinutes(30);

                await emailService.SendResetPasswordLinkAsync(forgotPasswordDto.Email, link, link_expiry.ToLocalTime());

                var response = new ForgotPasswordResponseDto(forgotPasswordDto.Email);

                return ServiceResult<ForgotPasswordResponseDto>.Ok(response, "Reset password link is sent");
            }
            catch (Exception ex)
            {
                return ServiceResult<ForgotPasswordResponseDto>.Fail("Internal server error: " + ex.Message, StatusCodes.Status500InternalServerError);
            }

        }
    }
}

