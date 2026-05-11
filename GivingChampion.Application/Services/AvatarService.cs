using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.AvatarDto;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Services;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GivingChampion.API.Services
{
    public class AvatarService : BaseService, IAvatarService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Avatar> _avatarRepository;
        private readonly IGenericRepository<GivingChampion.Domain.Entities.Profile> _profileRepository;
        private readonly IMapper _mapper;

        public AvatarService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _avatarRepository = unitOfWork.Repository<Avatar>();
            _profileRepository = unitOfWork.Repository<GivingChampion.Domain.Entities.Profile>();
            _mapper = mapper;
        }

        public async Task<AvatarDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Avatar ID is required.");

            var avatar = await _avatarRepository.GetByIdAsync(id);

            if (avatar == null || avatar.IsDeleted)
                throw new NotFoundException($"Avatar with ID {id} was not found.");

            var profile = await GetCurrentProfileAsync();
            EnsureAvatarBelongsToProfile(avatar, profile.Id, "view");

            return _mapper.Map<AvatarDto>(avatar);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateAvatarDto dto)
        {

            if (id == Guid.Empty)
                throw new BadRequestException("Avatar ID is required.");

            if (dto == null)
                throw new BadRequestException("Avatar update data is required.");

            NormalizeAndValidate(dto);

            var avatar = await _avatarRepository.GetByIdAsync(id);

            if (avatar == null)
                throw new NotFoundException($"Avatar with ID {id} was not found.");

            if (avatar.IsDeleted)
                throw new BadRequestException("Cannot update a deleted avatar.");

            var profile = await GetCurrentProfileAsync();
            EnsureAvatarBelongsToProfile(avatar, profile.Id, "update");

            _mapper.Map(dto, avatar);

            avatar.UpdatedAt = DateTime.UtcNow;

            _avatarRepository.Update(avatar);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {

            if (id == Guid.Empty)
                throw new BadRequestException("Avatar ID is required.");

            var avatar = await _avatarRepository.GetByIdAsync(id);

            if (avatar == null)
                throw new NotFoundException($"Avatar with ID {id} was not found.");

            if (avatar.IsDeleted)
                throw new BadRequestException("Avatar is already deleted.");

            var profile = await GetCurrentProfileAsync();
            EnsureAvatarBelongsToProfile(avatar, profile.Id, "delete");

            avatar.IsDeleted = true;
            avatar.DeletedAt = DateTime.UtcNow;
            avatar.UpdatedAt = DateTime.UtcNow;

            _avatarRepository.Update(avatar);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task<GivingChampion.Domain.Entities.Profile> GetCurrentProfileAsync()
        {
            var profile = await _profileRepository.FirstOrDefaultAsync(p =>
                p.UserId == UserId &&
                !p.IsDeleted);

            if (profile == null)
                throw new NotFoundException("Profile was not found.");

            return profile;
        }

        private static void EnsureAvatarBelongsToProfile(Avatar avatar, Guid profileId, string action)
        {
            if (avatar.ProfileId != profileId)
                throw new ForbiddenException($"You are not allowed to {action} another user's avatar.");
        }

        private static void NormalizeAndValidate(UpdateAvatarDto dto)
        {
            dto.SkinColor = RequireText(dto.SkinColor, nameof(dto.SkinColor), 50);
            dto.HairColor = RequireText(dto.HairColor, nameof(dto.HairColor), 50);
            dto.HairStyle = RequireText(dto.HairStyle, nameof(dto.HairStyle), 50);
            dto.ClothesColor = RequireText(dto.ClothesColor, nameof(dto.ClothesColor), 50);
            dto.CharacterName = RequireText(dto.CharacterName, nameof(dto.CharacterName), 100);
        }

        private static string RequireText(string? value, string fieldName, int maxLength)
        {
            var normalized = value?.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
                throw new BadRequestException($"{fieldName} is required.");

            if (normalized.Length > maxLength)
                throw new BadRequestException($"{fieldName} cannot exceed {maxLength} characters.");

            return normalized;
        }
    }
}
