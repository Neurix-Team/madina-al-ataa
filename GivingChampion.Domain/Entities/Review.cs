using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GivingChampion.Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid Id { get; set; }
        [Range(0, 5)]
        public double Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
        public Guid ReviewerId { get; set; }
        public ApplicationUser Reviewer { get; set; }
        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; }
    }
}
