using GivingChampion.Application.Interfaces.Certificate;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers.Certificate
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// Handles HTTP requests for Certificate.
    /// </summary>
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;

        /// <summary>
        /// Performs the CertificateController operation.
        /// </summary>
        /// <param name="certificateService">Provides the certificateService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public CertificateController(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        [HttpGet("my")]
        /// <summary>
        /// Retrieves records owned by the current authenticated user.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <param name="cancellationToken">Provides the cancellationToken value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetMyCertificates(
            [FromQuery] PageParameters pageParameters,
            CancellationToken cancellationToken)
        {
            var result = await _certificateService.GetMyCertificatesAsync(
                pageParameters,
                cancellationToken);

            return result.Succeeded
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpGet("user/{userId:guid}")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Performs the GetCertificatesByUserId operation.
        /// </summary>
        /// <param name="userId">Provides the userId value required by the operation.</param>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <param name="cancellationToken">Provides the cancellationToken value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetCertificatesByUserId(
            Guid userId,
            [FromQuery] PageParameters pageParameters,
            CancellationToken cancellationToken)
        {
            var result = await _certificateService.GetCertificatesByIdAsync(
                userId,
                pageParameters,
                cancellationToken);

            return result.Succeeded
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Retrieves a paged collection of records that match the request.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <param name="cancellationToken">Provides the cancellationToken value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetAllCertificates(
            [FromQuery] PageParameters pageParameters,
            CancellationToken cancellationToken)
        {
            var result = await _certificateService.GetAllAsync(
                pageParameters,
                cancellationToken);

            return result.Succeeded
                ? Ok(result)
                : BadRequest(result);
        }
    }
}
