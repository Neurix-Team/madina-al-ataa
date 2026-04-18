using GivingChampion.Application.Auth;
using GivingChampion.Application.Auth.Interfaces;
using GivingChampion.Common.DTO.Auth;
using global::GivingChampion.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GivingChampion.API.Controllers.Auth
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(
            IAuthService authService,
            IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(
            RegisterRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Data);
        }

        [HttpPost("continue-registration")]
        [Authorize]
        public async Task<IActionResult> ContinueRegistration(
            CompleteSocialRegistrationRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //if (string.IsNullOrWhiteSpace(currentUserId) || currentUserId != request.Userid)
            //{
            //    return Forbid();
            //}

            var result = await _authService.CompleteSocialRegistrationAsync(request, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Data);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            LoginRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request, cancellationToken);

            if (!result.Succeeded)
            {
                return Unauthorized(new { errors = result.Errors });
            }

            return Ok(result.Data);
        }

        [HttpGet("google/login")]
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            var callbackUrl = Url.ActionLink(nameof(GoogleCallback));

            if (string.IsNullOrWhiteSpace(callbackUrl))
            {
                return BadRequest("Could not generate callback URL.");
            }

            var properties = new AuthenticationProperties
            {
                RedirectUri = callbackUrl
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback(
            [FromQuery] string? remoteError = null,
            CancellationToken cancellationToken = default)
        {
            var frontendCallbackUrl = _configuration["Frontend:AuthCallbackUrl"]
                ?? "http://localhost:5173/auth/callback";

            if (!string.IsNullOrWhiteSpace(remoteError))
            {
                return Redirect($"{frontendCallbackUrl}?error={Uri.EscapeDataString(remoteError)}");
            }

            var externalResult = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            if (!externalResult.Succeeded || externalResult.Principal is null)
            {
                return Redirect($"{frontendCallbackUrl}?error=ExternalAuthenticationFailed");
            }

            var principal = externalResult.Principal;

            var providerKey =
                principal.FindFirstValue(ClaimTypes.NameIdentifier) ??
                principal.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(providerKey))
            {
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                return Redirect($"{frontendCallbackUrl}?error=ProviderKeyMissing");
            }

            var externalUser = new ExternalUserInfo {
                Provider = "Google",
                ProviderKey = providerKey,
                Email = principal.FindFirstValue(ClaimTypes.Email),
                FullName = principal.FindFirstValue(ClaimTypes.Name) 
            };

            var result = await _authService.CompleteGoogleLoginAsync(externalUser, cancellationToken);

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            if (!result.Succeeded || result.Data is null)
            {
                var error = result.Errors.FirstOrDefault()?.Description ?? "Google login failed.";
                return Redirect($"{frontendCallbackUrl}?error={Uri.EscapeDataString(error)}");
            }

            return Redirect($"{frontendCallbackUrl}?code={Uri.EscapeDataString(result.Data.Code)}&needsregistration={result.Data.NeedsRegistration}");
        }

        [HttpPost("google/exchange")]
        [AllowAnonymous]
        public async Task<IActionResult> ExchangeGoogleCode(
            [FromBody] ExchangeCodeRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _authService.ExchangeExternalCodeAsync(request.Code, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Data);
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            return Ok(new
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                email = User.FindFirstValue(ClaimTypes.Email),
                userName = User.Identity?.Name,
                roles = User.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray()
            });
        }
    }
}
