using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.AvatarDto;
using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Services
{
    public class AvatarService : IAvatarService
    {
        private readonly IAvatarRepository _avatarRepository;

        public AvatarService(IAvatarRepository avatarRepository)
        {
            _avatarRepository = avatarRepository;
        }

        public async Task<List<AvatarDto>> GetAllAsync()
        {
            var avatars = await _avatarRepository.GetAllAsync();

            return avatars.Select(a => new AvatarDto
            {
                Id = a.Id,
                Gender = a.Gender,
                SkinColor = a.SkinColor,
                HairColor = a.HairColor,
                HairStyle = a.HairStyle,
                ClothesColor = a.ClothesColor,
                CharacterName = a.CharacterName
            }).ToList();
        }

        public async Task<AvatarDto?> GetByIdAsync(Guid id)
        {
            var avatar = await _avatarRepository.GetByIdAsync(id);

            if (avatar == null)
                return null;

            return new AvatarDto
            {
                Id = avatar.Id,
                Gender = avatar.Gender,
                SkinColor = avatar.SkinColor,
                HairColor = avatar.HairColor,
                HairStyle = avatar.HairStyle,
                ClothesColor = avatar.ClothesColor,
                CharacterName = avatar.CharacterName
            };
        }

        public async Task<AvatarDto> CreateAsync(CreateAvatarDto dto)
        {
            var avatar = new Avatar
            {
                Gender = dto.Gender,
                SkinColor = dto.SkinColor,
                HairColor = dto.HairColor,
                HairStyle = dto.HairStyle,
                ClothesColor = dto.ClothesColor,
                CharacterName = dto.CharacterName
            };

            await _avatarRepository.AddAsync(avatar);
            await _avatarRepository.SaveChangesAsync();

            return new AvatarDto
            {
                Id = avatar.Id,
                Gender = avatar.Gender,
                SkinColor = avatar.SkinColor,
                HairColor = avatar.HairColor,
                HairStyle = avatar.HairStyle,
                ClothesColor = avatar.ClothesColor,
                CharacterName = avatar.CharacterName
            };
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateAvatarDto dto)
        {
            var avatar = await _avatarRepository.GetByIdAsync(id);

            if (avatar == null)
                return false;

            avatar.Gender = dto.Gender;
            avatar.SkinColor = dto.SkinColor;
            avatar.HairColor = dto.HairColor;
            avatar.HairStyle = dto.HairStyle;
            avatar.ClothesColor = dto.ClothesColor;
            avatar.CharacterName = dto.CharacterName;

            _avatarRepository.Update(avatar);
            await _avatarRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var avatar = await _avatarRepository.GetByIdAsync(id);

            if (avatar == null)
                return false;

            avatar.IsDeleted = true;
            avatar.DeletedAt = DateTime.UtcNow;

            _avatarRepository.Update(avatar);
            await _avatarRepository.SaveChangesAsync();

            return true;
        }
    }
}