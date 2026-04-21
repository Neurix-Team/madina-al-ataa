using System;

namespace GivingChampion.Common.DTO.LevelDto
{
    public class LevelDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public int Number { get; set; }

        public int MaxXp { get; set; }
    }
}