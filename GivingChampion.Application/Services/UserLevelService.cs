using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.UserLevelDto;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Services;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;
using DomainProfile = GivingChampion.Domain.Entities.Profile;

namespace GivingChampion.API.Services
{
    public class UserLevelService : BaseService, IUserLevelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserLevel> _userLevelRepository;
        private readonly IGenericRepository<DomainProfile> _profileRepository;
        private readonly IMapper _mapper;

        public UserLevelService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userLevelRepository = unitOfWork.Repository<UserLevel>();
            _profileRepository = unitOfWork.Repository<DomainProfile>();
            _mapper = mapper;
        }

        public async Task<UserLevelDto?> GetMyLevelAsync()
        {
            if (UserId == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid user token.");

            var profile = await _profileRepository.FirstOrDefaultAsync(
                p => p.UserId == UserId
            );

            if (profile == null)
                throw new NotFoundException($"Profile with User ID {UserId} was not found.");

            if (profile.IsDeleted)
                throw new NotFoundException($"Profile with User ID {UserId} was not found.");

            var userLevel = await _userLevelRepository.FirstOrDefaultAsync(
                level => level.ProfileId == profile.Id
            );

            if (userLevel == null)
                throw new NotFoundException($"User level for profile ID {profile.Id} was not found.");

            if (userLevel.IsDeleted)
                throw new NotFoundException($"User level for profile ID {profile.Id} was not found.");

            return _mapper.Map<UserLevelDto>(userLevel);
        }

        public async Task<UserLevelDto?> GetByProfileIdAsync(Guid profileId)
        {
            if (profileId == Guid.Empty)
                throw new BadRequestException("Profile ID is required.");

            var userLevel = await _userLevelRepository.FirstOrDefaultAsync(
                level => level.ProfileId == profileId
            );

            if (userLevel == null)
                throw new NotFoundException($"User level for profile ID {profileId} was not found.");

            if (userLevel.IsDeleted)
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