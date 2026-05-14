using GivingChampion.Application.Interfaces;
using GivingChampion.Application.DTO;
using GivingChampion.Application.DTO.GeoQuestDto;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    /// <summary>
    /// Handles HTTP requests for UserGeoQuests.
    /// </summary>
    public class UserGeoQuestsController : ControllerBase
    {
        private readonly IUserGeoQuestService _userGeoQuestService;

        /// <summary>
        /// Performs the UserGeoQuestsController operation.
        /// </summary>
        /// <param name="userGeoQuestService">Provides the userGeoQuestService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public UserGeoQuestsController(IUserGeoQuestService userGeoQuestService)
        {
            _userGeoQuestService = userGeoQuestService;
        }

        [HttpGet]
        /// <summary>
        /// Retrieves a paged collection of records that match the request.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetAllAsync([FromQuery] PageParameters pageParameters)
        {
            var result = await _userGeoQuestService.GetAllAsync(pageParameters);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:guid}")]
        /// <summary>
        /// Retrieves a single record by its identifier.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _userGeoQuestService.GetByIdAsync(id);
            return Ok(result);
        }

        //[Authorize(Roles = "Admin")]
        //[HttpPut("{id:guid}")]
        //public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserGeoQuestDto dto)
        //{
        //    var result = await _userGeoQuestService.UpdateAsync(id, dto);

        //    if (!result.Succeeded || !result.Value)
        //        return BadRequest(result);

        //    return NoContent();
        //}

        //[Authorize(Roles = "Admin")]
        //[HttpDelete("{id:guid}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var result = await _userGeoQuestService.SoftDeleteAsync(id);

        //    if (!result.Succeeded || !result.Value)
        //        return BadRequest(result);

        //    return NoContent();
        //}

        [Authorize]
        [HttpGet("{id:guid}/status")]
        /// <summary>
        /// Performs the CheckGeoQuestStatus operation.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> CheckGeoQuestStatus(Guid id)
        {
            var result = await _userGeoQuestService.CheckGeoQuestStatus(id);
            return Ok(result);
        }

        [HttpPost("verify-location")]
        /// <summary>
        /// Verifies the supplied data against the configured business rules.
        /// </summary>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> VerifyLocation([FromBody] VerifyLocationDto dto)
        {
            var result = await _userGeoQuestService.UpdateAsyncVerification(dto);
            return Ok(result);
        }
    }
}
