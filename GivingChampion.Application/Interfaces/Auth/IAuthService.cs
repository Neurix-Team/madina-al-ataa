using GivingChampion.Common.DTO.Auth;
using global::GivingChampion.Common.Results;

namespace GivingChampion.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<AuthServiceResult<TokenResponse>> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default);

        Task<AuthServiceResult<TokenResponse>> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default);

        Task<AuthServiceResult<ExternalLoginCodeResponse>> CompleteGoogleLoginAsync(
            ExternalUserInfo externalUser,
            CancellationToken cancellationToken = default);

        Task<AuthServiceResult<TokenResponse>> CompleteSocialRegistrationAsync(
            CompleteSocialRegistrationRequest request,
            CancellationToken cancellationToken = default);

        Task<AuthServiceResult<TokenResponse>> ExchangeExternalCodeAsync(
            string code,
            CancellationToken cancellationToken = default);
    }
}
