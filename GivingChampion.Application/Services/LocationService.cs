using AutoMapper;
using GivingChampion.Application.Interfaces.Location;
using GivingChampion.Application.Interfaces;
using GivingChampion.Common.DTO.Location;
using GivingChampion.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var locations = await _locationRepository.GetAvailableForLevelAsync(userLevel);
            var dtos = _mapper.Map<List<LocationDto>>(locations);
            return Result<List<LocationDto>>.Success(dtos);
        }

        public async Task<Result<LocationDto>> GetByIdAsync(Guid id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
                return Result<LocationDto>.Failure("Location not found");

            var dto = _mapper.Map<LocationDto>(location);
            return Result<LocationDto>.Success(dto);
        }

        public async Task<Result<LocationDto>> CreateLocationAsync(CreateLocationDto dto)
        {
            var location = _mapper.Map<Location>(dto);
            await _locationRepository.CreateAsync(location);

            var createdDto = _mapper.Map<LocationDto>(location);
            return Result<LocationDto>.Success(createdDto);
        }

        public async Task<Result> UpdateLocationAsync(Guid id, UpdateLocationDto dto)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
                return Result.Failure("Location not found");

            _mapper.Map(dto, location);
            await _locationRepository.UpdateAsync(location);

            return Result.Success();
        }

        public async Task<Result> SoftDeleteLocationAsync(Guid id)
        {
            await _locationRepository.SoftDeleteAsync(id);
            return Result.Success();
        }
    }
}