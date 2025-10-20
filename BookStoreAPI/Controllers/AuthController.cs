using BookStoreAPI.DTOs;
using BookStoreAPI.Services.Implementations;
using BookStoreAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {

            var result = await authService.RegisterAsync(registerUserDto);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });


            return Created("dummy", result.Data); // замість dummy треба вказати куди йти щоб отримати доступ до клієнта (якийсь endpoint) 

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            var (result, refreshToken, expiry) = await authService.LoginAsync(loginUserDto);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message, userId = result?.Data?.Id, userEmail = result?.Data?.Email });

            if (string.IsNullOrEmpty(refreshToken) || expiry == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to generate refresh token." });

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expiry
            });

            return Ok(result.Data);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new {message = "Missing refresh token" });

            var result = await authService.RefreshTokenAsync(refreshToken);
           
            if (result.Success)
                return Ok(new { accessToken = result.Data });

            return StatusCode(result.StatusCode, new { message = result.Message });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyDto verifyDto)
        {
            var(response, refreshToken, expiry) = await authService.VerifyAsync(verifyDto);

            if (!response.Success)
                return StatusCode(response.StatusCode, new { message = response.Message });

            if (string.IsNullOrEmpty(refreshToken) || expiry == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to generate refresh token." });

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expiry
            });

            return Ok(response);

        }

        [HttpPost("resend")]
        public async Task<IActionResult> Resend([FromBody] ResendDto resendDto)
        {
            var result = await authService.Resend(resendDto);

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

            return StatusCode(result.StatusCode, new { message = result.Message });
        }




    }
}
