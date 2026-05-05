using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Application.DTO.GeoQuestDto
{
    public class GeoQuestDto
    {
        [Key]
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public Guid LocationId { get; set; }

    }
}
