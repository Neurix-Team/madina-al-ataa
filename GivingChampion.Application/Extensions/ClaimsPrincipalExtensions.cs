using System.Security.Claims;

namespace GivingChampion.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool TryGetCurrentUserId(this ClaimsPrincipal user, out Guid userId)
        {
            userId = Guid.Empty;

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return !string.IsNullOrWhiteSpace(userIdClaim)
                   && Guid.TryParse(userIdClaim, out userId);
        }
    }
}