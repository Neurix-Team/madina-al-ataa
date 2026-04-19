using AutoMapper;
using GivingChampion.Application.Interfaces;
using GivingChampion.Common.DTO.Notification;
using GivingChampion.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingChampion.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            IMapper mapper,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<List<NotificationDto>>> GetMyNotificationsAsync(Guid userId, bool unreadOnly = false)
        {

            var notifications = unreadOnly
                ? await _notificationRepository.GetUnreadByUserIdAsync(userId)
                : await _notificationRepository.GetByUserIdAsync(userId);

            var dtos = _mapper.Map<List<NotificationDto>>(notifications);
            return Result<List<NotificationDto>>.Success(dtos);
        }

        public async Task<Result> MarkAsReadAsync(Guid userId, Guid notificationId)
        {
            await _notificationRepository.MarkAsReadAsync(userId, notificationId);
            return Result.Success();
        }

        public async Task<Result> MarkAllAsReadAsync(Guid userId)
        {
            await _notificationRepository.MarkAllAsReadAsync(userId);
            return Result.Success();
        }

        // Internal method for other services to send notifications
        public async Task CreateNotificationAsync(Notification notification)
        {
            await _notificationRepository.CreateAsync(notification);
            _logger.LogInformation("Notification sent to user {UserId}: {Title}",
                notification.UserId, notification.Title);
        }
    }
}