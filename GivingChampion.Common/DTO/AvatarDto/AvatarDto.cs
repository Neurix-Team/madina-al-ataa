using System;
using GivingChampion.Common.Enums;

namespace GivingChampion.Common.DTO.AvatarDto
{
    public class AvatarDto
    {
        public Guid Id { get; set; }

        public GenderType Gender { get; set; }

        public string SkinColor { get; set; } 

        public string HairColor { get; set; } 

        public string HairStyle { get; set; }

        public string ClothesColor { get; set; } 

        public string CharacterName { get; set; } 
    }
}