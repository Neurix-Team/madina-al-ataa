using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.ProfileDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.API.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public ProfileService(IProfileRepository profileRepository, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        //public async Task<List<ProfileDto>> GetAllAsync()
        //{
        //    var profiles = await _profileRepository.GetAllAsync();
        //    return _mapper.Map<List<ProfileDto>>(profiles);
        //}

        public async Task<ProfileDto?> GetByIdAsync(Guid id)
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            return profile == null ? null : _mapper.Map<ProfileDto>(profile);
        }

        public async Task<ProfileDto> CreateAsync(CreateProfileDto dto)
        {
            var profile = _mapper.Map<Domain.Entities.Profile>(dto);
            await _profileRepository.AddAsync(profile);
            await _profileRepository.SaveChangesAsync();
            return _mapper.Map<ProfileDto>(profile);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateProfileDto dto)
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            if (profile == null)
                return false;

            _mapper.Map(dto, profile);  // Map updated properties
            _profileRepository.Update(profile);
            await _profileRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            if (profile == null)
                return false;

            profile.IsDeleted = true;
            profile.DeletedAt = DateTime.UtcNow;

            _profileRepository.Update(profile);
            await _profileRepository.SaveChangesAsync();

            return true;
        }
    }
}