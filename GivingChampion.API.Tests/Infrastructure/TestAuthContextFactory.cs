using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GivingChampion.API.Tests.Infrastructure;

internal static class TestAuthContextFactory
{
    public static IHttpContextAccessor CreateHttpContextAccessor(Guid? userId = null, params string[] roles)
    {
        return new HttpContextAccessor
        {
            HttpContext = CreateHttpContext(userId, roles)
        };
    }

    public static ControllerContext CreateControllerContext(Guid? userId = null, params string[] roles)
    {
        return new ControllerContext
        {
            HttpContext = CreateHttpContext(userId, roles)
        };
    }

    private static HttpContext CreateHttpContext(Guid? userId, params string[] roles)
    {
        return new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    BuildClaims(userId, roles),
                    "TestAuth"))
        };
    }

    private static IEnumerable<Claim> BuildClaims(Guid? userId, IEnumerable<string> roles)
    {
        if (userId.HasValue)
            yield return new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString());

        foreach (var role in roles)
            yield return new Claim(ClaimTypes.Role, role);
    }
}
