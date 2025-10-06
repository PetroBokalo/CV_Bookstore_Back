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

            var (result, refreshtoken, expires) = await authService.RegisterAsync(registerUserDto);

            if (!result.Success)
                return BadRequest(result.Message);

            if (string.IsNullOrEmpty(refreshtoken) || expires == null)
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate refresh token.");


            Response.Cookies.Append("refreshToken", refreshtoken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expires
            });


            return Created("dummy", result); // замість dummy треба вказати куди йти щоб отримати доступ до клієнта (якийсь endpoint) 

        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenDto tokenDto)
        {
            var result = await authService.RefreshTokenAsync(tokenDto);

            if (result.Success)
                return Ok(result);

            return result.StatusCode switch
            {
                401 => Unauthorized(),
                400 => BadRequest(),
                _ => StatusCode(result.StatusCode, result)
            };
        }

    }
}
