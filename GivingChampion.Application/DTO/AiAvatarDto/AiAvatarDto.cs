using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.AiAvatarDto
{
    public class AiAvatarDto
    {
        [Key]
        public Guid Id { get; set; }

        public string? FavoriteCategory { get; set; }
        [Range(0, 10)]
        public int SuccessRate { get; set; }
        
        public string? LastSuggestion { get; set; } 
    }
}