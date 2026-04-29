using GivingChampion.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Common.DTO.VolunteerOrder
{
    public class UpdateVolunteerOrderDto
    {
            /// Current status of the volunteer order.
            /// Example: Pending, Approved, Rejected, Completed.
            /// </summary>
            [Required(ErrorMessage = "Status is required.")]
            [EnumDataType(typeof(OrderStatus), ErrorMessage = "Invalid order status.")]
            public OrderStatus Status { get; set; }


         

         
        }
    }
