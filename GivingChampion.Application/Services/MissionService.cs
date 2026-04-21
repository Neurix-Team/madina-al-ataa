using AutoMapper;
using GivingChampion.Application.Interfaces;
using GivingChampion.Common.DTO.Mission;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingChampion.Application.Services
{
    public class MissionService : IMissionService
    {
        private readonly IMissionRepository _missionRepository;
        private readonly IMapper _mapper;

        public MissionService(IMissionRepository missionRepository, IMapper mapper)
        {
            _missionRepository = missionRepository;
            _mapper = mapper;
        }

        public async Task<Result<MissionDto>> CreateMissionAsync(CreateMissionDto dto)
        {
            var mission = _mapper.Map<Mission>(dto);
            await _missionRepository.CreateAsync(mission);
            var createdDto = _mapper.Map<MissionDto>(mission);
            return Result<MissionDto>.Success(createdDto);
        }

        public async Task<Result<MissionDto>> GetByIdAsync(Guid id)
        {
            var mission = await _missionRepository.GetByIdAsync(id);
            if (mission == null) return Result<MissionDto>.Failure("Mission not found");
            var dto = _mapper.Map<MissionDto>(mission);
            return Result<MissionDto>.Success(dto);
        }

        public async Task<Result> UpdateMissionAsync(Guid id, UpdateMissionDto dto)
        {
            var mission = await _missionRepository.GetByIdAsync(id);
            if (mission == null) return Result.Failure("Mission not found");
            _mapper.Map(dto, mission);
            await _missionRepository.UpdateAsync(mission);
            return Result.Success();
        }

        public async Task<Result> SoftDeleteMissionAsync(Guid id)
        {
            await _missionRepository.SoftDeleteAsync(id);
            return Result.Success();
        }

        public async Task<Result<PagedList<MissionDto>>> GetAllActiveAsync(PageParameters pageParameters)
        {
            var missions = await _missionRepository.GetAllActiveAsync(pageParameters);
            var dtos = _mapper.MapPagedList<Mission,MissionDto>(missions);
            return Result<PagedList<MissionDto>>.Success(dtos);
        }

        public async Task<Result<PagedList<MissionDto>>> GetAvailableForUserAsync(int userLevel, PageParameters pageParameters)
        {
            var missions = await _missionRepository.GetAvailableForLevelAsync(userLevel, pageParameters);
            var dtos = _mapper.MapPagedList<Mission,MissionDto>(missions);
            return Result<PagedList<MissionDto>>.Success(dtos);
        }
    }
}