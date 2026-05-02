using AutoMapper;
using GivingChampion.Application.Interfaces.Certificate;
using GivingChampion.Application.DTO.CertificateDto;
using GivingChampion.Persistance.Interfaces;
namespace GivingChampion.Application.Services.Certificate;

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _certificateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    // Constructor to initialize repository and mapper
    public CertificateService(
        ICertificateRepository certificateRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _certificateRepository = certificateRepository ?? throw new ArgumentNullException(nameof(certificateRepository));
        _unitOfWork = unitOfWork;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    // Create a new certificate
    public async Task<CertificateReadAllDto?> CreateAsync(CertificateCreateDto dto, CancellationToken cancellationToken = default)
    {
        // Step 1: Check if the volunteer exists
        var volunteerExists = await _certificateRepository.VolunteerExistsAsync(dto.VolunteerId, cancellationToken);
        if (!volunteerExists)
        {
            return null; // Return null if the volunteer does not exist
        }

        // Step 2: Map the CertificateCreateDto to Certificate entity
        var certificate = _mapper.Map<Domain.Entities.Certificate>(dto);


        // Step 4: Add the certificate to the repository
        await _certificateRepository.AddAsync(certificate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Step 5: Return the created certificate as a DTO
        return _mapper.Map<CertificateReadAllDto>(certificate);
    }

    // Check if a volunteer exists by their ID
    public async Task<bool> CheckVolunteerExists(Guid volunteerId, CancellationToken cancellationToken = default)
    {
        // Check if volunteer exists in the repository
        return await _certificateRepository.VolunteerExistsAsync(volunteerId, cancellationToken);
    }
    public async Task<List<CertificateReadAllDto>> GetCertificatesByIdAsync(
     Guid userId,
     CancellationToken cancellationToken = default)
    {
        var certificates = await _certificateRepository.GetCertificateByIdAsync(userId, cancellationToken);

        return _mapper.Map<List<CertificateReadAllDto>>(certificates);
    }
}
