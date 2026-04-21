using System;

namespace GivingChampion.Common.DTO.ProfileDto
{
    public class ProfileDto
    {
        public Guid Id { get; set; }= Guid.NewGuid();

        public double Rating { get; set; }

        public int Impact { get; set; }

        public Guid UserId { get; set; }

        public Guid AvatarId { get; set; }

        public Guid LevelId { get; set; }

        public string? AvatarName { get; set; }

        public int? LevelNumber { get; set; }
    }
}