using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.DTO;
using GivingChampion.Application.DTO.GeoQuestDto;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services
{
    public class GeoQuestService : IGeoQuestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<GeoQuest> _geoQuestRepository;
        private readonly IMapper _mapper;

        public GeoQuestService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _geoQuestRepository = unitOfWork.Repository<GeoQuest>();
            _mapper = mapper;
        }

        public async Task<Result<PagedList<GeoQuestDto>>> GetAllAsync(PageParameters pageParameters)
        {
            if (pageParameters == null)
                throw new BadRequestException("Page parameters are required.");

            var geoQuests = await _geoQuestRepository.GetAllAsync(pageParameters);

            var geoQuestDtos = _mapper.MapPagedList<GeoQuest, GeoQuestDto>(geoQuests);

            return Result<PagedList<GeoQuestDto>>.Success(geoQuestDtos);
        }

        public async Task<Result<GeoQuestDto?>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("GeoQuest ID is required.");

            var geoQuest = await _geoQuestRepository.GetByIdAsync(id);

            if (geoQuest == null)
                throw new NotFoundException($"GeoQuest with ID {id} was not found.");

            if (geoQuest.IsDeleted)
                throw new NotFoundException($"GeoQuest with ID {id} was not found.");

            var geoQuestDto = _mapper.Map<GeoQuestDto>(geoQuest);

            return Result<GeoQuestDto?>.Success(geoQuestDto);
        }

        public async Task<Result<GeoQuestDto>> CreateAsync(CreateGeoQuestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("GeoQuest create data is required.");

            var geoQuest = _mapper.Map<GeoQuest>(dto);

            await _geoQuestRepository.AddAsync(geoQuest);
            await _unitOfWork.SaveChangesAsync();

            var geoQuestDto = _mapper.Map<GeoQuestDto>(geoQuest);

            return Result<GeoQuestDto>.Success(geoQuestDto);
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, UpdateGeoQuestDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("GeoQuest ID is required.");

            if (dto == null)
                throw new BadRequestException("GeoQuest update data is required.");

            var geoQuest = await _geoQuestRepository.GetByIdAsync(id);

            if (geoQuest == null)
                throw new NotFoundException($"GeoQuest with ID {id} was not found.");

            if (geoQuest.IsDeleted)
                throw new BadRequestException("Cannot update a deleted GeoQuest.");

            _mapper.Map(dto, geoQuest);

            _geoQuestRepository.Update(geoQuest);

            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> SoftDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("GeoQuest ID is required.");

            var geoQuest = await _geoQuestRepository.GetByIdAsync(id);

            if (geoQuest == null)
                throw new NotFoundException($"GeoQuest with ID {id} was not found.");

            if (geoQuest.IsDeleted)
                throw new BadRequestException("GeoQuest is already deleted.");

            geoQuest.IsDeleted = true;
            geoQuest.DeletedAt = DateTime.UtcNow;

            _geoQuestRepository.Update(geoQuest);

            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}
