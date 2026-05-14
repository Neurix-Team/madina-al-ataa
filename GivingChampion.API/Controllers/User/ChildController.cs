using AutoMapper;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Application.DTO.Child;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/child")]
    [Produces("application/json")]
    /// <summary>
    /// Handles HTTP requests for Child.
    /// </summary>
    public class ChildController : ControllerBase
    {
        private readonly IChildService _childService;

        /// <summary>
        /// Performs the ChildController operation.
        /// </summary>
        /// <param name="childService">Provides the childService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public ChildController(IChildService childService)
        {
            _childService = childService;
        }

        // Parent creates a child request
        [HttpPost]
        [Authorize(Roles = "User")]
        /// <summary>
        /// Creates a new record from the supplied request data.
        /// </summary>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<ChildDto>>> CreateChild([FromBody] CreateChildDto dto)
        {
            var result = await _childService.CreateChildAsync(dto);
            return result.Succeeded ? CreatedAtAction(nameof(GetMyChildren), result) : BadRequest(result);
        }

        // Parent views their children
        [HttpGet("my-children")]
        [Authorize(Roles = "User")]
        /// <summary>
        /// Retrieves records owned by the current authenticated user.
        /// </summary>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<List<ChildDto>>>> GetMyChildren()
        {
            var result = await _childService.GetMyChildrenAsync();
            return Ok(result);
        }

        // Admin views pending children
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Performs the GetPendingChildren operation.
        /// </summary>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<List<ChildDto>>>> GetPendingChildren()
        {
            var result = await _childService.GetPendingApprovalsAsync();
            return Ok(result);
        }

        // Admin approves child
        [HttpPost("approve")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Approves the specified record for the next workflow step.
        /// </summary>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result>> ApproveChild([FromBody] ApproveChildDto dto)
        {
            var result = await _childService.ApproveChildAsync(dto.ChildId);
            return result.Succeeded ? NoContent() : BadRequest(result);
        }

        // Admin rejects child
        [HttpPost("reject")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Rejects the specified record according to administrative rules.
        /// </summary>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result>> RejectChild([FromBody] RejectChildDto dto)
        {
            var result = await _childService.RejectChildAsync(dto);
            return result.Succeeded ? NoContent() : BadRequest(result);
        }
    }
}
