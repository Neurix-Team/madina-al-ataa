using GivingChampion.Common.DTO.CertificateDto;
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

        Task<Result<PagedList<CertificateReadAllDto>>> GetCertificatesByIdAsync(
            Guid userId,
            PageParameters pageParameters,
            CancellationToken cancellationToken = default);

        Task<Result<PagedList<CertificateReadAllDto>>> GetAllAsync(
            PageParameters pageParameters,
            CancellationToken cancellationToken = default);
    }
}