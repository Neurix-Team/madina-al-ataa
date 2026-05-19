using GivingChampion.Application.Auth;
using GivingChampion.Application.Auth.Interfaces;
using GivingChampion.Application.Interfaces.Auth;
using GivingChampion.Application.DTO.Auth;
using GivingChampion.Common.Results;
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
    [Produces("application/json")]
    /// <summary>
    /// Handles HTTP requests for Auth.
    /// </summary>
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Performs the AuthController operation.
        /// </summary>
        /// <param name="authService">Provides the authService value required by the operation.</param>
        /// <param name="configuration">Provides the configuration value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public AuthController(
            IAuthService authService,
            IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<TokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        /// <summary>
        /// Performs the Register operation.
        /// </summary>
        /// <param name="request">Provides the request value required by the operation.</param>
        /// <param name="cancellationToken">Provides the cancellationToken value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request,
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
        [ProducesResponseType(typeof(Result<TokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        /// <summary>
        /// Performs the ContinueRegistration operation.
        /// </summary>
        /// <param name="request">Provides the request value required by the operation.</param>
        /// <param name="cancellationToken">Provides the cancellationToken value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> ContinueRegistration(
            [FromBody] CompleteSocialRegistrationRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _authService.CompleteSocialRegistrationAsync(request, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Data);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<TokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        /// <summary>
        /// Performs the Login operation.
        /// </summary>
        /// <param name="request">Provides the request value required by the operation.</param>
        /// <param name="cancellationToken">Provides the cancellationToken value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
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
        /// <summary>
        /// Performs the GoogleLogin operation.
        /// </summary>
        /// <returns>The result produced by the operation.</returns>
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
        /// <summary>
        /// Performs the GoogleCallback operation.
        /// </summary>
        /// <param name="remoteError">Provides the remoteError value required by the operation.</param>
        /// <param name="cancellationToken">Provides the cancellationToken value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
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

            var providerKey = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? principal.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(providerKey))
            {
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                return Redirect($"{frontendCallbackUrl}?error=ProviderKeyMissing");
            }

            var externalUser = new ExternalUserInfo
            {
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
        [ProducesResponseType(typeof(Result<TokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        /// <summary>
        /// Performs the ExchangeGoogleCode operation.
        /// </summary>
        /// <param name="request">Provides the request value required by the operation.</param>
        /// <param name="cancellationToken">Provides the cancellationToken value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Me(CancellationToken cancellationToken)
        {
            var currentUser = await _authService.GetCurrentUserAsync(cancellationToken);
            return Ok(currentUser);
        }
    }
}
