using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Interfaces.Mission;
using GivingChampion.Common.DTO.Mission;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services
{
    public class UserMissionService : IUserMissionService
    {
        private readonly IUserMissionRepository _userMissionRepository;
        private readonly IMissionRepository _missionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserMissionService(
            IUserMissionRepository userMissionRepository,
            IMissionRepository missionRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userMissionRepository = userMissionRepository;
            _missionRepository = missionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<UserMissionDto>> StartMissionAsync(StartMissionDto dto, Guid userId)
        {
            if (dto == null)
                throw new BadRequestException("Mission start data is required.");

            if (userId == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid user token.");

            if (dto.MissionId == Guid.Empty)
                throw new BadRequestException("Mission ID is required.");

            var mission = await _missionRepository.GetByIdAsync(dto.MissionId);

            if (mission == null)
                throw new NotFoundException("Mission not found.");

            // لو عندك Status للـ Mission وعايز تمنع بدء Mission غير نشطة، فعّل الشرط ده:
            // if (mission.Status != MissionStatus.InProgress)
            //     throw new BadRequestException("This mission is not currently in progress.");

            if (await _userMissionRepository.IsMissionCompletedAsync(userId, dto.MissionId))
                throw new ConflictException("You have already completed this mission.");

            if (await _userMissionRepository.IsMissionStartedAsync(userId, dto.MissionId))
                throw new ConflictException("You have already started this mission.");

            if (mission.RequiredLevel > 0)
            {
                // TODO:
                // Check user profile level here when profile/user-level repository is available.
                // If user level is lower than required:
                // throw new BadRequestException("Your level is not high enough to start this mission.");
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
            await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<UserMissionDto>(userMission);
            resultDto.MissionTitle = mission.Title;
            resultDto.KPReward = mission.KPReward;
            resultDto.XPReward = mission.XPReward;

            return Result<UserMissionDto>.Success(resultDto);
        }

        public async Task<Result<UserMissionDto>> UpdateProgressAsync(
            Guid userMissionId,
            UpdateProgressDto dto,
            Guid userId)
        {
            if (dto == null)
                throw new BadRequestException("Progress update data is required.");

            if (userId == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid user token.");

            if (userMissionId == Guid.Empty)
                throw new BadRequestException("User mission ID is required.");

            if (dto.Progress < 0)
                throw new BadRequestException("Progress cannot be less than zero.");

            var userMission = await _userMissionRepository.GetByIdAsync(userMissionId);

            if (userMission == null)
                throw new NotFoundException("User mission not found.");

            if (userMission.UserId != userId)
                throw new UnauthorizedAccessException("You can only update your own missions.");

            if (userMission.Status == MissionStatus.Completed)
                throw new ConflictException("This mission is already completed.");

            userMission.Progress = Math.Min(dto.Progress, 100);

            if (userMission.Progress >= 100)
            {
                userMission.Status = MissionStatus.Completed;
                userMission.CompletedAt = DateTime.UtcNow;
            }

            await _userMissionRepository.UpdateAsync(userMission);
            await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<UserMissionDto>(userMission);
            resultDto.MissionTitle = userMission.Mission?.Title ?? "Unknown Mission";

            return Result<UserMissionDto>.Success(resultDto);
        }

        public async Task<Result<PagedList<UserMissionDto>>> GetMyActiveMissionsAsync(
            PageParameters pageParameters,
            Guid userId)
        {
            if (userId == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid user token.");

            var userMissions = await _userMissionRepository.GetActiveByUserIdAsync(
                pageParameters,
                userId);

            var dtos = _mapper.MapPagedList<UserMission, UserMissionDto>(userMissions);

            return Result<PagedList<UserMissionDto>>.Success(dtos);
        }

        public async Task<Result<PagedList<UserMissionDto>>> GetMyCompletedMissionsAsync(
            PageParameters pageParameters,
            Guid userId)
        {
            if (userId == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid user token.");

            var userMissions = await _userMissionRepository.GetByUserIdAsync(
                pageParameters,
                userId,
                MissionStatus.Completed);

            var dtos = _mapper.MapPagedList<UserMission, UserMissionDto>(userMissions);

            return Result<PagedList<UserMissionDto>>.Success(dtos);
        }

        public async Task<Result<UserMissionDto>> GetByIdAsync(
            Guid userMissionId,
            Guid userId)
        {
            if (userId == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid user token.");

            if (userMissionId == Guid.Empty)
                throw new BadRequestException("User mission ID is required.");

            var userMission = await _userMissionRepository.GetByIdAsync(userMissionId);

            if (userMission == null)
                throw new NotFoundException("User mission not found.");

            if (userMission.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            var dto = _mapper.Map<UserMissionDto>(userMission);

            return Result<UserMissionDto>.Success(dto);
        }
    }
}
