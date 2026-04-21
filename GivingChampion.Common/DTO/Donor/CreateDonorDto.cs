using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Common.DTO.Donor
{
    public class CreateDonorDto
    {
        //[Required]
        //public Guid UserId { get; set; }

        public int PreferredCategory { get; set; } = 0; // Default category
    }
}
