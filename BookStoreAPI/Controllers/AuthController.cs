using BookStoreAPI.DTOs.Authentication;
using BookStoreAPI.Services.Implementations;
using BookStoreAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStoreAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        private readonly IConfiguration configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            this.authService = authService;
            this.configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {

            var(result, refreshToken, expiry) = await authService.RegisterAsync(registerUserDto);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            if (string.IsNullOrEmpty(refreshToken) || expiry == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to generate refresh token." });

            SetRefreshTokenCookie(refreshToken, expiry);

            return Accepted(result.Data);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            var (result, refreshToken, expiry) = await authService.LoginAsync(loginUserDto);

            if (!result.Success && result.StatusCode == StatusCodes.Status403Forbidden)
                return StatusCode(result.StatusCode, new { message = result.Message, data = result.Data});

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message});

            if (string.IsNullOrEmpty(refreshToken) || expiry == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to generate refresh token." });

            SetRefreshTokenCookie(refreshToken, expiry);

            return Ok(result.Data);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new {message = "Missing refresh token" });

            var result = await authService.RefreshAccessTokenAsync(refreshToken);
           
            if (result.Success)
                return Ok(new { accessToken = result.Data });

            return StatusCode(result.StatusCode, new { message = result.Message });
        }

        [HttpPost("verify")]
        [Authorize]
        public async Task<IActionResult> Verify([FromBody] VerifyDto verifyDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("User ID not found in token.");

            string userId = userIdClaim.Value;

            var result = await authService.VerifyAsync(verifyDto, Convert.ToInt32(userId));

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return Accepted();
        }

        [HttpPost("resend")]
        [Authorize]
        public async Task<IActionResult> Resend()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("User ID not found in token.");

            string userId = userIdClaim.Value;

            var result = await authService.ResendVerifyCodeAsync(Convert.ToInt32(userId));

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(StatusCodes.Status204NoContent); 

        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var result = await authService.ForgotPasswordAsync(forgotPasswordDto);

            if (!result.Success)
                return StatusCode(result.StatusCode, new {message = result.Message});

            return Ok(result.Data);

        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await authService.ResetPasswordAsync(resetPasswordDto);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return NoContent();
        }

        [HttpGet("google-login")]
        public async Task<IActionResult> GoogleLogin([FromQuery] GoogleLoginUserDto googleLoginUserDto)
        {
            var(result, refreshToken, expiry) = await authService.GoogleLoginAsync(googleLoginUserDto);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            
            if (string.IsNullOrEmpty(refreshToken) || expiry == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to generate refresh token." });

            if (string.IsNullOrEmpty(result?.Data?.accessToken))
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to generate access token." });

            SetRefreshTokenCookie(refreshToken, expiry);

            string? frontendPhoneInputURl;

            if (result.Data.IsPhoneNumberProvided)
                frontendPhoneInputURl = configuration["Authentication:Google:FrontCallbackURL"]; 
            else
                frontendPhoneInputURl = configuration["Authentication:Google:FrontProvidePhoneURL"];

            return Redirect($"{frontendPhoneInputURl}#accessToken={result.Data.accessToken}&link={result.Data.Link}");
        }

        [HttpGet("google")]
        public IActionResult GoogleStart()
        {
            var clientId = configuration["Authentication:Google:ClientId"];
            var redirectUri = configuration["Authentication:Google:RedirectURL"]; 
            var url = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=openid%20email%20profile";

            return Redirect(url);
        }

        [HttpPut("provide-phoneNumber")]
        [Authorize]
        public async Task<IActionResult> ProvidePhoneNumber([FromBody] ProvidePhoneNumberDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("User ID not found in token.");

            string userId = userIdClaim.Value;

            var result = await authService.ProvidePhoneNumberAsync(dto, userId);

            if (!result.Success) 
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("User ID not found in token.");

            string userId = userIdClaim.Value;

            var result = await authService.LogoutAsync(userId);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(StatusCodes.Status204NoContent);
        }

        private void SetRefreshTokenCookie(string refreshToken, DateTime? expiry)
        {
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expiry
            });
        }

    }
}
