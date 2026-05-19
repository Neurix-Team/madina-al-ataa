using GivingChampion.Application.DTO.Auth;
using GivingChampion.Application.Validators.Auth;

namespace GivingChampion.API.Tests.Services;

public class AuthRequestValidatorsTests
{
    private readonly RegisterRequestValidator _validator = new();
    private readonly CompleteSocialRegistrationRequestValidator _completeSocialRegistrationValidator = new();

    [Fact]
    public void RegisterRequest_WithMatchingConfirmPassword_IsValid()
    {
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            Password = "ValidPass1",
            ConfirmPassword = "ValidPass1",
            Fullname = "Test User",
            BirthDate = DateTime.UtcNow.AddYears(-20)
        };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void RegisterRequest_WithoutConfirmPassword_ReturnsValidationError()
    {
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            Password = "ValidPass1",
            ConfirmPassword = string.Empty,
            Fullname = "Test User",
            BirthDate = DateTime.UtcNow.AddYears(-20)
        };

        var result = _validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterRequest.ConfirmPassword)
            && error.ErrorMessage == "Confirm password is required.");
    }

    [Fact]
    public void RegisterRequest_WithMismatchedConfirmPassword_ReturnsValidationError()
    {
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            Password = "ValidPass1",
            ConfirmPassword = "DifferentPass1",
            Fullname = "Test User",
            BirthDate = DateTime.UtcNow.AddYears(-20)
        };

        var result = _validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterRequest.ConfirmPassword)
            && error.ErrorMessage == "Confirm password must match password.");
    }

    [Fact]
    public void CompleteSocialRegistrationRequest_WithValidBirthDate_IsValid()
    {
        var request = new CompleteSocialRegistrationRequest
        {
            Userid = Guid.NewGuid().ToString(),
            Newpassword = "ValidPass1",
            BirthDate = DateTime.UtcNow.AddYears(-20)
        };

        var result = _completeSocialRegistrationValidator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CompleteSocialRegistrationRequest_WithoutBirthDate_ReturnsValidationError()
    {
        var request = new CompleteSocialRegistrationRequest
        {
            Userid = Guid.NewGuid().ToString(),
            Newpassword = "ValidPass1",
            BirthDate = default
        };

        var result = _completeSocialRegistrationValidator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CompleteSocialRegistrationRequest.BirthDate));
    }
}
