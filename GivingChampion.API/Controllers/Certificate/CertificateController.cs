using GivingChampion.Application.Interfaces.Certificate;
using GivingChampion.Common.DTO.CertificateDto;
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

            #region Fields

            private readonly ICertificateService _certificateService;
            private readonly ILogger<CertificateController> _logger;

            #endregion

            #region Constructor

            // Constructor to initialize CertificateService and Logger
            public CertificateController(ICertificateService certificateService, ILogger<CertificateController> logger)
            {
                _certificateService = certificateService ?? throw new ArgumentNullException(nameof(certificateService));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

        #endregion

        #region Query Methods

        #region GetById
        /// <summary>
        /// Retrieves a certificate by its unique identifier (ID).
        /// </summary>
        [Authorize]
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetCertificatesByUserId(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Call the service to get certificates for the user
                var certificates = await _certificateService.GetCertificatesByIdAsync(userId, cancellationToken);
                // Step 2: Return the certificates as an OK response
                return Ok(certificates);
            }
            catch (Exception ex)
            {
                // Log error and return 500 Internal Server Error
                _logger.LogError(ex, "Error occurred while fetching certificates for user.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

            #region GetAllCertificates
            /// <summary>
            /// Retrieves all certificates.
            /// </summary>
        [Authorize]
            [HttpGet]
            public async Task<IActionResult> GetAllCertificates(CancellationToken cancellationToken)
            {
                try
                {
                var userId = new Guid(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
                var certificates = await _certificateService.GetCertificatesByIdAsync(userId, cancellationToken);
                    return Ok(certificates);
                }
                catch (Exception ex)
                {
                    // Log error and return 500 Internal Server Error
                    _logger.LogError(ex, "Error occurred while fetching all certificates.");
                    return StatusCode(500, "Internal server error");
                }
            }

        #endregion

        #endregion


        //#region CreateCertificate
        ///// <summary>
        ///// Creates a new certificate.
        ///// </summary>
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CertificateCreateDto dto, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        // Call the service to create the certificate
        //        var createdCertificate = await _certificateService.CreateAsync(dto, cancellationToken);

        //        // Return the created certificate with 201 status code
        //        return CreatedAtAction(nameof(GetById), new { id = createdCertificate?.Id }, createdCertificate);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log error and return 500 Internal Server Error
        //        _logger.LogError(ex, "Error occurred while creating the certificate.");
        //        return StatusCode(500, "Internal server error");
        //    }
        //}
        //#endregion
    }
    }
