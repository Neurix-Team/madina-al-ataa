using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.DTO.Volunteer
{
    public class VolunteerDto
    {
      
            /// <summary>
            /// Primary key for Volunteer entity
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// Skills owned by the volunteer (e.g., Teaching, Design, Coding)
            /// </summary>
            public string Skills { get; set; } = string.Empty;

            /// <summary>
            /// Availability schedule of the volunteer (e.g., Weekends, Evenings)
            /// </summary>
            public string Availability { get; set; } = string.Empty;

            /// <summary>
            /// Total volunteer hours accumulated
            /// </summary>
            public int TotalHours { get; set; }

            /// <summary>
            /// Foreign key reference to Application User (Identity User)
            /// </summary>
            public Guid UserId { get; set; }
        }
    }


