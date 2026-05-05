using System;
using System.Collections.Generic;
using System.Text;
using GivingChampion.Common.Enums;

namespace GivingChampion.Application.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations;

    namespace GivingChampion.Application.DTO.ActivityDto
    {
        public class ActivityDto
        {
            public Guid EntityId { get; set; }
            public Guid UserId { get; set; }
            public string Description { get; set; } = string.Empty;
            public string UserName { get; set; }
            public string Action { get; set; }
            public string EntityType { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
