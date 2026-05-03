using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.UserLevelDto;
using GivingChampion.Application.Exceptions;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.API.Services
{
    public class UserLevelService : IUserLevelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserLevelRepository _userLevelRepository;
        private readonly IMapper _mapper;

        public UserLevelService(
            IUnitOfWork unitOfWork,
            IUserLevelRepository userLevelRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
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

            _mapper.Map(dto, userLevel);

            _userLevelRepository.Update(userLevel);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("User level ID is required.");

            var userLevel = await _userLevelRepository.GetByIdAsync(id);

            if (userLevel == null)
                throw new NotFoundException($"User level with ID {id} was not found.");

            userLevel.IsDeleted = true;
            userLevel.DeletedAt = DateTime.UtcNow;

            _userLevelRepository.Update(userLevel);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}