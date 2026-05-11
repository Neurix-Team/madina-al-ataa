using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Domain.Entities
{
    public class RewardTransaction : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; }

        public RewardSourceType SourceType { get; set; }

        // Example:
        // ServiceRequestId
        // DonationRequestId
        // MissionId
        // GeoQuestId
        public Guid SourceEntityId { get; set; }

        // Example:
        // VolunteerOrderId
        // DonationOrderId
        // UserMissionId
        // UserGeoQuestId
        public Guid ActionEntityId { get; set; }

        public int XPAmount { get; set; }

        public int KPAmount { get; set; }

        public int ImpactAmount { get; set; }

        [StringLength(300)]
        public string Reason { get; set; } = string.Empty;

        public DateTime EarnedAt { get; set; }
    }
    
}
