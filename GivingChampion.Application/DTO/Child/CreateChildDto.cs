using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Application.DTO.Child
{
    public class CreateChildDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDay { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Daily limit must be greater than 0")]
        public decimal DailyLimit { get; set; }

        public bool AllowDonations { get; set; } = true;
    }
}
