using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO.ReviewDto
{
    public class UpdateReviewDto
    {
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public double Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Review text cannot be longer than 1000 characters")]
        public string ReviewText { get; set; }

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow; 
    }
}