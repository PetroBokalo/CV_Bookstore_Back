using BookStoreAPI.Repositories.Interfaces;
using BookStoreAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        IUserRepository userRepo;
        TokenService tokenService;

        public CartController(IUserRepository userRepo, TokenService tokenService)
        {
            this.userRepo = userRepo;
            this.tokenService = tokenService;
        }


        [HttpGet("temp")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetTempdata()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new {Email = email});
        }

    }
}
