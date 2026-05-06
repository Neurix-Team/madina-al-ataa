using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Application.DTO.Volunteer
{
    public class UpdateProgressDto
    {
        [Required]
        [Range(1, 100, ErrorMessage = "Progress must be between 1 and 100.")]
        public int AddedProgress { get; set; }
    }
}