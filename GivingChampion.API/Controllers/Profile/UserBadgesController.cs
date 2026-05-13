using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.UserBadge;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    /// <summary>
    /// Handles HTTP requests for UserBadges.
    /// </summary>
    public class UserBadgesController : ControllerBase
    {
        private readonly IUserBadgeService _userBadgeService;

        /// <summary>
        /// Performs the UserBadgesController operation.
        /// </summary>
        /// <param name="userBadgeService">Provides the userBadgeService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public UserBadgesController(IUserBadgeService userBadgeService)
        {
            _userBadgeService = userBadgeService;
        }

        // GET api/UserBadges
        [HttpGet]
        /// <summary>
        /// Retrieves a paged collection of records that match the request.
        /// </summary>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetAll()
        {
            var userBadges = await _userBadgeService.GetAllByUserIdAsync();
            return Ok(userBadges);
        }

        // GET api/UserBadges/{id}
        [HttpGet("{id:guid}")]
        /// <summary>
        /// Retrieves a single record by its identifier.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetById(Guid id)
        {
            var userBadge = await _userBadgeService.GetByIdAsync(id);
            return Ok(userBadge);
        }

        // GET api/UserBadges/profile/{userId}
        [Authorize(Roles = "Admin")]
        [HttpGet("profile/{userId:guid}")]
        /// <summary>
        /// Performs the GetByUserId operation.
        /// </summary>
        /// <param name="userId">Provides the userId value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var userBadges = await _userBadgeService.GetAllByUserIdAsync(userId);
            return Ok(userBadges);
        }

        //// POST api/UserBadges
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CreateUserBadgeDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var created = await _userBadgeService.CreateAsync(dto);

        //    return CreatedAtAction(
        //        nameof(GetById),
        //        new { id = created.Id },
        //        created
        //    );
        //}

        //// PUT api/UserBadges/{id}
        //[HttpPut("{id:guid}")]
        //public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserBadgeDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var updated = await _userBadgeService.UpdateAsync(id, dto);

        //    if (!updated)
        //        return NotFound("User badge not found");

        //    return NoContent();
        //}

        //// DELETE api/UserBadges/{id}
        //[HttpDelete("{id:guid}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var deleted = await _userBadgeService.SoftDeleteAsync(id);

        //    if (!deleted)
        //        return NotFound("User badge not found");

        //    return NoContent();
        //}
    }
}
