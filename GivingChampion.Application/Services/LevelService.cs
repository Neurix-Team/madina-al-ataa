using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.LevelDto;
using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Services
{
    public class LevelService : ILevelService
    {
        private readonly ILevelRepository _levelRepository;
        private readonly IMapper _mapper;

        public LevelService(ILevelRepository levelRepository, IMapper mapper)
        {
            _levelRepository = levelRepository;
            _mapper = mapper;
        }

        public async Task<List<LevelDto>> GetAllAsync()
        {
            var levels = await _levelRepository.GetAllAsync();
            return _mapper.Map<List<LevelDto>>(levels); // AutoMapper
        }

        public async Task<LevelDto?> GetByIdAsync(Guid id)
        {
            var level = await _levelRepository.GetByIdAsync(id);
            return level == null ? null : _mapper.Map<LevelDto>(level); // AutoMapper
        }

        public async Task<LevelDto> CreateAsync(CreateLevelDto dto)
        {
            var level = _mapper.Map<Level>(dto);
            await _levelRepository.AddAsync(level);
            await _levelRepository.SaveChangesAsync();

            return _mapper.Map<LevelDto>(level); // AutoMapper
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateLevelDto dto)
        {
            var level = await _levelRepository.GetByIdAsync(id);
            if (level == null)
                return false;

            _mapper.Map(dto, level); // AutoMapper
            _levelRepository.Update(level);
            await _levelRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var level = await _levelRepository.GetByIdAsync(id);
            if (level == null)
                return false;

            level.IsDeleted = true;
            level.DeletedAt = DateTime.UtcNow;
            _levelRepository.Update(level);
            await _levelRepository.SaveChangesAsync();
            return true;
        }
    }
}