using GivingChampion.Application.Interfaces;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Extensions.Pagination;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Infrastructure.Persistence.Repositories
{
    public class UserMissionRepository : IUserMissionRepository
    {
        private readonly AppDbContext _context;

        public UserMissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserMission?> GetByIdAsync(Guid id)
        {
            return await _context.UserMissions
                .Include(um => um.Mission)
                .Include(um => um.User)
                .FirstOrDefaultAsync(um => um.Id == id && !um.IsDeleted);
        }

        public async Task<UserMission?> GetByUserAndMissionAsync(Guid userId, Guid missionId)
        {
            return await _context.UserMissions
                .Include(um => um.Mission)
                .FirstOrDefaultAsync(um => um.UserId == userId &&
                                          um.MissionId == missionId &&
                                          !um.IsDeleted);
        }

        public async Task<List<UserMission>> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserMissions
                .Include(um => um.Mission)
                .Where(um => um.UserId == userId && !um.IsDeleted)
                .OrderByDescending(um => um.StartedAt)
                .ToListAsync();
        }

        public async Task<PagedList<UserMission>> GetActiveByUserIdAsync(PageParameters pageParameters, Guid userId)
        {
            return await _context.UserMissions
                .Include(um => um.Mission)
                .Where(um => um.UserId == userId &&
                            um.Status != MissionStatus.Completed &&
                            !um.IsDeleted).ToPagedListAsync(pageParameters);
        }

        public async Task CreateAsync(UserMission userMission)
        {
            userMission.StartedAt = DateTime.UtcNow;
            userMission.Status = MissionStatus.InProgress;
            await _context.UserMissions.AddAsync(userMission);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserMission userMission)
        {
            if (userMission.Progress >= 100 && userMission.Status != MissionStatus.Completed)
            {
                userMission.Status = MissionStatus.Completed;
                userMission.CompletedAt = DateTime.UtcNow;
            }

            _context.UserMissions.Update(userMission);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid userMissionId)
        {
            var userMission = await _context.UserMissions.FindAsync(userMissionId);
            if (userMission == null) return;

            userMission.IsDeleted = true;
            userMission.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsMissionStartedAsync(Guid userId, Guid missionId)
        {
            return await _context.UserMissions
                .AnyAsync(um => um.UserId == userId &&
                               um.MissionId == missionId && um.Status == MissionStatus.Completed &&
                               !um.IsDeleted);
        }
    }
}