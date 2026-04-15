using global::GivingChampion.Common.Auth;
using global::GivingChampion.Common.Results;

namespace GivingChampion.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<ServiceResult<TokenResponse>> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default);

        Task<ServiceResult<TokenResponse>> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default);

        Task<ServiceResult<ExternalLoginCodeResponse>> CompleteGoogleLoginAsync(
            ExternalUserInfo externalUser,
            CancellationToken cancellationToken = default);

        Task<ServiceResult<TokenResponse>> CompleteSocialRegistrationAsync(
            CompleteSocialRegistrationRequest request,
            CancellationToken cancellationToken = default);

        Task<ServiceResult<TokenResponse>> ExchangeExternalCodeAsync(
            string code,
            CancellationToken cancellationToken = default);
    }
}
