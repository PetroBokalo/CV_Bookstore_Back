using BookStoreAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStoreAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {

        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }


        [HttpGet("me")]
        [Authorize(Policy = "EmailVerifiedOnly")]
        public async Task<IActionResult> GetUserdata ()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("User ID not found in token.");

            string userId = userIdClaim.Value;

            var result = await accountService.GetUserDataAsync(userId);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });


            return Ok(result.Data);

        }
    }
}
