using System.ComponentModel.DataAnnotations;
using GivingChampion.Common.Enums;

namespace GivingChampion.Application.DTO.Partner
{
    public class UpdatePartnerDto
    {
        /// <summary>
        /// Organization official name (Required)
        /// </summary>
        [Required(ErrorMessage = "Organization name is required")]
        [MaxLength(200)]
        public string OrgName { get; set; } = string.Empty;

        /// <summary>
        /// Organization type (Required)
        /// </summary>
        [Required(ErrorMessage = "Organization type is required")]
        public OrgType OrgType { get; set; }

        /// <summary>
        /// Contact phone number of the organization (Optional)
        /// </summary>
        [Phone(ErrorMessage = "Invalid phone number format")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Official email address of the organization (Optional)
        /// </summary>
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(200)]
        public string? Email { get; set; }

     

    }
}