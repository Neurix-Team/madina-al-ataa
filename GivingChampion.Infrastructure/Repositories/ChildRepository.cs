using GivingChampion.Persistence.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Infrastructure.Persistence.Repositories
{
    public class EfChildRepository : IChildRepository
    {
        private readonly AppDbContext _context;

        public EfChildRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Child?> GetByIdAsync(Guid id)
        {
            return await _context.Children
                .Include(c => c.Parent)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Child?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Children
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
        }

        public async Task<List<Child>> GetByParentIdAsync(Guid parentId)
        {
            return await _context.Children
                .Where(c => c.ParentId == parentId && !c.IsDeleted)
                .Include(c => c.User)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Child>> GetPendingApprovalsAsync()
        {
            return await _context.Children
                .Where(c => c.Status == ObjectStatus.Pending && !c.IsDeleted)
                .Include(c => c.Parent)
                .Include(c => c.User)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task CreateAsync(Child child)
        {
            child.Status = ObjectStatus.Pending;
            child.CreatedAt = DateTime.UtcNow;
            await _context.Children.AddAsync(child);
        }

        public async Task ApproveAsync(Guid childId, Guid approvedById)
        {
            var child = await _context.Children
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == childId);

            if (child == null) return;

            child.Status = ObjectStatus.Approved;
            child.ApprovedAt = DateTime.UtcNow;
            child.ApprovedById = approvedById;

            // Activate the child's user account
            if (child.User != null)
            {
                child.User.EmailConfirmed = true;
                // You can also set LockoutEnabled = false if needed
            }

        }

        public async Task RejectAsync(Guid childId, string rejectionReason, Guid rejectedById)
        {
            var child = await _context.Children.FirstOrDefaultAsync(x => x.Id == childId);
            if (child == null) return;

            child.Status = ObjectStatus.Rejected;
            child.RejectionReason = rejectionReason;
            child.ApprovedById = rejectedById;

        }

        public async Task SoftDeleteAsync(Guid childId)
        {
            var child = await _context.Children.FirstOrDefaultAsync(x => x.Id == childId);
            if (child == null) return;

            _context.Children.Remove(child);
        }

        public Task UpdateAsync(Child child)
        {
            _context.Children.Update(child);
            return Task.CompletedTask;
        }
    }
}
