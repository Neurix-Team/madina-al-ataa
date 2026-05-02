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

        // Get all certificates for a specific user by UserId
        public async Task<List<Certificate>> GetCertificateByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Certificates
                .AsNoTracking()
                .Where(c => c.VolunteerId == userId)  
                .ToListAsync(cancellationToken);
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
