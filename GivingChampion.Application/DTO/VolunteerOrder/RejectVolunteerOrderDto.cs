using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.VolunteerOrder
{
    public class RejectVolunteerOrderDto
    {
        [Required(ErrorMessage = "Rejection reason is required")]
        [MaxLength(500, ErrorMessage = "Rejection reason cannot exceed 500 characters")]
        public string RejectionReason { get; set; } = string.Empty;
    }
}