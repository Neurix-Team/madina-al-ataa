using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Domain.Entities
{
    public class Avatar
    {
        [Key]
        public Guid Id { get; set; }

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

    public enum GenderType
    {
        Male,
        Female
    }
}