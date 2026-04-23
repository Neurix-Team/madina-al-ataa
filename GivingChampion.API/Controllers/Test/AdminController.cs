using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers.Test
{
    [ApiController]
    [Route("api/admin")]
    public sealed class AdminController : ControllerBase
    {
        [HttpGet("dashboard")]
        [ApiExplorerSettings(GroupName = "v1")]
        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            return Ok(new { message = "Admin only." });
        }


    }
}
