namespace GivingChampion.Application.DTO.Auth;

public sealed record RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
}

public sealed record CompleteSocialRegistrationRequest
{
    public string Userid { get; set; } = string.Empty;
    public string Newpassword { get; set; } = string.Empty;
}

public sealed record LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed record ExternalUserInfo
{
    public string Provider { get; set; } = string.Empty;
    public string ProviderKey { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? FullName { get; set; }
}

public sealed record ExchangeCodeRequest
{
    public string Code { get; set; } = string.Empty;
}

public sealed record TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string[] Roles { get; set; } = [];
}

public sealed record ExternalLoginCodeResponse
{
    public string Code { get; set; } = string.Empty;
    public bool NeedsRegistration { get; set; }
}
