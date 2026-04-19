using AutoMapper;
using GivingChampion.Application.Interfaces;
using GivingChampion.Common.DTO.Mission;
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
        private readonly ILogger<MissionService> _logger;

        public MissionService(
            IMissionRepository missionRepository,
            IMapper mapper,
            ILogger<MissionService> logger)
        {
            _missionRepository = missionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<List<MissionDto>>> GetAllActiveAsync()
        {
            var missions = await _missionRepository.GetAllActiveAsync();
            var dtos = _mapper.Map<List<MissionDto>>(missions);
            return Result<List<MissionDto>>.Success(dtos);
        }

        public async Task<Result<List<MissionDto>>> GetAvailableForUserAsync(int userLevel)
        {
            var missions = await _missionRepository.GetAvailableForLevelAsync(userLevel);
            var dtos = _mapper.Map<List<MissionDto>>(missions);
            return Result<List<MissionDto>>.Success(dtos);
        }

        public async Task<Result<MissionDto>> GetByIdAsync(Guid id)
        {
            var mission = await _missionRepository.GetByIdAsync(id);
            if (mission == null)
                return Result<MissionDto>.Failure("Mission not found");

            var dto = _mapper.Map<MissionDto>(mission);
            return Result<MissionDto>.Success(dto);
        }

        public async Task<Result<MissionDto>> CreateMissionAsync(CreateMissionDto dto)
        {
            var mission = _mapper.Map<Mission>(dto);
            await _missionRepository.CreateAsync(mission);

            var createdDto = _mapper.Map<MissionDto>(mission);
            return Result<MissionDto>.Success(createdDto);
        }

        public async Task<Result> UpdateMissionAsync(Guid id, UpdateMissionDto dto)
        {
            var mission = await _missionRepository.GetByIdAsync(id);
            if (mission == null)
                return Result.Failure("Mission not found");

            _mapper.Map(dto, mission);
            await _missionRepository.UpdateAsync(mission);

            return Result.Success();
        }

        public async Task<Result> SoftDeleteMissionAsync(Guid id)
        {
            await _missionRepository.SoftDeleteAsync(id);
            return Result.Success();
        }
    }
}