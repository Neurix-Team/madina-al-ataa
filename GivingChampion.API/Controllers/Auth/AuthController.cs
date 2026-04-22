using GivingChampion.Application.Auth;
using GivingChampion.Application.Auth.Interfaces;
using GivingChampion.Application.Interfaces.Auth;
using GivingChampion.Common.DTO.Auth;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GivingChampion.API.Controllers.Auth
{
    /// <summary>
    /// Authentication endpoints for user registration, login, and social authentication (Google).
    /// Supports both traditional email/password and external provider flows.
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/auth")]
    [Produces("application/json")]
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

        #region Traditional Authentication

        /// <summary>
        /// Registers a new user with email and password.
        /// </summary>
        /// <param name="request">Registration details including email, password, and full name</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Registration result containing user information</returns>
        /// <response code="200">User successfully registered</response>
        /// <response code="400">Invalid input or registration failed (e.g., email already exists)</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<TokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Completes the registration for users who signed up via social login (Google).
        /// This step allows users to provide additional profile information.
        /// </summary>
        /// <param name="request">Additional registration details (FullName, BirthDate, etc.)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Completed registration result</returns>
        /// <response code="200">Registration completed successfully</response>
        /// <response code="400">Invalid data or completion failed</response>
        /// <response code="401">Unauthorized - User must be authenticated via social login</response>
        [HttpPost("continue-registration")]
        [Authorize]
        [ProducesResponseType(typeof(Result<TokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ContinueRegistration(
            [FromBody] CompleteSocialRegistrationRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _authService.CompleteSocialRegistrationAsync(request, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Authenticates a user with email and password.
        /// </summary>
        /// <param name="request">Login credentials (email and password)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Login result containing JWT token and user info</returns>
        /// <response code="200">Login successful - returns authentication token</response>
        /// <response code="401">Invalid credentials or account is locked</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<TokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request, cancellationToken);

            if (!result.Succeeded)
            {
                return Unauthorized(new { errors = result.Errors });
            }

            return Ok(result.Data);
        }

        #endregion

        #region Google OAuth

        /// <summary>
        /// Initiates Google OAuth login flow.
        /// Redirects the user to Google's authorization page.
        /// </summary>
        /// <returns>Challenge response to start Google authentication</returns>
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

        /// <summary>
        /// Google OAuth callback endpoint.
        /// Handles the response from Google after user authorization.
        /// </summary>
        /// <param name="remoteError">Error from Google (if any)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Redirects to frontend with success or error parameters</returns>
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

        /// <summary>
        /// Exchanges the temporary code received from Google callback for a full authentication token.
        /// </summary>
        /// <param name="request">Contains the code received from the callback</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Full authentication response with token and user data</returns>
        [HttpPost("google/exchange")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<TokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
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

        #endregion

        #region User Info

        /// <summary>
        /// Returns basic information about the currently authenticated user.
        /// </summary>
        /// <returns>Current user claims (id, email, username, roles)</returns>
        /// <response code="200">Returns current user information</response>
        /// <response code="401">Unauthorized - User is not authenticated</response>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

        #endregion
    }
}