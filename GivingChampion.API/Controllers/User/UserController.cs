using GivingChampion.Application.Interfaces.User;
using GivingChampion.Application.DTO.User;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.Api.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/user")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IReadOnlyList<GetUserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<GetUserDto>>> GetAllUsers(
            [FromQuery] PageParameters pageParameters,
            [FromQuery] string? search = null)
        {
            var users = await _userService.GetAllUsersAsync(pageParameters, search);
            return Ok(users);
        }

        [HttpGet("{id:guid}")]
        [Authorize ]
        [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetUserDto>> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdForCurrentUserAsync(id, User.IsInRole("Admin"));
            return user != null ? Ok(user) : NotFound();
        }

        [HttpGet("by-email/{email}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetUserDto>> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            return user != null ? Ok(user) : NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<GetUserDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result<GetUserDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<GetUserDto>>> CreateUserByAdmin([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateUserByAdminAsync(dto);

            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetUserById), new { id = result.Value!.Id }, result);
            }

            return BadRequest(result);
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<GetUserDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result<GetUserDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<GetUserDto>>> CreateAdminUser([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateAdminUserAsync(dto);

            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetUserById), new { id = result.Value!.Id }, result);
            }

            return BadRequest(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<bool>>> UpdateUserByAdmin(Guid id, [FromBody] UpdateUser dto)
        {
            var result = await _userService.UpdateUserByAdminAsync(id, dto);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id:guid}/roles/assign")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<bool>>> AssignRoles(Guid id, [FromBody] RoleManagementRequest request)
        {
            var result = await _userService.AssignRolesAsync(id, request.Roles);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id:guid}/roles/remove")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<bool>>> RemoveRoles(Guid id, [FromBody] RoleManagementRequest request)
        {
            var result = await _userService.RemoveRolesAsync(id, request.Roles);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteMyAccount([FromBody] string? reason = null)
        {
            var result = await _userService.DeleteMyAccountAsync(reason);
            return result.Succeeded ? NoContent() : BadRequest(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUserByAdmin(Guid id, [FromQuery] string? reason = null)
        {
            var result = await _userService.DeleteUserByAdminAsync(id, reason);
            return result.Succeeded ? NoContent() : BadRequest(result);
        }
    }
}
