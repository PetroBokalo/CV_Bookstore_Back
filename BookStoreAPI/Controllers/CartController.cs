using BookStoreAPI.Repositories.Interfaces;
using BookStoreAPI.Services.Implementations;
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

        public CartController()
        {

        }


        [HttpGet("temp")]
        [Authorize]
        public IActionResult GetTempdata()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new {Email = email});
        }

    }
}
