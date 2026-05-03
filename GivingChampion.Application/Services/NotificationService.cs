using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.DTO.Notification;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GivingChampion.Application.Services
{
    public class NotificationService : BaseService, INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<NotificationService> logger,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PagedList<NotificationDto>>> GetMyNotificationsAsync(
            bool unreadOnly = false,
            PageParameters pageParameters = null)
        {
            var notifications = unreadOnly
                ? await _notificationRepository.GetUnreadByUserIdAsync(UserId)
                : await _notificationRepository.GetByUserIdAsync(UserId);

            var dtos = _mapper.Map<List<NotificationDto>>(notifications);

            var pageNumber = pageParameters?.PageNumber ?? 1;
            var pageSize = pageParameters?.PageSize ?? dtos.Count;

            var pagedList = new PagedList<NotificationDto>(
                dtos,
                dtos.Count,
                pageNumber,
                pageSize
            );

            return Result<PagedList<NotificationDto>>.Success(pagedList);
        }

        public async Task<Result> MarkAsReadAsync(Guid notificationId)
        {
            if (notificationId == Guid.Empty)
                throw new BadRequestException("Notification ID is required.");

            await _notificationRepository.MarkAsReadAsync(UserId, notificationId);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> MarkAllAsReadAsync()
        {
            await _notificationRepository.MarkAllAsReadAsync(UserId);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            if (notification == null)
                throw new BadRequestException("Notification data is required.");

            if (notification.UserId == Guid.Empty)
                throw new BadRequestException("Notification user ID is required.");

            if (string.IsNullOrWhiteSpace(notification.Title))
                throw new BadRequestException("Notification title is required.");

            await _notificationRepository.CreateAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Notification sent to user {UserId}: {Title}",
                notification.UserId,
                notification.Title);
        }
    }
}
