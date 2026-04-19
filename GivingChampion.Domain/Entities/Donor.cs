using GivingChampion.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GivingChampion.Domain.Entities
{
    [PrimaryKey("Id")]
    public class Donor : ISoftDeletable
    {
        public Guid Id { get; set; }
        //public string DonorType { get; set; }
        public decimal TotalDonated { get; set; }
        public int PreferedCategory { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = null;
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
