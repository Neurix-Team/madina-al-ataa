using GivingChampion.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO.Auth;

public sealed record RegisterRequest
{
    [EmailAddress(ErrorMessage = "Email must be a Valid Email Address")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password Must be Entered")]
    public string Password { get; set; }
    [Required(ErrorMessage = "The Full Name Must be Entered")]
    public string Fullname { get; set; }
    [MinimumAge(18, ErrorMessage = "Your Age must be 18 Years or Older")]
    public DateTime BirthDate { get; set; }
    //[Required(ErrorMessage = "City Must be Entered")]
    //public string City { get; set; }
    //[Required(ErrorMessage = "Address Must be Entered")]
    //public string Address { get; set; }
}

public sealed record CompleteSocialRegistrationRequest
{
    public string Userid { get; set; }
    public string Newpassword { get; set; }
}

public sealed record LoginRequest 
{ 
    public string Email { get; set; }
    public string Password { get; set; }
}

public sealed record ExternalUserInfo
{
    public string Provider { get; set; }
    public string ProviderKey { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
}

public sealed record ExchangeCodeRequest 
{ 
    public string Code { get; set; }
}

public sealed record TokenResponse
{
    public string AccessToken { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
    public string[] Roles { get; set; }
}

public sealed record ExternalLoginCodeResponse 
{ 
    public string Code { get; set; }
    public bool NeedsRegistration { get; set; }
}