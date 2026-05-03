using GivingChampion.Application.Interfaces.Admin;
using GivingChampion.Application.DTO.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin")]
    public sealed class AdminController : ControllerBase
    {
        private readonly IAdministratorService _administratorService;

        public AdminController(IAdministratorService administratorService)
        {
            _administratorService = administratorService;
        }

        [HttpGet("dashboard")]
        [ApiExplorerSettings(GroupName = "v1")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(DashboardMetricsDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DashboardMetricsDto>> Dashboard()
        {
            var dashboardMetrics = await _administratorService.GetDashboardMetricsAsync();

            return Ok(dashboardMetrics);
        }
    }
}
