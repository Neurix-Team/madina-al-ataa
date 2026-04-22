using AutoMapper;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Interfaces.Mission;
using GivingChampion.Common.DTO.Mission;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingChampion.Application.Services
{
    public class UserMissionService : IUserMissionService
    {
        private readonly IUserMissionRepository _userMissionRepository;
        private readonly IMissionRepository _missionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserMissionService> _logger;

        public UserMissionService(
            IUserMissionRepository userMissionRepository,
            IMissionRepository missionRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<UserMissionService> logger)
        {
            _userMissionRepository = userMissionRepository;
            _missionRepository = missionRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Starts a new mission for the user
        /// </summary>
        public async Task<Result<UserMissionDto>> StartMissionAsync(StartMissionDto dto, Guid userId)
        {
            // Validate mission exists and is active
            var mission = await _missionRepository.GetByIdAsync(dto.MissionId);
            if (mission == null)
                return Result<UserMissionDto>.Failure("Mission not found.");

            if (mission.Status != MissionStatus.InProgress)
                return Result<UserMissionDto>.Failure("This mission is not currently in progress.");

            // Check if user already started this mission
            if (await _userMissionRepository.IsMissionStartedAsync(userId, dto.MissionId))
                return Result<UserMissionDto>.Failure("You have already started this mission.");

            // Check if user meets level requirement
            // (You can enhance this by getting user level from User entity or a separate UserLevel service)
            if (mission.RequiredLevel > 0)
            {
                // TODO: Add level check if you have user level tracking
            }

            var userMission = new UserMission
            {
                UserId = userId,
                MissionId = dto.MissionId,
                Progress = 0,
                Status = MissionStatus.InProgress,
                StartedAt = DateTime.UtcNow
            };

            await _userMissionRepository.CreateAsync(userMission);

            var resultDto = _mapper.Map<UserMissionDto>(userMission);
            resultDto.MissionTitle = mission.Title;
            resultDto.KPReward = mission.KPReward;
            resultDto.XPReward = mission.XPReward;

            _logger.LogInformation("User {UserId} started mission '{MissionTitle}' (ID: {MissionId})",
                userId, mission.Title, dto.MissionId);

            return Result<UserMissionDto>.Success(resultDto);
        }

        /// <summary>
        /// Updates progress on an active user mission
        /// </summary>
        public async Task<Result<UserMissionDto>> UpdateProgressAsync(Guid userMissionId, UpdateProgressDto dto, Guid userId)
        {
            var userMission = await _userMissionRepository.GetByIdAsync(userMissionId);
            if (userMission == null)
                return Result<UserMissionDto>.Failure("User mission not found.");

            // Security: Only the owner can update progress
            if (userMission.UserId != userId)
                return Result<UserMissionDto>.Failure("You can only update your own missions.");

            if (userMission.Status == MissionStatus.Completed)
                return Result<UserMissionDto>.Failure("This mission is already completed.");

            // Update progress
            userMission.Progress = Math.Min(dto.Progress, 100); // Cap at 100%

            // Auto-complete if progress reaches 100%
            if (userMission.Progress >= 100 && userMission.Status != MissionStatus.Completed)
            {
                userMission.Status = MissionStatus.Completed;
                userMission.CompletedAt = DateTime.UtcNow;
            }

            await _userMissionRepository.UpdateAsync(userMission);

            var resultDto = _mapper.Map<UserMissionDto>(userMission);
            resultDto.MissionTitle = userMission.Mission?.Title ?? "Unknown Mission";

            _logger.LogInformation("User {UserId} updated progress on mission {MissionId} to {Progress}%",
                userId, userMission.MissionId, userMission.Progress);

            return Result<UserMissionDto>.Success(resultDto);
        }

        /// <summary>
        /// Gets all active (in-progress) missions for the current user
        /// </summary>
        public async Task<Result<PagedList<UserMissionDto>>> GetMyActiveMissionsAsync(PageParameters pageParameters, Guid userId)
        {
            var userMissions = await _userMissionRepository.GetActiveByUserIdAsync(pageParameters, userId);
            var dtos = _mapper.Map<PagedList<UserMissionDto>>(userMissions);

            return Result<PagedList<UserMissionDto>>.Success(dtos);
        }

        /// <summary>
        /// Gets all completed missions for the current user
        /// </summary>
        public async Task<Result<PagedList<UserMissionDto>>> GetMyCompletedMissionsAsync(PageParameters pageParameters, Guid userId)
        {
            var userMissions = await _userMissionRepository.GetByUserIdAsync(userId);
            var completed = userMissions.Where(um => um.Status == MissionStatus.Completed).ToList();

            var dtos = _mapper.Map<PagedList<UserMissionDto>>(completed);
            return Result<PagedList<UserMissionDto>>.Success(dtos);
        }

        /// <summary>
        /// Gets a specific user mission by ID
        /// </summary>
        public async Task<Result<UserMissionDto>> GetByIdAsync(Guid userMissionId, Guid userId)
        {
            var userMission = await _userMissionRepository.GetByIdAsync(userMissionId);
            if (userMission == null)
                return Result<UserMissionDto>.Failure("User mission not found.");

            // Security check
            if (userMission.UserId != userId)
                return Result<UserMissionDto>.Failure("Access denied.");

            var dto = _mapper.Map<UserMissionDto>(userMission);
            return Result<UserMissionDto>.Success(dto);
        }

        /// <summary>
        /// Gets all user missions (for admin or dashboard purposes)
        /// </summary>
        public async Task<Result<PagedList<UserMissionDto>>> GetAllUserMissionsAsync(PageParameters pageParameters, Guid userId)
        {
            var userMissions = await _userMissionRepository.GetByUserIdAsync(userId);
            var dtos = _mapper.Map<PagedList<UserMissionDto>>(userMissions);
            return Result<PagedList<UserMissionDto>>.Success(dtos);
        }
    }
}