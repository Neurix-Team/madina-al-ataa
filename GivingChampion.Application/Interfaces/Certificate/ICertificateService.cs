using GivingChampion.Common.DTO.CertificateDto;
using GivingChampion.Domain.Contexts;
using GivingChampion.Persistance.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.Certificate
{
    public interface ICertificateService
    {
      

            // Create a new certificate
            Task<CertificateReadAllDto?> CreateAsync(CertificateCreateDto dto, CancellationToken cancellationToken = default);

            // Check if a volunteer exists
            Task<bool> CheckVolunteerExists(Guid volunteerId, CancellationToken cancellationToken = default);

            Task<List<CertificateReadAllDto>> GetCertificateByIdAsync(CancellationToken cancellationToken = default);
        }
    }


