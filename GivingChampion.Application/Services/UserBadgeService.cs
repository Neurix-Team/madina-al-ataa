using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.DTO.UserBadge;
using GivingChampion.Application.Services;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GivingChampion.API.Services
{
    public class UserBadgeService : BaseService, IUserBadgeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserBadgeRepository _userBadgeRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public UserBadgeService(
            IUnitOfWork unitOfWork,
            IUserBadgeRepository userBadgeRepository,
            IProfileRepository profileRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userBadgeRepository = userBadgeRepository;
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        public async Task<List<UserBadgeDto>> GetAllByUserIdAsync()
        {
            if (UserId == Guid.Empty)
                throw new BadRequestException("User ID is required.");
            var profile = await _profileRepository.GetByUserIdAsync(UserId);

            if (profile == null)
                throw new NotFoundException($"Profile for user with ID {UserId} was not found.");

            var userBadges = await _userBadgeRepository.GetAllByProfileIdAsync(profile.Id);

            return _mapper.Map<List<UserBadgeDto>>(userBadges);
        }

        public async Task<List<UserBadgeDto>> GetAllByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("User ID is required.");

            var userBadges = await _userBadgeRepository.GetAllByUserIdAsync(userId);

            return _mapper.Map<List<UserBadgeDto>>(userBadges);
        }

        public async Task<UserBadgeDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("UserBadge ID is required.");

            var userBadge = await _userBadgeRepository.GetByIdAsync(id);

            if (userBadge == null)
                throw new NotFoundException($"UserBadge with ID {id} was not found.");

            if (userBadge.IsDeleted)
                throw new NotFoundException($"UserBadge with ID {id} was not found.");

            return _mapper.Map<UserBadgeDto>(userBadge);
        }

        public async Task<UserBadgeDto> CreateAsync(CreateUserBadgeDto dto)
        {
            if (dto == null)
                throw new BadRequestException("UserBadge create data is required.");

            var existingUserBadge = await _userBadgeRepository.GetByProfileAndBadgeAsync(
                dto.ProfileId,
                dto.BadgeId,
                includeDeleted: true);

            if (existingUserBadge != null)
            {
                if (!existingUserBadge.IsDeleted)
                    throw new BadRequestException("This badge is already assigned to this profile.");

                existingUserBadge.IsDeleted = false;
                existingUserBadge.DeletedAt = null;

                _userBadgeRepository.Update(existingUserBadge);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<UserBadgeDto>(existingUserBadge);
            }

            var userBadge = _mapper.Map<UserBadge>(dto);

            await _userBadgeRepository.AddAsync(userBadge);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserBadgeDto>(userBadge);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateUserBadgeDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("UserBadge ID is required.");

            if (dto == null)
                throw new BadRequestException("UserBadge update data is required.");

            var userBadge = await _userBadgeRepository.GetByIdAsync(id);

            if (userBadge == null)
                throw new NotFoundException($"UserBadge with ID {id} was not found.");

            if (userBadge.IsDeleted)
                throw new BadRequestException("Cannot update a deleted UserBadge.");

            _mapper.Map(dto, userBadge);

            _userBadgeRepository.Update(userBadge);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("UserBadge ID is required.");

            var userBadge = await _userBadgeRepository.GetByIdAsync(id);

            if (userBadge == null)
                throw new NotFoundException($"UserBadge with ID {id} was not found.");

            if (userBadge.IsDeleted)
                throw new BadRequestException("UserBadge is already deleted.");

            userBadge.IsDeleted = true;
            userBadge.DeletedAt = DateTime.UtcNow;

            _userBadgeRepository.Update(userBadge);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
