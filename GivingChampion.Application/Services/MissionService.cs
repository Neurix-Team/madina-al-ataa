using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.DTO.Mission;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using GivingChampion.Application.DTO.ActivityDto;
using GivingChampion.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace GivingChampion.Application.Services
{
    public class MissionService : BaseService, IMissionService
    {
        private readonly IMissionRepository _missionRepository;
        private readonly IActivityService _activityService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MissionService(
            IMissionRepository missionRepository,
            IActivityService activityService,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            IMapper mapper) : base(httpContextAccessor, activityService)
        {
            _missionRepository = missionRepository;
            _activityService = activityService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<MissionDto>> CreateMissionAsync(CreateMissionDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Mission data is required.");

            var mission = _mapper.Map<Mission>(dto);

            await _missionRepository.CreateAsync(mission);

            await AddActivityAsync(
                mission.Id,
                ActivityEntityType.Mission,
                ActivityAction.MissionCreated,
                $"Mission '{mission.Title}' created."
            );
            await _unitOfWork.SaveChangesAsync();

            var createdDto = _mapper.Map<MissionDto>(mission);

            return Result<MissionDto>.Success(createdDto);
        }

        public async Task<Result<MissionDto>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Mission ID is required.");

            var mission = await _missionRepository.GetByIdAsync(id);

            if (mission == null)
                throw new NotFoundException($"Mission with ID {id} was not found.");

            var dto = _mapper.Map<MissionDto>(mission);

            return Result<MissionDto>.Success(dto);
        }

        public async Task<Result> UpdateMissionAsync(Guid id, UpdateMissionDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Mission ID is required.");

            if (dto == null)
                throw new BadRequestException("Mission update data is required.");

            var mission = await _missionRepository.GetByIdAsync(id);

            if (mission == null)
                throw new NotFoundException($"Mission with ID {id} was not found.");

            if (mission.IsDeleted)
                throw new BadRequestException("Cannot update a deleted mission.");

            _mapper.Map(dto, mission);

            await _missionRepository.UpdateAsync(mission);

            await AddActivityAsync(
                mission.Id,
                ActivityEntityType.Mission,
                ActivityAction.MissionUpdated,
                $"Mission '{mission.Title}' updated."
            );
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> SoftDeleteMissionAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Mission ID is required.");

            var mission = await _missionRepository.GetByIdAsync(id);

            if (mission == null)
                throw new NotFoundException($"Mission with ID {id} was not found.");

            if (mission.IsDeleted)
                throw new BadRequestException("Mission is already deleted.");

            await _missionRepository.SoftDeleteAsync(id);

            await AddActivityAsync(
                mission.Id,
                ActivityEntityType.Mission,
                ActivityAction.MissionDeleted,
                $"Mission '{mission.Title}' deleted."
            );
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<PagedList<MissionDto>>> GetAllActiveAsync(
            PageParameters pageParameters)
        {
            var missions = await _missionRepository.GetAllActiveAsync(pageParameters);

            var dtos = _mapper.MapPagedList<Mission, MissionDto>(missions);

            return Result<PagedList<MissionDto>>.Success(dtos);
        }

        public async Task<Result<PagedList<MissionDto>>> GetAvailableForUserAsync(
            int userLevel,
            PageParameters pageParameters)
        {
            if (userLevel < 0)
                throw new BadRequestException("User level cannot be negative.");

            var missions = await _missionRepository.GetAvailableForLevelAsync(
                userLevel,
                pageParameters);

            var dtos = _mapper.MapPagedList<Mission, MissionDto>(missions);

            return Result<PagedList<MissionDto>>.Success(dtos);
        }
    }
}
