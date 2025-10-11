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
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {

            var (result, refreshtoken, expiry) = await authService.RegisterAsync(registerUserDto);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            if (string.IsNullOrEmpty(refreshtoken) || expiry == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to generate refresh token." });


            Response.Cookies.Append("refreshToken", refreshtoken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expiry
            });


            return Created("dummy", result.Data); // замість dummy треба вказати куди йти щоб отримати доступ до клієнта (якийсь endpoint) 

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginUserDto loginUserDto)
        {
            var (result, refreshToken, expiry) = await authService.LoginAsync(loginUserDto);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

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

    }
}
