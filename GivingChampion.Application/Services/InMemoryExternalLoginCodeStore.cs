using GivingChampion.Application.Interfaces.Auth;
using GivingChampion.Application.DTO.Auth;
using System.Collections.Concurrent;

namespace GivingChampion.Application.Services
{
    public sealed class InMemoryExternalLoginCodeStore : IExternalLoginCodeStore
    {
        private readonly ConcurrentDictionary<string, (TokenResponse Token, DateTime ExpiresAtUtc)> _store = new();

        public string Store(TokenResponse token)
        {
            var code = Guid.NewGuid().ToString("N");
            _store[code] = (token, DateTime.UtcNow.AddMinutes(1));
            return code;
        }

        public TokenResponse? Take(string code)
        {
            if (!_store.TryRemove(code, out var entry))
            {
                return null;
            }

            if (entry.ExpiresAtUtc < DateTime.UtcNow)
            {
                return null;
            }

            return entry.Token;
        }
    }
}
