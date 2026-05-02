using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.VolunteerHistoryService;
using GivingChampion.Common.DTO.VolunteerOrder;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services
{
    public class VolunteerHistoryService : IVolunteerHistoryService
    {
        private readonly IVolunteerHistoryRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VolunteerHistoryService(
            IVolunteerHistoryRepository repo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<VolunteerHistoryDto>>> GetUserHistory(
            Guid userId,
            PageParameters pageParameters)
        {
            var data = await _repo.GetByUserIdAsync(userId, pageParameters);

            var mappedItems = _mapper.Map<IReadOnlyList<VolunteerHistoryDto>>(data.Items);

            var pagedDtos = new PagedList<VolunteerHistoryDto>(
                mappedItems,
                data.PageNumber,
                data.PageSize,
                data.TotalCount
            );

            return Result<PagedList<VolunteerHistoryDto>>.Success(pagedDtos);
        }

        public async Task<Result<PagedList<VolunteerHistoryDto>>> GetRequestHistory(
            Guid requestId,
            PageParameters pageParameters)
        {
            var data = await _repo.GetByRequestIdAsync(requestId, pageParameters);

            var mappedItems = _mapper.Map<IReadOnlyList<VolunteerHistoryDto>>(data.Items);

            var pagedDtos = new PagedList<VolunteerHistoryDto>(
                mappedItems,
                data.PageNumber,
                data.PageSize,
                data.TotalCount
            );

            return Result<PagedList<VolunteerHistoryDto>>.Success(pagedDtos);
        }

        public async Task AddAsync(
            Guid userId,
            Guid requestId,
            Guid orderId,
            VolunteerHistoryAction action,
            int? progress = null)
        {
            var history = new VolunteerHistories
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ServiceRequestId = requestId,
                VolunteerOrderId = orderId,
                Action = action,
                ProgressValue = progress,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(history);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
