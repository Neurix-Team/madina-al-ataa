using System;
using System.ComponentModel.DataAnnotations;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
using GivingChampion.Domain.Enums;

namespace GivingChampion.Domain.Entities
{
    public class Partner : BaseEntity
    {
        public Guid Id { get; set; } 
        /// <summary>
        /// Organization official name
        /// </summary>
        [Required(ErrorMessage = "Organization name is required")]
        [MaxLength(200)]
        public string OrgName { get; set; } = string.Empty;

        /// <summary>
        /// Type of organization (Foundation, Corporate)
        /// </summary>
        [Required(ErrorMessage = "Organization type is required")]
        public OrgType OrgType { get; set; }   

        /// <summary>
        /// Contact phone number of the organization
        /// </summary>
        [Phone(ErrorMessage = "Invalid phone number format")]
        [MaxLength(20)]
        public  string? PhoneNumber { get; set; }

        /// <summary>
        /// Official email address of the organization
        /// </summary>
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(200)]
        public string? Email { get; set; }

        /// <summary>
        /// Indicates whether the partner is verified or not
        /// </summary>
        public bool Verified { get; set; } = false;

        /// <summary>
        /// Total number of projects associated with this partner
        /// </summary>
        [Range(0, int.MaxValue)]
        public int ProjectsCount { get; set; } = 0;

    }
}