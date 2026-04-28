using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.Exceptions;
using GivingChampion.Common.DTO.ProfileDto;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.API.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public ProfileService(
            IProfileRepository profileRepository,
            IMapper mapper)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        public async Task<ProfileDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Profile ID is required.");

            var profile = await _profileRepository.GetByIdAsync(id);

            if (profile == null)
                throw new NotFoundException($"Profile with ID {id} was not found.");

            if (profile.IsDeleted)
                throw new NotFoundException($"Profile with ID {id} was not found.");

            return _mapper.Map<ProfileDto>(profile);
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

            await _profileRepository.SaveChangesAsync();

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

            await _profileRepository.SaveChangesAsync();

            return true;
        }
    }
}