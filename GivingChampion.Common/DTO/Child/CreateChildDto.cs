using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Common.DTO.Child
{
    public class CreateChildDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Daily limit must be greater than 0")]
        public decimal DailyLimit { get; set; }

        public bool AllowDonations { get; set; } = true;
    }
}
