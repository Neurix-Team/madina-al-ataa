using GivingChampion.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Domain.Entities
{
    public class AiAvatar : ISoftDeletable
    {
        [Key] 
        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string FavoriteCategory { get; set; }
        [Range(0, 10)]
        public int SuccessRate { get; set; }
        [Required]
        [StringLength(500)]

        public string LastSuggestion { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
