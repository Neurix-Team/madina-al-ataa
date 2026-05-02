using System.ComponentModel.DataAnnotations;
using GivingChampion.Common.Enums;
namespace GivingChampion.Application.DTO.AvatarDto
{
    public class UpdateAvatarDto
    {
        [Required]
        public GenderType Gender { get; set; }

        [Required]
        [StringLength(50)]
        public string SkinColor { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string HairColor { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string HairStyle { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ClothesColor { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CharacterName { get; set; } = string.Empty;
    }
}