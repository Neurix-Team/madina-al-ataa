using GivingChampion.Common.Pagination;
using CertificateEntity = GivingChampion.Domain.Entities.Certificate;

namespace GivingChampion.Persistance.Interfaces
{
    public interface ICertificateRepository
    {
        Task<PagedList<CertificateEntity>> GetCertificateByIdAsync(
            Guid userId,
            PageParameters pageParameters,
            CancellationToken cancellationToken = default);

        Task<PagedList<CertificateEntity>> GetAllAsync(
            PageParameters pageParameters,
            CancellationToken cancellationToken = default);

        Task<bool> VolunteerExistsAsync(
            Guid volunteerId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            CertificateEntity certificate,
            CancellationToken cancellationToken = default);
    }
}