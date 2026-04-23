using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Common.DTO.VolunteerOrder
{

        public class CreateVolunteerOrderDto
        { 
            [Required(ErrorMessage = "Service request ID is required.")]
            public Guid ServiceRequestId { get; set; } 

      
        }
    }

