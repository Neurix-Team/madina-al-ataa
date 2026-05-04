using GivingChampion.Application.DTO.CertificateDto;
using GivingChampion.Persistence.Contexts;
using GivingChampion.Persistance.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using GivingChampion.Domain.Entities;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
namespace GivingChampion.Application.Interfaces.Certificate
{
    public interface ICertificateService
    {
        Task<CertificateReadAllDto?> CreateAsync(
            CertificateCreateDto dto,
            CancellationToken cancellationToken = default);

        Task<bool> CheckVolunteerExists(
            Guid volunteerId,
            CancellationToken cancellationToken = default);
        Task<Result<PagedList<CertificateReadAllDto>>> GetMyCertificatesAsync(
    PageParameters pageParameters,
    CancellationToken cancellationToken = default);

        Task<Result<PagedList<CertificateReadAllDto>>> GetCertificatesByIdAsync(
            Guid userId,
            PageParameters pageParameters,
            CancellationToken cancellationToken = default);
     
        Task<Result<PagedList<CertificateReadAllDto>>> GetAllAsync(
            PageParameters pageParameters,
            CancellationToken cancellationToken = default);
    }
}