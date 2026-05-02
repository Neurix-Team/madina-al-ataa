using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Application.DTO.CertificateDto
{
    
        public class CertificateCreateDto
        {
        public Guid VolunteerId { get; set; }  // This is used to check if the volunteer exists
        /// <summary>
        /// Certificate type or title (Required, MaxLength = 200)
        /// </summary>
        [Required(ErrorMessage = "Certificate type is required")]
            [MaxLength(200)]
            public string Type { get; set; }

            /// <summary>
            /// Name of the person or entity the certificate was issued to (Required, MaxLength = 200)
            /// </summary>
            [Required(ErrorMessage = "IssuedTo is required")]
            [MaxLength(200)]
            public string IssuedTo { get; set; }  // This will be dynamically linked to the volunteer who completed the task

            /// <summary>
            /// QR Code data (stored as Base64 or short text representation), optional for the system.
            /// </summary>
            [MaxLength(2000)]
            public string QrCode { get; set; } = string.Empty;
        }
    }
