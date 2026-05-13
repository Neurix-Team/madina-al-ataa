using AutoMapper;
using GivingChampion.Application.Interfaces.Certificate;
using GivingChampion.Application.DTO.CertificateDto;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;
using CertificateEntity = GivingChampion.Domain.Entities.Certificate;

namespace GivingChampion.Application.Services
{
    public class CertificateService : BaseService, ICertificateService
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CertificateService(
            ICertificateRepository certificateRepository,
            IVolunteerRepository volunteerRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _certificateRepository = certificateRepository ?? throw new ArgumentNullException(nameof(certificateRepository));
            _volunteerRepository = volunteerRepository;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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

        public async Task<Result<PagedList<CertificateReadAllDto>>> GetMyCertificatesAsync(
            PageParameters pageParameters,
            CancellationToken cancellationToken = default)
        {
            var volunteer = await _volunteerRepository.GetByUserIdAsync(UserId);

            var certificates = await _certificateRepository.GetCertificateByIdAsync(
                volunteer.Id,
                pageParameters,
                cancellationToken);

            var dtos = _mapper.MapPagedList<CertificateEntity, CertificateReadAllDto>(certificates);

            return Result<PagedList<CertificateReadAllDto>>.Success(dtos);
        }

        public async Task<Result<PagedList<CertificateReadAllDto>>> GetCertificatesByIdAsync(
            Guid userId,
            PageParameters pageParameters,
            CancellationToken cancellationToken = default)
        {
            var volunteer = await _volunteerRepository.GetByUserIdAsync(userId);

            var certificates = await _certificateRepository.GetCertificateByIdAsync(
                volunteer.Id,
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