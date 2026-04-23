using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.Certificate;
using GivingChampion.Common.DTO.CertificateDto;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services.Certificate
{
    public class CertificateService : ICertificateService
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IMapper _mapper;

        public CertificateService(
            ICertificateRepository certificateRepository,
            IMapper mapper)
        {
            _certificateRepository = certificateRepository
                ?? throw new ArgumentNullException(nameof(certificateRepository));

            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CertificateReadAllDto?> CreateAsync(
            CertificateCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto == null)
                throw new BadRequestException("Certificate create data is required.");

            if (dto.VolunteerId == Guid.Empty)
                throw new BadRequestException("Volunteer ID is required.");

            var volunteerExists = await _certificateRepository
                .VolunteerExistsAsync(dto.VolunteerId, cancellationToken);

            if (!volunteerExists)
                throw new NotFoundException($"Volunteer with ID {dto.VolunteerId} was not found.");

            var certificate = _mapper.Map<Domain.Entities.Certificate>(dto);

            await _certificateRepository.AddAsync(certificate, cancellationToken);

            return _mapper.Map<CertificateReadAllDto>(certificate);
        }

        public async Task<bool> CheckVolunteerExists(
            Guid volunteerId,
            CancellationToken cancellationToken = default)
        {
            if (volunteerId == Guid.Empty)
                throw new BadRequestException("Volunteer ID is required.");

            return await _certificateRepository.VolunteerExistsAsync(
                volunteerId,
                cancellationToken);
        }

        // Get all certificates for a specific user
        public async Task<List<Certificate>> GetCertificateByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _certificateRepository.GetCertificateByIdAsync(userId, cancellationToken);
        }
    }
    }