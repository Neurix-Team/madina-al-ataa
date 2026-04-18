using System;

namespace GivingChampion.Common.DTO.BadgeDto
{
    public class BadgeDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } 

        public string? Description { get; set; }

        public string? Requirement { get; set; }

        public string? Category { get; set; }
    }
}