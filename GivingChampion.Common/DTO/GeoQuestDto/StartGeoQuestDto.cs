using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Common.DTO.GeoQuestDto
{
    public class StartGeoQuestDto
    {
        public Guid Id { get; set; }
        public Guid GeoQuestId { get; set; }
        public Guid UserId { get; set; }
        public Guid UserGeoQuestId { get; set; }
    }
}
