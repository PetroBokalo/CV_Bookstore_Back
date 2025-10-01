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

            var result = await authService.RegisterAsync(registerUserDto);
            if (result.Success)
                return Created("dummy", result); // замість dummy треба вказати куди йти щоб отримати доступ до клієнта (якийсь endpoint) 
            else
                return BadRequest(result);
        }


    }
}
