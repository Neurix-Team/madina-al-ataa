using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.Exceptions;
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

        public LevelService(
            ILevelRepository levelRepository,
            IMapper mapper)
        {
            _levelRepository = levelRepository;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<LevelDto>>> GetAllAsync(PageParameters pageParameters)
        {
            var levels = await _levelRepository.GetAllAsync(pageParameters);

            var levelDtos = _mapper.MapPagedList<Level, LevelDto>(levels);

            return Result<PagedList<LevelDto>>.Success(levelDtos);
        }

        public async Task<Result<LevelDto?>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Level ID is required.");

            var level = await _levelRepository.GetByIdAsync(id);

            if (level == null)
                throw new NotFoundException($"Level with ID {id} was not found.");

            if (level.IsDeleted)
                throw new NotFoundException($"Level with ID {id} was not found.");

            var levelDto = _mapper.Map<LevelDto>(level);

            return Result<LevelDto?>.Success(levelDto);
        }

        public async Task<Result<LevelDto>> CreateAsync(CreateLevelDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Level data is required.");

            var level = _mapper.Map<Level>(dto);

            if (level.Number <= 0)
                throw new BadRequestException("Level number must be greater than zero.");

            if (level.MaxXp < 0)
                throw new BadRequestException("Max XP cannot be negative.");

            await _levelRepository.AddAsync(level);

            await _levelRepository.SaveChangesAsync();

            var levelDto = _mapper.Map<LevelDto>(level);

            return Result<LevelDto>.Success(levelDto);
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, UpdateLevelDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Level ID is required.");

            if (dto == null)
                throw new BadRequestException("Level update data is required.");

            var level = await _levelRepository.GetByIdAsync(id);

            if (level == null)
                throw new NotFoundException($"Level with ID {id} was not found.");

            if (level.IsDeleted)
                throw new BadRequestException("Cannot update a deleted level.");

            _mapper.Map(dto, level);

            if (level.Number <= 0)
                throw new BadRequestException("Level number must be greater than zero.");

            if (level.MaxXp < 0)
                throw new BadRequestException("Max XP cannot be negative.");

            _levelRepository.Update(level);

            await _levelRepository.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> SoftDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Level ID is required.");

            var level = await _levelRepository.GetByIdAsync(id);

            if (level == null)
                throw new NotFoundException($"Level with ID {id} was not found.");

            if (level.IsDeleted)
                throw new BadRequestException("Level is already deleted.");

            level.IsDeleted = true;
            level.DeletedAt = DateTime.UtcNow;

            _levelRepository.Update(level);

            await _levelRepository.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}