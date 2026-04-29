using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.VolunteerHistoryService;
using GivingChampion.Common.DTO.VolunteerOrder;
using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services
{
    public class VolunteerHistoryService : IVolunteerHistoryService
    {
        private readonly IVolunteerHistoryRepository _repo;
        private readonly IMapper _mapper;

        public VolunteerHistoryService(
            IVolunteerHistoryRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<VolunteerHistoryDto>> GetUserHistory(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("User ID is required.");

            var data = await _repo.GetByUserIdAsync(userId);

            return _mapper.Map<List<VolunteerHistoryDto>>(data);
        }

        public async Task<List<VolunteerHistoryDto>> GetRequestHistory(Guid requestId)
        {
            if (requestId == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            var data = await _repo.GetByRequestIdAsync(requestId);

            return _mapper.Map<List<VolunteerHistoryDto>>(data);
        }

        public async Task AddAsync(
            Guid userId,
            Guid requestId,
            Guid orderId,
            VolunteerHistoryAction action,
            int? progress = null)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("User ID is required.");

            if (requestId == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            if (orderId == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            if (!Enum.IsDefined(typeof(VolunteerHistoryAction), action))
                throw new BadRequestException("Volunteer history action is invalid.");

            if (progress.HasValue && (progress.Value < 0 || progress.Value > 100))
                throw new BadRequestException("Progress value must be between 0 and 100.");

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

            await _repo.SaveChangesAsync();
        }
    }
}