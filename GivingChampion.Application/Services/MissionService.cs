using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.DTO.Mission;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services
{
    public class MissionService : IMissionService
    {
        private readonly IMissionRepository _missionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MissionService(
            IMissionRepository missionRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _missionRepository = missionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<MissionDto>> CreateMissionAsync(CreateMissionDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Mission data is required.");

            var mission = _mapper.Map<Mission>(dto);

            await _missionRepository.CreateAsync(mission);
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
