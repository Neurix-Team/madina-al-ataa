using GivingChampion.Application.Interfaces.User;
using GivingChampion.Common.DTO.User;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingChampion.Api.Controllers
{
    /// <summary>
    /// API endpoints for managing users in the GivingChampion platform.
    /// Supports user registration, admin user management, role assignment, and profile operations.
    /// </summary>
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

        #region Read Operations

        /// <summary>
        /// Retrieves a paginated list of all users.
        /// </summary>
        /// <param name="page">Page number (starts at 1)</param>
        /// <param name="pageSize">Number of items per page (max 100)</param>
        /// <param name="search">Optional search term (matches username, email, or full name)</param>
        /// <returns>List of users with basic information</returns>
        /// <response code="200">Returns the paginated list of users</response>
        /// <response code="401">Unauthorized - User is not authenticated</response>
        /// <response code="403">Forbidden - User does not have Admin role</response>
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

        /// <summary>
        /// Retrieves a specific user by their unique identifier.
        /// Users can only view their own profile unless they are an Admin.
        /// </summary>
        /// <param name="id">The unique identifier of the user</param>
        /// <returns>User details including roles</returns>
        /// <response code="200">Returns the requested user</response>
        /// <response code="404">User not found</response>
        /// <response code="403">Forbidden - Attempting to access another user's profile without Admin rights</response>
        [HttpGet("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetUserDto>> GetUserById(Guid id)
        {
            // Security: Users can only view their own profile unless they are Admin
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!User.IsInRole("Admin") && currentUserId != id.ToString())
                return Forbid();

            var user = await _userService.GetUserByIdAsync(id);
            return user != null ? Ok(user) : NotFound();
        }

        /// <summary>
        /// Retrieves a user by their email address (Admin only).
        /// </summary>
        /// <param name="email">The email address of the user</param>
        /// <returns>User details including roles</returns>
        /// <response code="200">Returns the requested user</response>
        /// <response code="404">User not found</response>
        [HttpGet("by-email/{email}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetUserDto>> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            return user != null ? Ok(user) : NotFound();
        }

        #endregion

        #region Create Operations

        /// <summary>
        /// Creates a new user by an administrator.
        /// </summary>
        /// <param name="dto">User creation details</param>
        /// <returns>The created user</returns>
        /// <response code="201">User successfully created by admin</response>
        /// <response code="400">Invalid input or user already exists</response>
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

        /// <summary>
        /// Creates a new user with administrative privileges.
        /// </summary>
        /// <param name="dto">Admin user creation details</param>
        /// <returns>The created admin user</returns>
        /// <response code="201">Admin user successfully created</response>
        /// <response code="400">Invalid input or user already exists</response>
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

        #endregion

        #region Update & Role Management

        /// <summary>
        /// Updates a user's profile information (Admin only).
        /// </summary>
        /// <param name="id">User ID to update</param>
        /// <param name="dto">Updated user information</param>
        /// <returns>Success status</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<bool>>> UpdateUserByAdmin(Guid id, [FromBody] UpdateUser dto)
        {
            var result = await _userService.UpdateUserByAdminAsync(id, dto);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Assigns one or more roles to a user (Admin only).
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="roles">List of roles to assign</param>
        [HttpPost("{id:guid}/roles/assign")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<bool>>> AssignRoles(Guid id, [FromBody] IEnumerable<string> roles)
        {
            var result = await _userService.AssignRolesAsync(id, roles);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Removes one or more roles from a user (Admin only).
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="roles">List of roles to remove</param>
        [HttpPost("{id:guid}/roles/remove")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<bool>>> RemoveRoles(Guid id, [FromBody] IEnumerable<string> roles)
        {
            var result = await _userService.RemoveRolesAsync(id, roles);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        #endregion

        #region Delete Operations

        /// <summary>
        /// Allows the currently authenticated user to delete their own account.
        /// </summary>
        /// <param name="reason">Optional reason for account deletion</param>
        [HttpDelete("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteMyAccount([FromBody] string? reason = null)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");
            var result = await _userService.DeleteMyAccountAsync(userId, reason);

            return result.Succeeded ? NoContent() : BadRequest(result);
        }

        /// <summary>
        /// Permanently deletes a user (Admin only).
        /// </summary>
        /// <param name="id">User ID to delete</param>
        /// <param name="reason">Optional reason for deletion (logged for audit)</param>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUserByAdmin(Guid id, [FromQuery] string? reason = null)
        {
            var result = await _userService.DeleteUserByAdminAsync(id, reason);
            return result.Succeeded ? NoContent() : BadRequest(result);
        }

        #endregion
    }
}