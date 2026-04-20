using GivingChampion.Application.Interfaces.Certificate;
using GivingChampion.Common.DTO.CertificateDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            [HttpGet("{id:guid}")]
            public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
            {
                try
                {
                    var certificate = await _certificateService.GetCertificateByIdAsync(cancellationToken);

                    // If certificate is not found, return 404 Not Found
                    if (certificate == null)
                    {
                        _logger.LogWarning($"Certificate with ID {id} not found.");
                        return NotFound($"Certificate with ID {id} not found.");
                    }

                    // Return the certificate data if found
                    return Ok(certificate);
                }
                catch (Exception ex)
                {
                    // Log error and return 500 Internal Server Error
                    _logger.LogError(ex, $"Error occurred while fetching certificate with ID {id}.");
                    return StatusCode(500, "Internal server error");
                }
            }
            #endregion

            #region GetAllCertificates
            /// <summary>
            /// Retrieves all certificates.
            /// </summary>
            [HttpGet]
            public async Task<IActionResult> GetAllCertificates(CancellationToken cancellationToken)
            {
                try
                {
                    var certificates = await _certificateService.GetCertificateByIdAsync(cancellationToken);
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


            #region CreateCertificate
            /// <summary>
            /// Creates a new certificate.
            /// </summary>
            [HttpPost]
            public async Task<IActionResult> Create([FromBody] CertificateCreateDto dto, CancellationToken cancellationToken)
            {
                try
                {
                    // Call the service to create the certificate
                    var createdCertificate = await _certificateService.CreateAsync(dto, cancellationToken);

                    // Return the created certificate with 201 status code
                    return CreatedAtAction(nameof(GetById), new { id = createdCertificate?.Id }, createdCertificate);
                }
                catch (Exception ex)
                {
                    // Log error and return 500 Internal Server Error
                    _logger.LogError(ex, "Error occurred while creating the certificate.");
                    return StatusCode(500, "Internal server error");
                }
            }
            #endregion
        }
    }
