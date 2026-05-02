using AutoMapper;
using GivingChampion.Application.Interfaces.Certificate;
using GivingChampion.Common.DTO.CertificateDto;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Persistance.Interfaces;
using CertificateEntity = GivingChampion.Domain.Entities.Certificate;

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

        public async Task<CertificateReadAllDto?> CreateAsync(
            CertificateCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var volunteerExists = await _certificateRepository.VolunteerExistsAsync(
                dto.VolunteerId,
                cancellationToken);

            if (!volunteerExists)
                return null;

            var certificate = _mapper.Map<CertificateEntity>(dto);


        // Step 4: Add the certificate to the repository
        await _certificateRepository.AddAsync(certificate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CertificateReadAllDto>(certificate);
        }

        public async Task<bool> CheckVolunteerExists(
            Guid volunteerId,
            CancellationToken cancellationToken = default)
        {
            return await _certificateRepository.VolunteerExistsAsync(
                volunteerId,
                cancellationToken);
        }

        public async Task<Result<PagedList<CertificateReadAllDto>>> GetCertificatesByIdAsync(
            Guid userId,
            PageParameters pageParameters,
            CancellationToken cancellationToken = default)
        {
            var certificates = await _certificateRepository.GetCertificateByIdAsync(
                userId,
                pageParameters,
                cancellationToken);

            var dtos = _mapper.MapPagedList<CertificateEntity, CertificateReadAllDto>(certificates);

            return Result<PagedList<CertificateReadAllDto>>.Success(dtos);
        }

        public async Task<Result<PagedList<CertificateReadAllDto>>> GetAllAsync(
            PageParameters pageParameters,
            CancellationToken cancellationToken = default)
        {
            var certificates = await _certificateRepository.GetAllAsync(
                pageParameters,
                cancellationToken);

            var dtos = _mapper.MapPagedList<CertificateEntity, CertificateReadAllDto>(certificates);

            return Result<PagedList<CertificateReadAllDto>>.Success(dtos);
        }
    }
}
