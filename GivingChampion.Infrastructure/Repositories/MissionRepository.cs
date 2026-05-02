using GivingChampion.Application.Interfaces;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Extensions.Pagination;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Infrastructure.Persistence.Repositories
{
    public class MissionRepository : IMissionRepository
    {
        private readonly AppDbContext _context;

        public MissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Mission?> GetByIdAsync(Guid id)
        {
            return await _context.Missions
                .Include(m => m.Location)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        }

        public async Task<PagedList<Mission>> GetAllActiveAsync(PageParameters pageParameters)
        {
            var query = await _context.Missions
                .Include(m => m.Location)
                .Where(m => m.Status == MissionStatus.Open && !m.IsDeleted)
                .OrderBy(m => m.RequiredLevel)
                .ThenBy(m => m.Title).ToPagedListAsync(pageParameters);

            return query;
        }

        public async Task<PagedList<Mission>> GetByDifficultyAsync(DifficultyLevel difficulty, PageParameters pageParameters)
        {
            var query = await _context.Missions
                .Where(m => m.Difficulty == difficulty && !m.IsDeleted)
                .ToPagedListAsync(pageParameters);

            return query;
        }

        public async Task<PagedList<Mission>> GetAvailableForLevelAsync(int userLevel, PageParameters pageParameters)
        {
            var query = await _context.Missions
                .Where(m => m.RequiredLevel <= userLevel &&
                           m.Status == MissionStatus.Open &&
                           !m.IsDeleted)
                .Include(m => m.Location)
                .OrderBy(m => m.RequiredLevel)
                .ToPagedListAsync(pageParameters);

            return query;
        }

        public async Task CreateAsync(Mission mission)
        {
            mission.Status = MissionStatus.Open;
            await _context.Missions.AddAsync(mission);
        }

        public Task UpdateAsync(Mission mission)
        {
            _context.Missions.Update(mission);
            return Task.CompletedTask;
        }

        public async Task SoftDeleteAsync(Guid missionId)
        {
            var mission = await _context.Missions.FindAsync(missionId);
            if (mission == null) return;

            //mission.IsDeleted = true;
            //mission.DeletedAt = DateTime.UtcNow;

            _context.Missions.Remove(mission);
        }
    }
}
