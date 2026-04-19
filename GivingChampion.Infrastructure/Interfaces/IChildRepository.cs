using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IChildRepository
    {
        Task<Child?> GetByIdAsync(Guid id);
        Task<List<Child>> GetByParentIdAsync(Guid parentId);
        Task<List<Child>> GetPendingApprovalsAsync();
        Task CreateAsync(Child child);
        Task ApproveAsync(Guid childId, Guid approvedById);
        Task RejectAsync(Guid childId, string reason, Guid rejectedById);
        Task SoftDeleteAsync(Guid childId);
    }
}
