using System.ComponentModel.DataAnnotations;
using GivingChampion.Common.Enums;
namespace GivingChampion.Common.DTO.AvatarDto
{
    public class CreateAvatarDto
    {
        [Required]
        public GenderType Gender { get; set; }

        [Required]
        [StringLength(50)]
        public string SkinColor { get; set; } 

        [Required]
        [StringLength(50)]
        public string HairColor { get; set; } 

        [Required]
        [StringLength(50)]
        public string HairStyle { get; set; } 

        [Required]
        [StringLength(50)]
        public string ClothesColor { get; set; }

        [Required]
        [StringLength(100)]
        public string CharacterName { get; set; } 
    }
}