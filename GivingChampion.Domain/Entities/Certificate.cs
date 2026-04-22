using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GivingChampion.Domain.Entities
{
    public class Certificate : BaseEntity
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Certificate type or title
        /// </summary>
        [Required(ErrorMessage = "Certificate type is required")]
        [MaxLength(200)]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Name of the person or entity the certificate was issued to
        /// </summary>
        [Required(ErrorMessage = "IssuedTo is required")]
        [MaxLength(200)]
        public string IssuedTo { get; set; } = string.Empty;

        /// <summary>
        /// Date the certificate was issued (UTC)
        /// </summary>
        [Required(ErrorMessage = "IssuedDate is required")]
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;


        [Required]
        public Guid VolunteerId { get; set; }

        [ForeignKey(nameof(VolunteerId))]
        public Volunteer? Volunteer { get; set; }

        /// <summary>
        /// Total hours associated with this certificate (e.g., training hours, volunteer hours)
        /// </summary>
        [Range(1, int.MaxValue)]
        public int TotalHours { get; set; } = 1;

        /// <summary>
        /// QR code data (stored as Base64 or short text representation)
        /// </summary>
        [MaxLength(2000)]
        public string QrCode { get; set; } = string.Empty;
        //public bool IsDeleted { get; set; }
        //public DateTime? DeletedAt { get; set; }
    }
}

    
