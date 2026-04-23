using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.LevelDto;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

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

        public async Task<Result<PagedList<LevelDto>>> GetAllAsync(PageParameters pageParameters)
        {
            var levels = await _levelRepository.GetAllAsync(pageParameters);
            return Result<PagedList<LevelDto>>.Success(_mapper.MapPagedList<Level, LevelDto>(levels)); // AutoMapper
        }

        public async Task<Result<LevelDto?>> GetByIdAsync(Guid id)
        {
            var level = await _levelRepository.GetByIdAsync(id);
            return level == null ? Result<LevelDto?>.Failure("Level not found") : Result<LevelDto?>.Success(_mapper.Map<LevelDto>(level)); // AutoMapper
        }

        public async Task<Result<LevelDto>> CreateAsync(CreateLevelDto dto)
        {
            var level = _mapper.Map<Level>(dto);
            await _levelRepository.AddAsync(level);
            await _levelRepository.SaveChangesAsync();

            return Result<LevelDto>.Success(_mapper.Map<LevelDto>(level)); // AutoMapper
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, UpdateLevelDto dto)
        {
            var level = await _levelRepository.GetByIdAsync(id);
            if (level == null)
                return Result<bool>.Failure("Level not found");
            _mapper.Map(dto, level); // AutoMapper
            _levelRepository.Update(level);
            await _levelRepository.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> SoftDeleteAsync(Guid id)
        {
            var level = await _levelRepository.GetByIdAsync(id);
            if (level == null)
                return Result<bool>.Failure("Level not found");
            level.IsDeleted = true;
            level.DeletedAt = DateTime.UtcNow;
            _levelRepository.Update(level);
            await _levelRepository.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}