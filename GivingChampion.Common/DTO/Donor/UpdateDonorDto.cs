using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Common.DTO.Donor
{
    public class UpdateDonorDto
    {
        public int PreferredCategory { get; set; }
        //public decimal? TotalDonated { get; set; } // Optional - usually updated via donations
    }
}
