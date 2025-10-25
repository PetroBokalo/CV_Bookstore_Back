using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetUserdata ()
        {
            return Ok("Here is user data");
        }
    }
}
