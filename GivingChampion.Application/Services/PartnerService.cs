using AutoMapper;
using GivingChampion.Application.Interfaces.Partner;
using GivingChampion.Common.DTO.Partner;
using GivingChampion.Common.DTO.PartnerDto;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly IPartnerRepository _partnerRepository;
        private readonly IMapper _mapper;

        public PartnerService(IPartnerRepository partnerRepository, IMapper mapper)
        {
            _partnerRepository = partnerRepository;
            _mapper = mapper;
        }

        #region Query Methods

        public async Task<Result<PagedList<PartnerDto>>> GetAllAsync(PageParameters pageParameters)
        {
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

        #endregion

        #region Command Methods

        public async Task<PartnerDto> CreateAsync(CreatePartnerDto dto)
        {
            var partner = _mapper.Map<Partner>(dto);

            await _partnerRepository.AddAsync(partner);
            await _partnerRepository.SaveChangesAsync();

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

            await _partnerRepository.SaveChangesAsync();

            return _mapper.Map<PartnerDto>(partner);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var partner = await _partnerRepository.GetByIdAsync(id);

            if (partner == null)
                return false;

            _partnerRepository.SoftDelete(partner);

            await _partnerRepository.SaveChangesAsync();

            return true;
        }

        #endregion
    }
}