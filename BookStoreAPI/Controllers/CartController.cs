
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
        [Authorize(Policy = "EmailVerifiedOnly")]
        public IActionResult GetTempdata()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new {Email = email});
        }

    }
}
