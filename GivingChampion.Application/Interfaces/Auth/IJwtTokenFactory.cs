using GivingChampion.Common.Auth;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Application.Interfaces.Auth
{
    public interface IJwtTokenFactory
    {
        TokenResponse Create(ApplicationUser user, IList<string> roles);
    }
}
