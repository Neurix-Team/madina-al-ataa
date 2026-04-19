using GivingChampion.Common.Enums;

namespace GivingChampion.Common.DTO.PartnerDto
{
 
    public class PartnerDto
    {
         /// <summary>
        /// Unique identifier of the partner.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Official organization name.
        /// </summary>
        public string OrgName { get; set; } = string.Empty;

        /// <summary>
        /// Organization type as enum value.
        /// </summary>
        public OrgType OrgType { get; set; }

        /// <summary>
        /// Organization type as readable text.
        /// </summary>
        public string OrgTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Contact phone number of the organization.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Official email address of the organization.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Indicates whether the partner has been verified by admin.
        /// </summary>
        public bool Verified { get; set; }

        /// <summary>
        /// Number of projects related to this partner.
        /// </summary>
        public int ProjectsCount { get; set; }

        /// <summary>
        /// Date and time when the partner was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the partner was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}