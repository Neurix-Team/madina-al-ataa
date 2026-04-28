using System;
using System.ComponentModel.DataAnnotations;
using GivingChampion.Common.Enums;
﻿using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GivingChampion.Domain.Entities
{
    public class Avatar : BaseEntity
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

        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; }
        //public bool IsDeleted { get; set; }
        //public DateTime? DeletedAt { get; set; }
    }
}