using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.DTO.Donor
{
    public class DonorDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalDonated { get; set; }
        public int PreferredCategory { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
