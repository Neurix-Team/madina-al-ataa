using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.ReviewDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers.Profile
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET api/reviews
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetById(Guid id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            if (review == null)
                return NotFound();
            return Ok(review);
        }

        // POST api/reviews
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReviewDto>> Create([FromBody] CreateReviewDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _reviewService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/review/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReviewDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _reviewService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        // DELETE api/review/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _reviewService.SoftDeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
