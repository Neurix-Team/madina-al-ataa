using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.Partner;
using GivingChampion.Common.DTO.Partner;
using GivingChampion.Common.DTO.PartnerDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly IPartnerRepository _partnerRepository;
        private readonly IMapper _mapper;

        public PartnerService(
            IPartnerRepository partnerRepository,
            IMapper mapper)
        {
            _partnerRepository = partnerRepository;
            _mapper = mapper;
        }

        public async Task<List<PartnerDto>> GetAllAsync()
        {
            var partners = await _partnerRepository.GetAllAsync();

            return _mapper.Map<List<PartnerDto>>(partners);
        }

        public async Task<PartnerDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Partner ID is required.");

            var partner = await _partnerRepository.GetByIdAsync(id);

            if (partner == null)
                throw new NotFoundException($"Partner with ID {id} was not found.");

            if (partner.IsDeleted)
                throw new NotFoundException($"Partner with ID {id} was not found.");

            return _mapper.Map<PartnerDto>(partner);
        }

        public async Task<PartnerDto> CreateAsync(CreatePartnerDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Partner create data is required.");

            var partner = _mapper.Map<Partner>(dto);

            await _partnerRepository.AddAsync(partner);
            await _partnerRepository.SaveChangesAsync();

            return _mapper.Map<PartnerDto>(partner);
        }

        public async Task<PartnerDto?> UpdateAsync(Guid id, UpdatePartnerDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Partner ID is required.");

            if (dto == null)
                throw new BadRequestException("Partner update data is required.");

            var partner = await _partnerRepository.GetByIdAsync(id);

            if (partner == null)
                throw new NotFoundException($"Partner with ID {id} was not found.");

            if (partner.IsDeleted)
                throw new BadRequestException("Cannot update a deleted partner.");

            _mapper.Map(dto, partner);

            partner.UpdatedAt = DateTime.UtcNow;

            _partnerRepository.Update(partner);

            await _partnerRepository.SaveChangesAsync();

            return _mapper.Map<PartnerDto>(partner);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Partner ID is required.");

            var partner = await _partnerRepository.GetByIdAsync(id);

            if (partner == null)
                throw new NotFoundException($"Partner with ID {id} was not found.");

            if (partner.IsDeleted)
                throw new BadRequestException("Partner is already deleted.");

            _partnerRepository.SoftDelete(partner);

            await _partnerRepository.SaveChangesAsync();

            return true;
        }
    }
}