using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.DTO.Notification;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.Extensions.Logging;

namespace GivingChampion.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PagedList<NotificationDto>>> GetMyNotificationsAsync(
            Guid userId,
            bool unreadOnly = false,
            PageParameters pageParameters = null)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("User ID is required.");

            var notifications = unreadOnly
                ? await _notificationRepository.GetUnreadByUserIdAsync(userId)
                : await _notificationRepository.GetByUserIdAsync(userId);

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

        public async Task<Result> MarkAsReadAsync(Guid userId, Guid notificationId)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("User ID is required.");

            if (notificationId == Guid.Empty)
                throw new BadRequestException("Notification ID is required.");

            await _notificationRepository.MarkAsReadAsync(userId, notificationId);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> MarkAllAsReadAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("User ID is required.");

            await _notificationRepository.MarkAllAsReadAsync(userId);
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
