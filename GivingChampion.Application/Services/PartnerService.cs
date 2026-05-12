using AutoMapper;
using GivingChampion.Application.Interfaces.Partner;
using GivingChampion.Application.DTO.Partner;
using GivingChampion.Application.DTO.PartnerDto;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.DTO.ActivityDto;
using GivingChampion.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace GivingChampion.Application.Services
{
    public class PartnerService : BaseService, IPartnerService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Partner> _partnerRepository;
        private readonly IMapper _mapper;
        private readonly IActivityService _activityService;


        // Constructor to initialize dependencies (PartnerRepository and AutoMapper)
        public PartnerService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IMapper mapper, IActivityService activityService) : base(httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _partnerRepository = unitOfWork.Repository<Partner>();
            _mapper = mapper;
            _activityService = activityService;
        }


        public async Task<Result<PagedList<PartnerDto>>> GetAllAsync(PageParameters pageParameters)
        {
            // Fetching all partners from the repository
            var partners = await _partnerRepository.GetAllAsync(pageParameters);

            var dtos = _mapper.MapPagedList<Partner, PartnerDto>(partners);

            return Result<PagedList<PartnerDto>>.Success(dtos);
        }

        public async Task<PartnerDto?> GetByIdAsync(Guid id)
        {
            var partner = await _partnerRepository.GetByIdAsync(id);

            if (partner == null)
                return null;

            return _mapper.Map<PartnerDto>(partner);
        }


        public async Task<PartnerDto> CreateAsync(CreatePartnerDto dto)
        {
            var partner = _mapper.Map<Partner>(dto);

            await _partnerRepository.AddAsync(partner);
            await _unitOfWork.SaveChangesAsync();

            await AddActivityAsync(partner.Id, ActivityEntityType.Partner, ActivityAction.PartnerCreated, $"Created partner {partner.OrgName}");

            return _mapper.Map<PartnerDto>(partner);
        }

        public async Task<PartnerDto?> UpdateAsync(Guid id, UpdatePartnerDto dto)
        {
            var partner = await _partnerRepository.GetByIdAsync(id);

            if (partner == null)
                return null;

            _mapper.Map(dto, partner);

            partner.UpdatedAt = DateTime.UtcNow;

            _partnerRepository.Update(partner);

            await AddActivityAsync(partner.Id, ActivityEntityType.Partner, ActivityAction.PartnerUpdated, $"Updated partner {partner.OrgName}");

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PartnerDto>(partner);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var partner = await _partnerRepository.GetByIdAsync(id);

            if (partner == null)
                return false;

            partner.IsDeleted = true;
            partner.DeletedAt = DateTime.UtcNow;
            _partnerRepository.Update(partner);

            await AddActivityAsync(partner.Id, ActivityEntityType.Partner, ActivityAction.PartnerDeleted, $"Deleted partner {partner.OrgName}");

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

    }
}
