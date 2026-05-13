using GivingChampion.Application.DTO.ActivityDto;
using GivingChampion.Application.Interfaces;
using GivingChampion.Common.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GivingChampion.Application.Services
{
    public abstract class BaseService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActivityService? _activityService;

        protected BaseService(IHttpContextAccessor httpContextAccessor, IActivityService? activityService = null)
        {
            _httpContextAccessor = httpContextAccessor;
            _activityService = activityService;
        }


        protected Guid UserId
        {
            get
            {
                var userIdValue = _httpContextAccessor.HttpContext?
                    .User
                    .Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?
                    .Value;

                if (string.IsNullOrWhiteSpace(userIdValue))
                    throw new UnauthorizedAccessException("User ID claim was not found.");

                return new Guid(userIdValue);
            }
        }
        protected async Task AddActivityAsync(
            Guid entityId,
            ActivityEntityType entityType,
            ActivityAction action,
            string description)
        {
            if (_activityService == null)
                throw new InvalidOperationException("Activity service is not configured for this service.");

            await _activityService.AddAsync(new CreateActivityDto
            {
                UserId = UserId,
                EntityId = entityId,
                EntityType = entityType,
                Action = action,
                Description = description
            });
        }
    }
}