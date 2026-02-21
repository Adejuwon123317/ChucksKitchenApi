using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChucksKitchenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        [HttpGet("Me")]
        public IActionResult GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var Role = User.FindFirstValue(ClaimTypes.Role);
            return Ok(new
            {
                userId,
                email,
                Role,
                Message = "You are Authenticated"
            });
        }
    }
}
