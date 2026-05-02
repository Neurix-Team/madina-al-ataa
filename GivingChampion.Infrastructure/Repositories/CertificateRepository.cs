using GivingChampion.Common.Extensions.Pagination;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Contexts;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;
using CertificateEntity = GivingChampion.Domain.Entities.Certificate;

namespace GivingChampion.Persistance.Repositories
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly AppDbContext _context;

        public CertificateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<CertificateEntity>> GetCertificateByIdAsync(
            Guid userId,
            PageParameters pageParameters,
            CancellationToken cancellationToken = default)
        {
            return await _context.Certificates
                .AsNoTracking()
                .Where(c => c.VolunteerId == userId)
                .OrderByDescending(c => c.IssuedDate)
                .ToPagedListAsync(pageParameters, cancellationToken);
        }

        public async Task<PagedList<CertificateEntity>> GetAllAsync(
            PageParameters pageParameters,
            CancellationToken cancellationToken = default)
        {
            return await _context.Certificates
                .AsNoTracking()
                .OrderByDescending(c => c.IssuedDate)
                .ToPagedListAsync(pageParameters, cancellationToken);
        }

        public async Task<bool> VolunteerExistsAsync(
            Guid volunteerId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Volunteers
                .AsNoTracking()
                .AnyAsync(v => v.Id == volunteerId, cancellationToken);
        }

        public async Task AddAsync(
            CertificateEntity certificate,
            CancellationToken cancellationToken = default)
        {
            await _context.Certificates.AddAsync(certificate, cancellationToken);
        }

        // Get all certificates
        public async Task<List<Certificate>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Certificates
                .AsNoTracking()
                .OrderByDescending(c => c.IssuedDate)
                .ToListAsync(cancellationToken);
        }
    }
}
