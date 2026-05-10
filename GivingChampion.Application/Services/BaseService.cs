using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GivingChampion.Application.Services
{
    public abstract class BaseService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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
    }
}
