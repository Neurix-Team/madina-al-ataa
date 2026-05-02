using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.UserLevelDto
{
    public class UserLevelDto
    {
        [Key]
        public Guid Id { get; set; }

        public int Xp { get; set; }

        public int Kp { get; set; }

        public Guid ProfileId { get; set; }

        public Guid LevelId { get; set; }

        public int? LevelNumber { get; set; }

        public int? LevelMaxXp { get; set; }
    }
}