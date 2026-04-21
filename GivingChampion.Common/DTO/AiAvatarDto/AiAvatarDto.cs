using System;

namespace GivingChampion.Common.DTO.AiAvatarDto
{
    public class AiAvatarDto
    {
        public Guid Id { get; set; }

        public string FavoriteCategory { get; set; } 

        public int SuccessRate { get; set; }
        
        public string? LastSuggestion { get; set; } 
    }
}