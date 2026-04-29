using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Helpers;
using GivingChampion.Application.Interfaces;
using GivingChampion.Common.DTO;
using GivingChampion.Common.DTO.GeoQuestDto;
using GivingChampion.Common.DTO.UserGeoQuestDto;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services
{
    public class UserGeoQuestService : IUserGeoQuestService
    {
        private readonly IUserGeoQuestRepository _userGeoQuestRepository;
        private readonly IGeoQuestRepository _geoQuestRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;

        public UserGeoQuestService(
            IUserGeoQuestRepository userGeoQuestRepository,
            IGeoQuestRepository geoQuestRepository,
            ILocationRepository locationRepository,
            IMapper mapper)
        {
            _userGeoQuestRepository = userGeoQuestRepository;
            _geoQuestRepository = geoQuestRepository;
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<UserGeoQuestDto>>> GetAllAsync(
            Guid userId,
            PageParameters pageParameters)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("User ID is required.");

            if (pageParameters == null)
                throw new BadRequestException("Page parameters are required.");

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
            if (id == Guid.Empty)
                throw new BadRequestException("UserGeoQuest ID is required.");

            var userGeoQuest = await _userGeoQuestRepository.GetByIdAsync(id);

            if (userGeoQuest == null)
                throw new NotFoundException($"UserGeoQuest with ID {id} was not found.");

            if (userGeoQuest.IsDeleted)
                throw new NotFoundException($"UserGeoQuest with ID {id} was not found.");

            var userGeoQuestDto = _mapper.Map<UserGeoQuestDto>(userGeoQuest);

            return Result<UserGeoQuestDto?>.Success(userGeoQuestDto);
        }

        public async Task<Result<bool>> UpdateAsync(
            Guid id,
            UpdateUserGeoQuestDto dto,
            bool isSuccess = false)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("UserGeoQuest ID is required.");

            if (dto == null)
                throw new BadRequestException("UserGeoQuest update data is required.");

            var userGeoQuest = await _userGeoQuestRepository.GetByIdAsync(id);

            if (userGeoQuest == null)
                throw new NotFoundException($"UserGeoQuest with ID {id} was not found.");

            if (userGeoQuest.IsDeleted)
                throw new BadRequestException("Cannot update a deleted UserGeoQuest.");

            _mapper.Map(dto, userGeoQuest);

            _userGeoQuestRepository.Update(userGeoQuest);

            await _userGeoQuestRepository.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> SoftDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("UserGeoQuest ID is required.");

            var userGeoQuest = await _userGeoQuestRepository.GetByIdAsync(id);

            if (userGeoQuest == null)
                throw new NotFoundException($"UserGeoQuest with ID {id} was not found.");

            if (userGeoQuest.IsDeleted)
                throw new BadRequestException("UserGeoQuest is already deleted.");

            userGeoQuest.IsDeleted = true;
            userGeoQuest.DeletedAt = DateTime.UtcNow;

            _userGeoQuestRepository.Update(userGeoQuest);

            await _userGeoQuestRepository.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<UserGeoQuestDto>> StartAsync(
            Guid geoQuestId,
            Guid userId)
        {
            if (geoQuestId == Guid.Empty)
                throw new BadRequestException("GeoQuest ID is required.");

            if (userId == Guid.Empty)
                throw new BadRequestException("User ID is required.");

            var geoQuest = await _geoQuestRepository.GetByIdAsync(geoQuestId);

            if (geoQuest == null)
                throw new NotFoundException($"GeoQuest with ID {geoQuestId} was not found.");

            if (geoQuest.IsDeleted)
                throw new NotFoundException($"GeoQuest with ID {geoQuestId} was not found.");

            var existingUserGeoQuest = await _userGeoQuestRepository
                .GetByUserIdAndGeoQuestIdAsync(userId, geoQuestId);

            if (existingUserGeoQuest != null && !existingUserGeoQuest.IsDeleted)
                throw new ConflictException("GeoQuest already started by this user.");

            var userGeoQuest = new UserGeoQuest
            {
                Id = Guid.NewGuid(),

                UserId = userId,
                GeoQuestId = geoQuestId,

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

        public async Task<Result<bool>> UpdateAsyncVerification(
            Guid userGeoQuestId,
            VerifyLocationDto dto,
            bool isSuccess = false)
        {
            if (userGeoQuestId == Guid.Empty)
                throw new BadRequestException("UserGeoQuest ID is required.");

            if (dto == null)
                throw new BadRequestException("Location verification data is required.");

            var userGeoQuest = await _userGeoQuestRepository.GetByIdAsync(userGeoQuestId);

            if (userGeoQuest == null)
                throw new NotFoundException($"UserGeoQuest with ID {userGeoQuestId} was not found.");

            if (userGeoQuest.IsDeleted)
                throw new BadRequestException("Cannot verify location for a deleted UserGeoQuest.");

            var geoQuest = await _geoQuestRepository.GetByIdAsync(userGeoQuest.GeoQuestId);

            if (geoQuest == null)
                throw new NotFoundException($"GeoQuest with ID {userGeoQuest.GeoQuestId} was not found.");

            if (geoQuest.IsDeleted)
                throw new NotFoundException($"GeoQuest with ID {userGeoQuest.GeoQuestId} was not found.");

            if (geoQuest.Location == null)
                throw new NotFoundException("GeoQuest location was not found.");

            var targetLatitude = geoQuest.Location.Latitude;
            var targetLongitude = geoQuest.Location.Longitude;

            if (!double.TryParse(targetLatitude, out var parsedTargetLatitude))
                throw new BadRequestException("GeoQuest target latitude is invalid.");

            if (!double.TryParse(targetLongitude, out var parsedTargetLongitude))
                throw new BadRequestException("GeoQuest target longitude is invalid.");

            var isLocationValid = LocationVerifier.IsWithinDistance(
                userLatitude: dto.Latitude,
                userLongitude: dto.Longitude,
                targetLatitude: parsedTargetLatitude,
                targetLongitude: parsedTargetLongitude,
                thresholdMeters: 50
            );

            if (!isLocationValid)
                throw new BadRequestException("Location verification failed.");

            userGeoQuest.IsLocationVerified = true;

            _userGeoQuestRepository.Update(userGeoQuest);

            await _userGeoQuestRepository.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<UserGeoQuestDto>> CheckGeoQuestStatus(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("UserGeoQuest ID is required.");

            var userGeoQuest = await _userGeoQuestRepository.GetByIdAsync(id);

            if (userGeoQuest == null)
                throw new NotFoundException($"UserGeoQuest with ID {id} was not found.");

            if (userGeoQuest.IsDeleted)
                throw new NotFoundException($"UserGeoQuest with ID {id} was not found.");

            if (userGeoQuest.GeoQuest == null)
                throw new NotFoundException("No GeoQuest associated with this UserGeoQuest.");

            string status = "Not Started";

            if (userGeoQuest.IsCompleted)
            {
                status = "Completed";
            }
            else if (userGeoQuest.StartedAt != null)
            {
                status = "In Progress";
            }

            var userGeoQuestDto = _mapper.Map<UserGeoQuestDto>(userGeoQuest);

            userGeoQuestDto.Status = status;

            return Result<UserGeoQuestDto>.Success(userGeoQuestDto);
        }
    }
}