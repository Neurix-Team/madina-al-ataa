using System.Text;
using GivingChampion.Application.Interfaces.Auth;
using GivingChampion.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using GivingChampion.Common.DTO.Auth;

namespace GivingChampion.Application.Services
{
    public sealed class JwtTokenFactory : IJwtTokenFactory
    {
        private readonly JwtOptions _jwtOptions;

        public JwtTokenFactory(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public TokenResponse Create(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponse {
                AccessToken = accessToken,
                ExpiresAtUtc = expiresAtUtc,
                UserId = user.Id.ToString(),
                Email = user.Email ?? string.Empty,
                Roles = roles.ToArray()};
        }
    }
}
