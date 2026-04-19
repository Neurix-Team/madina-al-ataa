using GivingChampion.Common.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Domain.Entities
{
    public class Avatar : ISoftDeletable
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
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public enum GenderType
    {
        Male,
        Female
    }
}