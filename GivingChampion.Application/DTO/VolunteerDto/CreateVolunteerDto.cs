using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Application.DTO.Volunteer
{
    public class CreateVolunteerDto
    {
    
            /// <summary>
            /// Skills owned by the volunteer (e.g., Teaching, Design, Coding)
            /// </summary>
            [MaxLength(500)]
            public string Skills { get; set; } = string.Empty;

            /// <summary>
            /// Availability schedule of the volunteer (e.g., Weekends, Evenings)
            /// </summary>
            [MaxLength(200)]
            public string Availability { get; set; } = string.Empty;

            /// <summary>
            /// The ID of the user who is associated with this volunteer
            /// This should be mapped from the logged-in user (Admin).
            /// </summary>
            public Guid UserId { get; set; }
        }
    }


