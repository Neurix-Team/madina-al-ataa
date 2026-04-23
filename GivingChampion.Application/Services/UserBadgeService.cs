using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.UserBadgeDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.API.Services
{
    public class UserBadgeService : IUserBadgeService
    {
        private readonly IUserBadgeRepository _userBadgeRepository;
        private readonly IMapper _mapper;

        public UserBadgeService(IUserBadgeRepository userBadgeRepository, IMapper mapper)
        {
            _userBadgeRepository = userBadgeRepository;
            _mapper = mapper;
        }

        public async Task<List<UserBadgeDto>> GetAllByProfileIdAsync(Guid profileId)
        {
            var userBadges = await _userBadgeRepository.GetAllByProfileIdAsync(profileId);
            return _mapper.Map<List<UserBadgeDto>>(userBadges); // AutoMapper
        }

        public async Task<UserBadgeDto?> GetByIdAsync(Guid id)
        {
            var userBadge = await _userBadgeRepository.GetByIdAsync(id);
            return userBadge == null ? null : _mapper.Map<UserBadgeDto>(userBadge); // AutoMapper
        }

        public async Task<UserBadgeDto> CreateAsync(CreateUserBadgeDto dto)
        {
            var userBadge = _mapper.Map<UserBadge>(dto);
            await _userBadgeRepository.AddAsync(userBadge);
            await _userBadgeRepository.SaveChangesAsync();

            return _mapper.Map<UserBadgeDto>(userBadge); // AutoMapper
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateUserBadgeDto dto)
        {
            var userBadge = await _userBadgeRepository.GetByIdAsync(id);
            if (userBadge == null)
                return false;

            _mapper.Map(dto, userBadge); // AutoMapper
            _userBadgeRepository.Update(userBadge);
            await _userBadgeRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var userBadge = await _userBadgeRepository.GetByIdAsync(id);
            if (userBadge == null)
                return false;

            userBadge.IsDeleted = true;
            userBadge.DeletedAt = DateTime.UtcNow;
            _userBadgeRepository.Update(userBadge);
            await _userBadgeRepository.SaveChangesAsync();
            return true;
        }
    }
}