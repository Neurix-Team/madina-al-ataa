using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.DTO.VolunteerOrder;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;
using GivingChampion.Application.DTO.GivingChampion.Application.DTO.ActivityDto;
using GivingChampion.Application.DTO.ActivityDto;
using GivingChampion.Application.Interfaces;

namespace GivingChampion.Application.Services
{
    public class ActivityService : BaseService, IActivityService
    {
        private readonly IActivityRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ActivityService(
            IActivityRepository repo,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<ActivityDto>>> GetEntityHistory(
            Guid entityId,
            PageParameters pageParameters)
        {
            var data = await _repo.GetByEntityIdAsync(entityId, pageParameters);

            var mappedItems = _mapper.Map<IReadOnlyList<ActivityDto>>(data.Items);
            var pagedDtos = new PagedList<ActivityDto>(
                mappedItems,
                data.PageNumber,
                data.PageSize,
                data.TotalCount
            );

            return Result<PagedList<ActivityDto>>.Success(pagedDtos);
        }

        public async Task AddAsync(CreateActivityDto createActivityDto)
        {
            var activity = _mapper.Map<Activity>(createActivityDto);
            await _repo.AddAsync(activity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
