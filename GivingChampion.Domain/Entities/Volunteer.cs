using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace GivingChampion.Domain.Entities
{
    public class Volunteer
    {
		/// <summary>
		/// Primary key for Volunteer entity
		/// </summary>
		public Guid Id { get; set; }
		/// <summary>
		/// Skills owned by the volunteer (e.g., Teaching, Design, Coding)
		/// </summary>
		[MaxLength(500)]
        public string Skills { get; set; } = string.Empty;

        /// <summary>
        /// Availability schedule of the volunteer (e.g., Weekends, Evenings)
        /// </summary>
        [MaxLength(200)]
        public string Availability { get; set; } = string.Empty;

        /// <summary>
        /// Total volunteer hours accumulated
        /// </summary>
        [Range(0, int.MaxValue)]
        public int TotalHours { get; set; }

        /// <summary>
        /// Foreign key reference to Application User (Identity User)
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Navigation property to the related Application User
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }



	}
}