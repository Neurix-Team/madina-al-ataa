using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Interfaces.Location;
using GivingChampion.Common.DTO.Location;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace GivingChampion.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationService> _logger;

        public LocationService(
            ILocationRepository locationRepository,
            IMapper mapper,
            ILogger<LocationService> logger)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<List<LocationDto>>> GetAllAsync()
        {
            var locations = await _locationRepository.GetAllAsync();

            var dtos = _mapper.Map<List<LocationDto>>(locations);

            return Result<List<LocationDto>>.Success(dtos);
        }

        public async Task<Result<List<LocationDto>>> GetAvailableForUserAsync(int userLevel)
        {
            if (userLevel < 0)
                throw new BadRequestException("User level cannot be negative.");

            var locations = await _locationRepository.GetAvailableForLevelAsync(userLevel);

            var dtos = _mapper.Map<List<LocationDto>>(locations);

            return Result<List<LocationDto>>.Success(dtos);
        }

        public async Task<Result<LocationDto>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Location ID is required.");

            var location = await _locationRepository.GetByIdAsync(id);

            if (location == null)
                throw new NotFoundException($"Location with ID {id} was not found.");

            if (location.IsDeleted)
                throw new NotFoundException($"Location with ID {id} was not found.");

            var dto = _mapper.Map<LocationDto>(location);

            return Result<LocationDto>.Success(dto);
        }

        public async Task<Result<LocationDto>> CreateLocationAsync(CreateLocationDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Location data is required.");

            var location = _mapper.Map<Location>(dto);

            await _locationRepository.CreateAsync(location);

            _logger.LogInformation("Location created successfully. LocationId: {LocationId}", location.Id);

            var createdDto = _mapper.Map<LocationDto>(location);

            return Result<LocationDto>.Success(createdDto);
        }

        public async Task<Result> UpdateLocationAsync(Guid id, UpdateLocationDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Location ID is required.");

            if (dto == null)
                throw new BadRequestException("Location update data is required.");

            var location = await _locationRepository.GetByIdAsync(id);

            if (location == null)
                throw new NotFoundException($"Location with ID {id} was not found.");

            if (location.IsDeleted)
                throw new BadRequestException("Cannot update a deleted location.");

            _mapper.Map(dto, location);

            await _locationRepository.UpdateAsync(location);

            _logger.LogInformation("Location updated successfully. LocationId: {LocationId}", id);

            return Result.Success();
        }

        public async Task<Result> SoftDeleteLocationAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Location ID is required.");

            var location = await _locationRepository.GetByIdAsync(id);

            if (location == null)
                throw new NotFoundException($"Location with ID {id} was not found.");

            if (location.IsDeleted)
                throw new BadRequestException("Location is already deleted.");

            await _locationRepository.SoftDeleteAsync(id);

            _logger.LogInformation("Location soft-deleted successfully. LocationId: {LocationId}", id);

            return Result.Success();
        }
    }
}