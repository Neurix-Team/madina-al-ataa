using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.ReviewDto
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        [Range(0, 5)]
        public double Rating { get; set; }

        public string ReviewText { get; set; }

        public Guid UserId { get; set; }

        public Guid AvatarId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}