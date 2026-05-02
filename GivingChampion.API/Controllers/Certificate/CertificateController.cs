using GivingChampion.Application.Interfaces.Certificate;
using GivingChampion.Application.DTO.CertificateDto;
using GivingChampion.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //public async Task<IActionResult> Create(
        //    [FromBody] CertificateCreateDto dto,
        //    CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var createdCertificate = await _certificateService.CreateAsync(dto, cancellationToken);
        //
        //        if (createdCertificate == null)
        //            return BadRequest("Volunteer does not exist.");
        //
        //        return Ok(createdCertificate);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while creating the certificate.");
        //        return StatusCode(500, "Internal server error");
        //    }
        //}
    }
}