using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.UserLevelDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.API.Services
{
    public class UserLevelService : IUserLevelService
    {
        private readonly IUserLevelRepository _userLevelRepository;
        private readonly IMapper _mapper;

        public UserLevelService(IUserLevelRepository userLevelRepository, IMapper mapper)
        {
            _userLevelRepository = userLevelRepository;
            _mapper = mapper;
        }

        //public async Task<List<UserLevelDto>> GetAllAsync()
        //{
        //    var userLevels = await _userLevelRepository.GetAllAsync();
        //    return _mapper.Map<List<UserLevelDto>>(userLevels); // AutoMapper
        //}

        public async Task<UserLevelDto?> GetByProfileIdAsync(Guid profileId)
        {
            var userLevel = await _userLevelRepository.GetByProfileIdAsync(profileId);
            return userLevel == null ? null : _mapper.Map<UserLevelDto>(userLevel); // AutoMapper
        }

        public async Task<UserLevelDto> CreateAsync(CreateUserLevelDto dto)
        {
            var userLevel = _mapper.Map<UserLevel>(dto);
            await _userLevelRepository.AddAsync(userLevel);
            await _userLevelRepository.SaveChangesAsync();

            return _mapper.Map<UserLevelDto>(userLevel); // AutoMapper
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateUserLevelDto dto)
        {
            var userLevel = await _userLevelRepository.GetByIdAsync(id);
            if (userLevel == null)
                return false;

            _mapper.Map(dto, userLevel); // AutoMapper
            _userLevelRepository.Update(userLevel);
            await _userLevelRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var userLevel = await _userLevelRepository.GetByIdAsync(id);
            if (userLevel == null)
                return false;

            userLevel.IsDeleted = true;
            userLevel.DeletedAt = DateTime.UtcNow;
            _userLevelRepository.Update(userLevel);
            await _userLevelRepository.SaveChangesAsync();
            return true;
        }
    }
}