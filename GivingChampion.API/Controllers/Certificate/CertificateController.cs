using GivingChampion.Application.Interfaces.Certificate;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers.Certificate
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;

        public CertificateController(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        [HttpGet("my")]
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
        //[Authorize(Roles = "Admin")]
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