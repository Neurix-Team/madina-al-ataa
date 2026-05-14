using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.AvatarDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    /// <summary>
    /// Handles HTTP requests for Avatars.
    /// </summary>
    public class AvatarsController : ControllerBase
    {
        private readonly IAvatarService _avatarService;

        /// <summary>
        /// Performs the AvatarsController operation.
        /// </summary>
        /// <param name="avatarService">Provides the avatarService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public AvatarsController(IAvatarService avatarService)
        {
            _avatarService = avatarService;
        }

        // GET api/avatars/{id}
        [Authorize]
        [HttpGet("{id}")]
        /// <summary>
        /// Retrieves a single record by its identifier.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetById(Guid id)
        {
            var avatar = await _avatarService.GetByIdAsync(id);

            if (avatar == null)
                return NotFound("Avatar not found");

            return Ok(avatar);
        }

        // PUT api/avatars/{id}
        [Authorize]
        [HttpPut("{id}")]
        /// <summary>
        /// Updates an existing record from the supplied request data.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAvatarDto dto)
        {
            var updated = await _avatarService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound("Avatar not found");

            return Ok(new { message = "Avatar updated successfully" });
        }
        // DELETE api/avatars/{id}
        //[Authorize]
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var deleted = await _avatarService.SoftDeleteAsync(id);

        //    if (!deleted)
        //        return NotFound("Avatar not found");

        //    return Ok(new { message = "Avatar deleted successfully" });
        //}
    }
}
