using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.Interfaces;
using GivingChampion.Common.DTO;
using GivingChampion.Common.DTO.GeoQuestDto;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Application.Services
{
    public class GeoQuestService : IGeoQuestService
    {
        private readonly IGeoQuestRepository _geoQuestRepository;
        private readonly IMapper _mapper;

        public GeoQuestService(IGeoQuestRepository geoQuestRepository, IMapper mapper)
        {
            _geoQuestRepository = geoQuestRepository;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<GeoQuestDto>>> GetAllAsync(PageParameters pageParameters)
        {
            var geoQuests = await _geoQuestRepository.GetAllAsync(pageParameters);
            var geoQuestDtos = _mapper.Map<PagedList<GeoQuestDto>>(geoQuests); // AutoMapper
            return Result<PagedList<GeoQuestDto>>.Success(geoQuestDtos);
        }

        public async Task<Result<GeoQuestDto?>> GetByIdAsync(Guid id)
        {
            var geoQuest = await _geoQuestRepository.GetByIdAsync(id);
            return geoQuest == null ? Result<GeoQuestDto?>.Failure("GeoQuest not found") : Result<GeoQuestDto?>.Success(_mapper.Map<GeoQuestDto>(geoQuest)); // AutoMapper
        }

        public async Task<Result<GeoQuestDto>> CreateAsync(CreateGeoQuestDto dto)
        {
            var geoQuest = _mapper.Map<GeoQuest>(dto);
            await _geoQuestRepository.AddAsync(geoQuest);
            await _geoQuestRepository.SaveChangesAsync();

            return Result<GeoQuestDto>.Success(_mapper.Map<GeoQuestDto>(geoQuest)); // AutoMapper
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, UpdateGeoQuestDto dto)
        {
            var geoQuest = await _geoQuestRepository.GetByIdAsync(id);
            if (geoQuest == null)
                return Result<bool>.Failure("GeoQuest not found");

            _mapper.Map(dto, geoQuest); // AutoMapper
            _geoQuestRepository.Update(geoQuest);
            await _geoQuestRepository.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> SoftDeleteAsync(Guid id)
        {
            var geoQuest = await _geoQuestRepository.GetByIdAsync(id);
            if (geoQuest == null)
                return Result<bool>.Failure("GeoQuest not found");

            geoQuest.IsDeleted = true;
            geoQuest.DeletedAt = DateTime.UtcNow;
            _geoQuestRepository.Update(geoQuest);
            await _geoQuestRepository.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}