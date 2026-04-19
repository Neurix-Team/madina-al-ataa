using GivingChampion.Application.Interfaces;
using GivingChampion.Common.Enums;
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

        public async Task<List<Mission>> GetAllActiveAsync()
        {
            return await _context.Missions
                .Include(m => m.Location)
                .Where(m => m.Status == MissionStatus.Open && !m.IsDeleted)
                .OrderBy(m => m.RequiredLevel)
                .ThenBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<List<Mission>> GetByDifficultyAsync(DifficultyLevel difficulty)
        {
            return await _context.Missions
                .Where(m => m.Difficulty == difficulty && !m.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Mission>> GetAvailableForLevelAsync(int userLevel)
        {
            return await _context.Missions
                .Where(m => m.RequiredLevel <= userLevel &&
                           m.Status == MissionStatus.Open &&
                           !m.IsDeleted)
                .Include(m => m.Location)
                .OrderBy(m => m.RequiredLevel)
                .ToListAsync();
        }

        public async Task CreateAsync(Mission mission)
        {
            mission.Status = MissionStatus.Open;
            await _context.Missions.AddAsync(mission);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Mission mission)
        {
            _context.Missions.Update(mission);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid missionId)
        {
            var mission = await _context.Missions.FindAsync(missionId);
            if (mission == null) return;

            mission.IsDeleted = true;
            mission.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}