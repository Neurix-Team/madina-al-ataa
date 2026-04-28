using AutoMapper;
using GivingChampion.API.Repositories;
using GivingChampion.Application.Interfaces;
using GivingChampion.Common.DTO;
using GivingChampion.Common.DTO.GeoQuestDto;
using GivingChampion.Common.DTO.UserGeoQuestDto;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GivingChampion.Application.Helpers;
namespace GivingChampion.Application.Services
{
    public class UserGeoQuestService : IUserGeoQuestService
    {
        private readonly IUserGeoQuestRepository _userGeoQuestRepository;
        private readonly IGeoQuestRepository _geoQuestRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;

        public UserGeoQuestService(IUserGeoQuestRepository userGeoQuestRepository, IGeoQuestRepository geoQuestRepository, 
            ILocationRepository locationRepository, IMapper mapper)
        {
            _userGeoQuestRepository = userGeoQuestRepository;
            _geoQuestRepository = geoQuestRepository;
            _locationRepository = locationRepository;
            _mapper = mapper;
        }
        public async Task<Result<PagedList<UserGeoQuestDto>>> GetAllAsync(Guid userId, PageParameters pageParameters)
        {
            var userGeoQuests = await _userGeoQuestRepository
                .GetAllByUserIdAsync(userId, pageParameters);

            var userGeoQuestDtos = _mapper.Map<List<UserGeoQuestDto>>(userGeoQuests.Items);

            var pagedResult = new PagedList<UserGeoQuestDto>(
                userGeoQuestDtos,
                userGeoQuests.TotalCount,
                userGeoQuests.PageNumber,
                userGeoQuests.PageSize
            );

            return Result<PagedList<UserGeoQuestDto>>.Success(pagedResult);
        }
        public async Task<Result<UserGeoQuestDto?>> GetByIdAsync(Guid id)
        {
            var userGeoQuest = await _userGeoQuestRepository.GetByIdAsync(id);
            return userGeoQuest == null ? Result<UserGeoQuestDto?>.Failure("UserGeoQuest not found") : Result<UserGeoQuestDto?>.Success(_mapper.Map<UserGeoQuestDto>(userGeoQuest)); // AutoMapper
        }


        public async Task<Result<bool>> UpdateAsync(Guid id, UpdateUserGeoQuestDto dto , bool isSuccess = false)
        {
            var userGeoQuest = await _userGeoQuestRepository.GetByIdAsync(id);
            if (userGeoQuest == null)
                return Result<bool>.Failure("UserGeoQuest not found");

            _mapper.Map(dto, userGeoQuest); // AutoMapper
            _userGeoQuestRepository.Update(userGeoQuest);
            await _userGeoQuestRepository.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        // SOFT DELETE a UserGeoQuest by Id
        public async Task<Result<bool>> SoftDeleteAsync(Guid id)
        {
            var userGeoQuest = await _userGeoQuestRepository.GetByIdAsync(id);
            if (userGeoQuest == null)
                return Result<bool>.Failure("UserGeoQuest not found");

            userGeoQuest.IsDeleted = true;
            userGeoQuest.DeletedAt = DateTime.UtcNow;
            _userGeoQuestRepository.Update(userGeoQuest);
            await _userGeoQuestRepository.SaveChangesAsync();

            return Result<bool>.Success(true);
        }



        public async Task<Result<UserGeoQuestDto>> StartAsync(Guid geoQuestId, Guid userId)
        {
            // Check if GeoQuest exists
            var geoQuest = await _geoQuestRepository.GetByIdAsync(geoQuestId);

            if (geoQuest == null)
                return Result<UserGeoQuestDto>.Failure("GeoQuest not found.");

            // Prevent starting the same GeoQuest twice
            var existingUserGeoQuest = await _userGeoQuestRepository
                .GetByUserIdAndGeoQuestIdAsync(userId, geoQuestId);

            if (existingUserGeoQuest != null)
                return Result<UserGeoQuestDto>.Failure("GeoQuest already started by this user.");

            var userGeoQuest = new UserGeoQuest
            {
                Id = Guid.NewGuid(),

                UserId = userId,
                GeoQuestId = geoQuestId,

                // Required by database
                Title = geoQuest.Title,

                StartedAt = DateTime.UtcNow,
                IsCompleted = false,
                IsLocationVerified = false,

                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _userGeoQuestRepository.AddAsync(userGeoQuest);
            await _userGeoQuestRepository.SaveChangesAsync();

            var dto = _mapper.Map<UserGeoQuestDto>(userGeoQuest);

            return Result<UserGeoQuestDto>.Success(dto);
        }
        // VERIFY User Location
        public async Task<Result<bool>> UpdateAsyncVerification(
     Guid userGeoQuestId,
     VerifyLocationDto dto,
     bool isSuccess = false)
        {
            var userGeoQuest = await _userGeoQuestRepository.GetByIdAsync(userGeoQuestId);

            if (userGeoQuest == null)
                return Result<bool>.Failure("UserGeoQuest not found.");

            var geoQuest = await _geoQuestRepository.GetByIdAsync(userGeoQuest.GeoQuestId);

            if (geoQuest == null)
                return Result<bool>.Failure("GeoQuest not found.");

            if (geoQuest.Location == null)
                return Result<bool>.Failure("GeoQuest location not found.");

            var targetLatitude = geoQuest.Location.Latitude;
            var targetLongitude = geoQuest.Location.Longitude;

            var isLocationValid = LocationVerifier.IsWithinDistance(
                userLatitude: dto.Latitude,
                userLongitude: dto.Longitude,
                targetLatitude: double.Parse(targetLatitude),
                targetLongitude: double.Parse(targetLongitude),
                thresholdMeters: 50
            );

            if (!isLocationValid)
                return Result<bool>.Failure("Location verification failed.");

            userGeoQuest.IsLocationVerified = true;

            _userGeoQuestRepository.Update(userGeoQuest);
            await _userGeoQuestRepository.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
        public async Task<Result<UserGeoQuestDto>> CheckGeoQuestStatus(Guid id)
        {
            // Get the UserGeoQuest by its ID
            var userGeoQuest = await _userGeoQuestRepository.GetByIdAsync(id);

            if (userGeoQuest == null)
                return Result<UserGeoQuestDto>.Failure("UserGeoQuest not found");

            // Check if the UserGeoQuest has a valid GeoQuest
            if (userGeoQuest.GeoQuest == null)
            {
                return Result<UserGeoQuestDto>.Failure("No GeoQuest associated with this UserGeoQuest");
            }

            // Determine the status of the GeoQuest
            string status = "Not Started";

            if (userGeoQuest.IsCompleted)
            {
                status = "Completed";
            }
            else if (userGeoQuest.StartedAt != null)
            {
                status = "In Progress";
            }

            // Map the result to a UserGeoQuestDto and return the result with status
            var userGeoQuestDto = _mapper.Map<UserGeoQuestDto>(userGeoQuest);
            userGeoQuestDto.Status = status; // Add status to the DTO

            return Result<UserGeoQuestDto>.Success(userGeoQuestDto);
        }

      
    }
}