using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.Interfaces;
using GivingChampion.Common.DTO;
using GivingChampion.Common.DTO.GeoQuestDto;
using GivingChampion.Common.DTO.UserGeoQuestDto;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingChampion.Application.Services
{
    public class UserGeoQuestService : IUserGeoQuestService
    {
        private readonly IUserGeoQuestRepository _userGeoQuestRepository;
        private readonly IMapper _mapper;

        public UserGeoQuestService(IUserGeoQuestRepository userGeoQuestRepository, IMapper mapper)
        {
            _userGeoQuestRepository = userGeoQuestRepository;
            _mapper = mapper;
        }

        // GET all UserGeoQuests with pagination
        //public async Task<Result<PagedList<UserGeoQuestDto>>> GetAllAsync(PageParameters pageParameters)
        //{
        //    var userGeoQuests = await _userGeoQuestRepository.GetAllAsync(pageParameters);
        //    var userGeoQuestDtos = _mapper.Map<PagedList<UserGeoQuestDto>>(userGeoQuests); // AutoMapper
        //    return Result<PagedList<UserGeoQuestDto>>.Success(userGeoQuestDtos);
        //}

        // GET a single UserGeoQuest by Id
        public async Task<Result<UserGeoQuestDto?>> GetByIdAsync(Guid id)
        {
            var userGeoQuest = await _userGeoQuestRepository.GetByIdAsync(id);
            return userGeoQuest == null ? Result<UserGeoQuestDto?>.Failure("UserGeoQuest not found") : Result<UserGeoQuestDto?>.Success(_mapper.Map<UserGeoQuestDto>(userGeoQuest)); // AutoMapper
        }

        // CREATE a new UserGeoQuest
        //public async Task<Result<UserGeoQuestDto>> CreateAsync(CreateUserGeoQuestDto dto)
        //{
        //    var userGeoQuest = _mapper.Map<UserGeoQuest>(dto);
        //    await _userGeoQuestRepository.AddAsync(userGeoQuest);
        //    await _userGeoQuestRepository.SaveChangesAsync();

        //    return Result<UserGeoQuestDto>.Success(_mapper.Map<UserGeoQuestDto>(userGeoQuest)); // AutoMapper
        //}

        // UPDATE an existing UserGeoQuest by Id
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



        // START Geo Quest (Mark as started)
        public async Task<Result<bool>> StartAsync(Guid id, StartGeoQuestDto dto)
        {
            var userGeoQuest = await _userGeoQuestRepository.GetByIdAsync(id);

            if (userGeoQuest == null)
                return Result<bool>.Failure("UserGeoQuest not found");

            // Mark the GeoQuest as started
            userGeoQuest.StartedAt = DateTime.UtcNow; // Set start time
            userGeoQuest.IsStarted = true; // Add this flag in the model if needed
            _userGeoQuestRepository.Update(userGeoQuest);

            await _userGeoQuestRepository.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        // VERIFY User Location

        public async Task<Result<bool>> UpdateAsyncVerification(Guid id, VerifyLocationDto dto, bool isSuccess = false)
        {
            var userGeoQuest = await _userGeoQuestRepository.GetByIdAsync(id);

            if (userGeoQuest == null)
                return Result<bool>.Failure("UserGeoQuest not found");

            // Verify the location of the GeoQuest
            if (userGeoQuest.GeoQuest.LocationLatitude == dto.Latitude &&
                userGeoQuest.GeoQuest.LocationLongitude == dto.Longitude)
            {
                // Mark the UserGeoQuest as location verified
                userGeoQuest.IsLocationVerified = true;
                _userGeoQuestRepository.Update(userGeoQuest);
                await _userGeoQuestRepository.SaveChangesAsync();
                return Result<bool>.Success(true);
            }

            return Result<bool>.Failure("Location verification failed");
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