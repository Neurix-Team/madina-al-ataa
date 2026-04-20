using System;
using System.Collections.Generic;
using System.Text;
using GivingChampion.Domain.Entities;
namespace GivingChampion.Persistance.Interfaces
{
    public interface ICertificateRepository
    {
       

            Task<Certificate?> GetCertificateByIdAsync(Guid id, CancellationToken cancellationToken = default);

            Task<bool> VolunteerExistsAsync(Guid volunteerId, CancellationToken cancellationToken = default);

            Task AddAsync(Certificate certificate, CancellationToken cancellationToken = default);
           Task<List<Certificate>> GetAllAsync(CancellationToken cancellationToken = default);  // Method to get all certificates
    }
}
