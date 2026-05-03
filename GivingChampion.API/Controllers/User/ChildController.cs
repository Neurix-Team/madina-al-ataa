using AutoMapper;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Application.DTO.Child;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/child")]
    [Produces("application/json")]
    public class ChildController : ControllerBase
    {
        private readonly IChildService _childService;

        public ChildController(IChildService childService)
        {
            _childService = childService;
        }

        // Parent creates a child request
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<Result<ChildDto>>> CreateChild([FromBody] CreateChildDto dto)
        {
            var parentId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");


            var result = await _childService.CreateChildAsync(dto, parentId);
            return result.Succeeded ? CreatedAtAction(nameof(GetMyChildren), result) : BadRequest(result);
        }

        // Parent views their children
        [HttpGet("my-children")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<Result<List<ChildDto>>>> GetMyChildren()
        {
            var parentId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            var result = await _childService.GetMyChildrenAsync(parentId);
            return Ok(result);
        }

        // Admin views pending children
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result<List<ChildDto>>>> GetPendingChildren()
        {
            var result = await _childService.GetPendingApprovalsAsync();
            return Ok(result);
        }

        // Admin approves child
        [HttpPost("approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result>> ApproveChild([FromBody] ApproveChildDto dto)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            var result = await _childService.ApproveChildAsync(dto.ChildId, adminId);
            return result.Succeeded ? NoContent() : BadRequest(result);
        }

        // Admin rejects child
        [HttpPost("reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result>> RejectChild([FromBody] RejectChildDto dto)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            var result = await _childService.RejectChildAsync(dto, adminId);
            return result.Succeeded ? NoContent() : BadRequest(result);
        }
    }
}