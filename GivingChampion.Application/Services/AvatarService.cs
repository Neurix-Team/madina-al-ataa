using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.AvatarDto;
using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Services
{
    public class AvatarService : IAvatarService
    {
        private readonly IAvatarRepository _avatarRepository;
        private readonly IMapper _mapper;

        public AvatarService(IAvatarRepository avatarRepository, IMapper mapper)
        {
            _avatarRepository = avatarRepository;
            _mapper = mapper;
        }

        public async Task<List<AvatarDto>> GetAllAsync()
        {
            var avatars = await _avatarRepository.GetAllAsync();
            return _mapper.Map<List<AvatarDto>>(avatars); // AutoMapper
        }

        public async Task<AvatarDto?> GetByIdAsync(Guid id)
        {
            var avatar = await _avatarRepository.GetByIdAsync(id);
            return avatar == null ? null : _mapper.Map<AvatarDto>(avatar); // AutoMapper
        }

        public async Task<AvatarDto> CreateAsync(CreateAvatarDto dto)
        {
            var avatar = _mapper.Map<Avatar>(dto);
            await _avatarRepository.AddAsync(avatar);
            await _avatarRepository.SaveChangesAsync();

            return _mapper.Map<AvatarDto>(avatar); // AutoMapper
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateAvatarDto dto)
        {
            var avatar = await _avatarRepository.GetByIdAsync(id);
            if (avatar == null)
                return false;

            _mapper.Map(dto, avatar); // AutoMapper
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