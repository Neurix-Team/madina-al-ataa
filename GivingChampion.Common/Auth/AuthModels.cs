namespace GivingChampion.Common.Auth;

public sealed record RegisterRequest(string Email, string Password, string FullName, DateTime BirthDate);

public sealed record LoginRequest(string Email, string Password);

public sealed record ExternalUserInfo(
    string Provider,
    string ProviderKey,
    string? Email,
    string? FullName);

public sealed record ExchangeCodeRequest(string Code);

public sealed record TokenResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    string UserId,
    string Email,
    string[] Roles);

public sealed record ExternalLoginCodeResponse(string Code);