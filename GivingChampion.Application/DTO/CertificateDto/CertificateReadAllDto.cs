using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Application.DTO.CertificateDto
{
    
        public class CertificateReadAllDto
        {
    
            /// <summary>
            /// Certificate unique identifier (ID)
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// Certificate type or title (MaxLength = 200)
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// Name of the person or entity the certificate was issued to (MaxLength = 200)
            /// </summary>
            public string IssuedTo { get; set; }

            /// <summary>
            /// Date the certificate was issued (UTC)
            /// </summary>
            public DateTime IssuedDate { get; set; }

            /// <summary>
            /// Total hours associated with the certificate
            /// </summary>
            public int TotalHours { get; set; }

            /// <summary>
            /// QR Code data (Base64 or short text representation)
            /// </summary>
            [MaxLength(2000)]
            public string QrCode { get; set; }  
        }
    }
