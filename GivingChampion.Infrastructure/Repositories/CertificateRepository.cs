using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Persistance.Repositories
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly AppDbContext _context;

        public CertificateRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get a certificate by its ID
        public async Task<Certificate?> GetCertificateByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Certificates
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        // Check if a volunteer exists by their ID
        public async Task<bool> VolunteerExistsAsync(Guid volunteerId, CancellationToken cancellationToken = default)
        {
            return await _context.Volunteers
                .AsNoTracking()
                .AnyAsync(v => v.Id == volunteerId, cancellationToken);
        }

        // Add a new certificate to the database
        public async Task AddAsync(Certificate certificate, CancellationToken cancellationToken = default)
        {
            await _context.Certificates.AddAsync(certificate, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);  // Save changes inside the repository
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