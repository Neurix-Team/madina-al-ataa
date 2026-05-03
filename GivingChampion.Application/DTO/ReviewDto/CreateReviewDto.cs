using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.ReviewDto
{
    public class CreateReviewDto
    {
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public double Rating { get; set; }

        [Required(ErrorMessage = "Review text is required")]
        [StringLength(1000, ErrorMessage = "Review text cannot be longer than 1000 characters")]
        public string ReviewText { get; set; }

        public Guid UserId { get; set; }

        public Guid AvatarId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}