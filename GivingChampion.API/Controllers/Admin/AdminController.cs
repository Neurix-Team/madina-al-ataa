using GivingChampion.Application.Interfaces.Admin;
using GivingChampion.Application.DTO.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin")]
    /// <summary>
    /// Handles HTTP requests for Admin.
    /// </summary>
    public sealed class AdminController : ControllerBase
    {
        private readonly IAdministratorService _administratorService;

        /// <summary>
        /// Performs the AdminController operation.
        /// </summary>
        /// <param name="administratorService">Provides the administratorService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public AdminController(IAdministratorService administratorService)
        {
            _administratorService = administratorService;
        }

        [HttpGet("dashboard")]
        [ApiExplorerSettings(GroupName = "v1")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(DashboardMetricsDto), StatusCodes.Status200OK)]
        /// <summary>
        /// Performs the Dashboard operation.
        /// </summary>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<DashboardMetricsDto>> Dashboard()
        {
            var dashboardMetrics = await _administratorService.GetDashboardMetricsAsync();

            return Ok(dashboardMetrics);
        }
    }
}
