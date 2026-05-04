using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.BadgeDto;
using GivingChampion.Application.Exceptions;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.API.Services
{
    public class BadgeService : IBadgeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IMapper _mapper;

        public BadgeService(
            IUnitOfWork unitOfWork,
            IBadgeRepository badgeRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _badgeRepository = badgeRepository;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<BadgeDto>>> GetAllAsync(PageParameters pageParameters)
        {
            if (pageParameters == null)
                throw new BadRequestException("Page parameters are required.");

            var badges = await _badgeRepository.GetAllAsync(pageParameters);

            var badgeDtos = _mapper.MapPagedList<Badge, BadgeDto>(badges);

            return Result<PagedList<BadgeDto>>.Success(badgeDtos);
        }

        public async Task<Result<BadgeDto?>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Badge ID is required.");

            var badge = await _badgeRepository.GetByIdAsync(id);

            if (badge == null)
                throw new NotFoundException($"Badge with ID {id} was not found.");

            var badgeDto = _mapper.Map<BadgeDto>(badge);

            return Result<BadgeDto?>.Success(badgeDto);
        }

        public async Task<Result<BadgeDto>> CreateAsync(CreateBadgeDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Badge create data is required.");

            var badge = _mapper.Map<Badge>(dto);

            await _badgeRepository.AddAsync(badge);

            await _unitOfWork.SaveChangesAsync();

            var badgeDto = _mapper.Map<BadgeDto>(badge);

            return Result<BadgeDto>.Success(badgeDto);
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, UpdateBadgeDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Badge ID is required.");

            if (dto == null)
                throw new BadRequestException("Badge update data is required.");

            var badge = await _badgeRepository.GetByIdAsync(id);

            if (badge == null)
                throw new NotFoundException($"Badge with ID {id} was not found.");

            _mapper.Map(dto, badge);

            _badgeRepository.Update(badge);

            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> SoftDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Badge ID is required.");

            var badge = await _badgeRepository.GetByIdAsync(id);

            if (badge == null)
                throw new NotFoundException($"Badge with ID {id} was not found.");

            badge.IsDeleted = true;
            badge.DeletedAt = DateTime.UtcNow;

            _badgeRepository.Update(badge);

            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}