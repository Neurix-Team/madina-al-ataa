using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.BadgeDto;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Services
{
    public class BadgeService : IBadgeService
    {
        private readonly IBadgeRepository _badgeRepository;
        private readonly IMapper _mapper;

        public BadgeService(IBadgeRepository badgeRepository, IMapper mapper)
        {
            _badgeRepository = badgeRepository;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<BadgeDto>>> GetAllAsync(PageParameters pageParameters)
        {
            var badges = await _badgeRepository.GetAllAsync(pageParameters);
            return Result<PagedList<BadgeDto>>.Success(_mapper.Map<PagedList<BadgeDto>>(badges)); 
        }

        public async Task<Result<BadgeDto?>> GetByIdAsync(Guid id)
        {
            var badge = await _badgeRepository.GetByIdAsync(id);
            return badge == null ? Result<BadgeDto?>.Failure("Badge not found") : Result<BadgeDto?>.Success(_mapper.Map<BadgeDto>(badge));
        }

        public async Task<Result<BadgeDto>> CreateAsync(CreateBadgeDto dto)
        {
            var badge = _mapper.Map<Badge>(dto);
            await _badgeRepository.AddAsync(badge);
            await _badgeRepository.SaveChangesAsync();

            return Result<BadgeDto>.Success(_mapper.Map<BadgeDto>(badge)); // AutoMapper
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, UpdateBadgeDto dto)
        {
            var badge = await _badgeRepository.GetByIdAsync(id);
            if (badge == null)
                return Result<bool>.Failure("Badge not found");
            _mapper.Map(dto, badge); // AutoMapper
            _badgeRepository.Update(badge);
            await _badgeRepository.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> SoftDeleteAsync(Guid id)
        {
            var badge = await _badgeRepository.GetByIdAsync(id);
            if (badge == null)
                return Result<bool>.Failure("Badge not found");
            badge.IsDeleted = true;
            badge.DeletedAt = DateTime.UtcNow;
            _badgeRepository.Update(badge);
            await _badgeRepository.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}