using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.BadgeDto;
using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Services
{
    public class BadgeService : IBadgeService
    {
        private readonly IBadgeRepository _badgeRepository;
        private readonly IMapper _mapper;

        public BadgeService(IBadgeRepository badgeRepository, IMapper mapper)
        {
            _badgeRepository = badgeRepository;
            _mapper = mapper;
        }

        public async Task<List<BadgeDto>> GetAllAsync()
        {
            var badges = await _badgeRepository.GetAllAsync();
            return _mapper.Map<List<BadgeDto>>(badges); // AutoMapper
        }

        public async Task<BadgeDto?> GetByIdAsync(Guid id)
        {
            var badge = await _badgeRepository.GetByIdAsync(id);
            return badge == null ? null : _mapper.Map<BadgeDto>(badge); // AutoMapper
        }

        public async Task<BadgeDto> CreateAsync(CreateBadgeDto dto)
        {
            var badge = _mapper.Map<Badge>(dto);
            await _badgeRepository.AddAsync(badge);
            await _badgeRepository.SaveChangesAsync();

            return _mapper.Map<BadgeDto>(badge); // AutoMapper
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateBadgeDto dto)
        {
            var badge = await _badgeRepository.GetByIdAsync(id);
            if (badge == null)
                return false;

            _mapper.Map(dto, badge); // AutoMapper
            _badgeRepository.Update(badge);
            await _badgeRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var badge = await _badgeRepository.GetByIdAsync(id);
            if (badge == null)
                return false;

            badge.IsDeleted = true;
            badge.DeletedAt = DateTime.UtcNow;
            _badgeRepository.Update(badge);
            await _badgeRepository.SaveChangesAsync();
            return true;
        }
    }
}