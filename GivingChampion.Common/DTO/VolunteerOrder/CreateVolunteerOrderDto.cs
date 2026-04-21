using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Common.DTO.VolunteerOrder
{

        /// <summary>
        /// DTO used when creating a new volunteer order.
        /// Contains the required data to assign a volunteer to a service request.
        /// </summary>
        public class CreateVolunteerOrderDto
        {
            /// <summary>
            /// Type of service the volunteer will provide.
            /// Example: Medical, Teaching, Delivery.
            /// </summary>
            [Required(ErrorMessage = "Service type is required.")]
            [StringLength(100, ErrorMessage = "Service type cannot exceed 100 characters.")]
            public string ServiceType { get; set; } = string.Empty;

            /// <summary>
            /// Optional description of the volunteer task.
            /// </summary>
            [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
            public string? Description { get; set; }

            /// <summary>
            /// Scheduled date and time for the volunteer task.
            /// </summary>
            [Required(ErrorMessage = "Schedule date is required.")]
            public DateTime ScheduleDate { get; set; }

            /// <summary>
            /// Duration of the volunteer task in hours.
            /// Must be greater than 0.
            /// </summary>
            [Required(ErrorMessage = "Duration is required.")]
            [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 hour.")]
            public int Duration { get; set; }

            /// <summary>
            /// Location where the volunteer service will take place.
            /// </summary>
            [Required(ErrorMessage = "Location is required.")]
            [StringLength(250, ErrorMessage = "Location cannot exceed 250 characters.")]
            public string Location { get; set; } = string.Empty;

            /// <summary>
            /// Optional additional notes related to the volunteer order.
            /// </summary>
            [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
            public string? Notes { get; set; }

            /// <summary>
            /// Foreign key of the related service request.
            /// </summary>
            [Required(ErrorMessage = "Service request ID is required.")]
            public Guid ServiceRequestId { get; set; }

      
        }
    }

