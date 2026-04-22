using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Common.DTO.UserGeoQuestDto
{
    public class UserGeoQuestDto
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        public Guid GeoQuestId { get; set; }
        public DateTime? StartedAt { get; set; } = DateTime.Now;
        public double LocationLatitude { get; set; }
        public double LocationLongitude { get; set; }
        public bool IsLocationVerified { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
