using AutoMapper;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Interfaces.Mission;
using GivingChampion.Common.DTO.Mission;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Extensions.Mapper;
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
        private readonly IMapper _mapper;

        public UserMissionService(IUserMissionRepository userMissionRepository, IMissionRepository missionRepository, IMapper mapper)
        {
            _userMissionRepository = userMissionRepository;
            _missionRepository = missionRepository;
            _mapper = mapper;
        }

        public async Task<Result<UserMissionDto>> StartMissionAsync(StartMissionDto dto, Guid userId)
        {
            var mission = await _missionRepository.GetByIdAsync(dto.MissionId);
            if (mission == null)
                return Result<UserMissionDto>.Failure("Mission not found.");

            //if (mission.Status != MissionStatus.InProgress)
            //    return Result<UserMissionDto>.Failure("This mission is not currently in progress.");

            if (await _userMissionRepository.IsMissionStartedAsync(userId, dto.MissionId))
                return Result<UserMissionDto>.Failure("You have already started this mission.");

            if (await _userMissionRepository.IsMissionCompletedAsync(userId, dto.MissionId))
                return Result<UserMissionDto>.Failure("You have already completed this mission.");

            if (mission.RequiredLevel > 0)
            {
                // Check if user meets level requirements
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

            return Result<UserMissionDto>.Success(resultDto);
        }

        public async Task<Result<UserMissionDto>> UpdateProgressAsync(Guid userMissionId, UpdateProgressDto dto, Guid userId)
        {
            var userMission = await _userMissionRepository.GetByIdAsync(userMissionId);
            if (userMission == null)
                return Result<UserMissionDto>.Failure("User mission not found.");

            if (userMission.UserId != userId)
                return Result<UserMissionDto>.Failure("You can only update your own missions.");

            if (userMission.Status == MissionStatus.Completed)
                return Result<UserMissionDto>.Failure("This mission is already completed.");

            userMission.Progress = Math.Min(dto.Progress, 100);

            if (userMission.Progress >= 100 && userMission.Status != MissionStatus.Completed)
            {
                userMission.Status = MissionStatus.Completed;
                userMission.CompletedAt = DateTime.UtcNow;
            }

            await _userMissionRepository.UpdateAsync(userMission);

            var resultDto = _mapper.Map<UserMissionDto>(userMission);
            resultDto.MissionTitle = userMission.Mission?.Title ?? "Unknown Mission";

            return Result<UserMissionDto>.Success(resultDto);
        }

        public async Task<Result<PagedList<UserMissionDto>>> GetMyActiveMissionsAsync(PageParameters pageParameters, Guid userId)
        {
            var userMissions = await _userMissionRepository.GetActiveByUserIdAsync(pageParameters, userId);
            var dtos = _mapper.MapPagedList<UserMission, UserMissionDto>(userMissions);

            return Result<PagedList<UserMissionDto>>.Success(dtos);
        }

        public async Task<Result<PagedList<UserMissionDto>>> GetMyCompletedMissionsAsync(PageParameters pageParameters, Guid userId)
        {
            var userMissions = await _userMissionRepository.GetByUserIdAsync(pageParameters, userId, MissionStatus.Completed);
            var dtos = _mapper.MapPagedList<UserMission, UserMissionDto>(userMissions);
            return Result<PagedList<UserMissionDto>>.Success(dtos);
        }

        public async Task<Result<UserMissionDto>> GetByIdAsync(Guid userMissionId, Guid userId)
        {
            var userMission = await _userMissionRepository.GetByIdAsync(userMissionId);
            if (userMission == null)
                return Result<UserMissionDto>.Failure("User mission not found.");

            if (userMission.UserId != userId)
                return Result<UserMissionDto>.Failure("Access denied.");

            var dto = _mapper.Map<UserMissionDto>(userMission);
            return Result<UserMissionDto>.Success(dto);
        }
    }
}