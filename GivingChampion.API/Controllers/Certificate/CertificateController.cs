using GivingChampion.Application.Interfaces.Certificate;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers.Certificate
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;
        private readonly ILogger<CertificateController> _logger;

        public CertificateController(
            ICertificateService certificateService,
            ILogger<CertificateController> logger)
        {
            _certificateService = certificateService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyCertificates(
            [FromQuery] PageParameters pageParameters,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _certificateService.GetMyCertificatesAsync(
                    pageParameters,
                    cancellationToken);

                if (!result.Succeeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching current user certificates.");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetCertificatesByUserId(
            Guid userId,
            [FromQuery] PageParameters pageParameters,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _certificateService.GetCertificatesByIdAsync(
                    userId,
                    pageParameters,
                    cancellationToken);

                if (!result.Succeeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching certificates for user {UserId}.", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllCertificates(
            [FromQuery] PageParameters pageParameters,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _certificateService.GetAllAsync(
                    pageParameters,
                    cancellationToken);

                if (!result.Succeeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all certificates.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}