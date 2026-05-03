using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.ProfileDto;
using GivingChampion.Application.Services;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;
using DomainProfile = GivingChampion.Domain.Entities.Profile;

namespace GivingChampion.API.Services
{
    public class ProfileService : BaseService, IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<DomainProfile> _profileRepository;
        private readonly IGenericRepository<Avatar> _avatarRepository;
        private readonly IGenericRepository<UserLevel> _userLevelRepository;
        private readonly IMapper _mapper;

        public ProfileService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _profileRepository = unitOfWork.Repository<DomainProfile>();
            _avatarRepository = unitOfWork.Repository<Avatar>();

            _userLevelRepository = unitOfWork.Repository<UserLevel>();
            _mapper = mapper;
        }

        public async Task<Result<ProfileDto?>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Profile ID is required.");

            var profile = await _profileRepository.GetByIdAsync(id);

            if (profile == null)
                throw new NotFoundException($"Profile with ID {id} was not found.");

            if (profile.IsDeleted)
                throw new NotFoundException($"Profile with ID {id} was not found.");

            var profileDto = _mapper.Map<ProfileDto>(profile);
            var userLevel = await _userLevelRepository.Query()
              .Include(ul => ul.Level)
              .FirstOrDefaultAsync(ul => ul.ProfileId == profile.Id);

            if (userLevel != null)
            {
                profileDto.LevelId = userLevel.LevelId;
                profileDto.LevelNumber = userLevel.Level?.Number;
            }

            var avatar = await _avatarRepository.FirstOrDefaultAsync(a => a.ProfileId == profile.Id);

            if (avatar != null)
            {
                profileDto.AvatarId = avatar.Id;
                profileDto.AvatarName = avatar.CharacterName;
            }

            return Result<ProfileDto?>.Success(profileDto);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateProfileDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Profile ID is required.");

            if (dto == null)
                throw new BadRequestException("Profile update data is required.");

            var profile = await _profileRepository.GetByIdAsync(id);

            if (profile == null)
                throw new NotFoundException($"Profile with ID {id} was not found.");

            if (profile.IsDeleted)
                throw new BadRequestException("Cannot update a deleted profile.");

            _mapper.Map(dto, profile);

            _profileRepository.Update(profile);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Profile ID is required.");

            var profile = await _profileRepository.GetByIdAsync(id);

            if (profile == null)
                throw new NotFoundException($"Profile with ID {id} was not found.");

            if (profile.IsDeleted)
                throw new BadRequestException("Profile is already deleted.");

            profile.IsDeleted = true;
            profile.DeletedAt = DateTime.UtcNow;

            _profileRepository.Update(profile);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<Result<ProfileDto?>> GetByUserIdAsync()
        {
            var id = UserId;

            var profile = await _profileRepository.FirstOrDefaultAsync(p => p.UserId == id);

            if (profile == null)
                throw new NotFoundException($"Profile with User ID {id} was not found.");

            if (profile.IsDeleted)
                throw new NotFoundException($"Profile with User ID {id} was not found.");

            var profileDto = _mapper.Map<ProfileDto>(profile);
            var userLevel = await _userLevelRepository.Query()
               .Include(ul => ul.Level)
              .FirstOrDefaultAsync(ul => ul.ProfileId == profile.Id);

            if (userLevel != null)
            {
                profileDto.LevelId = userLevel.LevelId;
                profileDto.LevelNumber = userLevel.Level?.Number;
            }

            var avatar = await _avatarRepository.FirstOrDefaultAsync(a => a.ProfileId == profile.Id);

            if (avatar != null)
            {
                profileDto.AvatarId = avatar.Id;
                profileDto.AvatarName = avatar.CharacterName;
            }

            return Result<ProfileDto?>.Success(profileDto);
        }
    }
}