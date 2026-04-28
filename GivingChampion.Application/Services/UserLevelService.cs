using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.Exceptions;
using GivingChampion.Common.DTO.UserLevelDto;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.API.Services
{
    public class UserLevelService : IUserLevelService
    {
        private readonly IUserLevelRepository _userLevelRepository;
        private readonly IMapper _mapper;

        public UserLevelService(
            IUserLevelRepository userLevelRepository,
            IMapper mapper)
        {
            _userLevelRepository = userLevelRepository;
            _mapper = mapper;
        }

        public async Task<UserLevelDto?> GetByProfileIdAsync(Guid profileId)
        {
            if (profileId == Guid.Empty)
                throw new BadRequestException("Profile ID is required.");

            var userLevel = await _userLevelRepository.GetByProfileIdAsync(profileId);

            if (userLevel == null)
                throw new NotFoundException($"User level for profile ID {profileId} was not found.");

            return _mapper.Map<UserLevelDto>(userLevel);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateUserLevelDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("User level ID is required.");

            if (dto == null)
                throw new BadRequestException("User level update data is required.");

            var userLevel = await _userLevelRepository.GetByIdAsync(id);

            if (userLevel == null)
                throw new NotFoundException($"User level with ID {id} was not found.");

            if (userLevel.IsDeleted)
                throw new BadRequestException("Cannot update a deleted user level.");

            _mapper.Map(dto, userLevel);

            _userLevelRepository.Update(userLevel);

            await _userLevelRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("User level ID is required.");

            var userLevel = await _userLevelRepository.GetByIdAsync(id);

            if (userLevel == null)
                throw new NotFoundException($"User level with ID {id} was not found.");

            if (userLevel.IsDeleted)
                throw new BadRequestException("User level is already deleted.");

            userLevel.IsDeleted = true;
            userLevel.DeletedAt = DateTime.UtcNow;

            _userLevelRepository.Update(userLevel);

            await _userLevelRepository.SaveChangesAsync();

            return true;
        }
    }
}