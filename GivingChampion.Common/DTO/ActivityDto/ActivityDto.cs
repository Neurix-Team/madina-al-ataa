using System;
using System.Collections.Generic;
using System.Text;
using GivingChampion.Common.Enums;

namespace GivingChampion.Common.DTO
{
    using System;

    namespace GivingChampion.Common.DTO.ActivityDto
    {
        public class ActivityDto
        {
            public Guid Id { get; set; }= Guid.NewGuid();

            public string Name { get; set; } = string.Empty;

            public string Description { get; set; } = string.Empty;

            public DateTime CreatedAt { get; set; }
        }
    }
}
