using AutoMapper;
using GivingChampion.Application.Interfaces.Partner;
using GivingChampion.Application.DTO.Partner;
using GivingChampion.Application.DTO.PartnerDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services
{
    public class PartnerService : IPartnerService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Partner> _partnerRepository;
        private readonly IMapper _mapper;

        #region Constructor

        // Constructor to initialize dependencies (PartnerRepository and AutoMapper)
        public PartnerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _partnerRepository = unitOfWork.Repository<Partner>();
            _mapper = mapper;
        }

        #endregion

        #region Query Methods

        #region GetAllPartner

        /// <summary>
        /// Retrieves all partners from the repository and maps them to PartnerDto.
        /// </summary>
        public async Task<List<PartnerDto>> GetAllAsync()
        {
            // Fetching all partners from the repository
            var partners = await _partnerRepository.ListAsync();

            // Mapping the partners to PartnerDto
            return _mapper.Map<List<PartnerDto>>(partners);
        }
        #endregion

        #region GetById
        /// <summary>
        /// Retrieves a partner by ID and maps it to PartnerDto.
        /// Returns null if partner is not found.
        /// </summary>
        public async Task<PartnerDto?> GetByIdAsync(Guid id)
        {
            // Fetching a partner by ID from the repository
            var partner = await _partnerRepository.GetByIdAsync(id);

            // Return null if partner is not found
            if (partner == null)
                return null;

            // Mapping the found partner to PartnerDto
            return _mapper.Map<PartnerDto>(partner);
        }
        #endregion

        #endregion

        #region Command Methods

        #region CreatePartner
        /// <summary>
        /// Creates a new partner in the repository and returns the mapped PartnerDto.
        /// </summary>
        public async Task<PartnerDto> CreateAsync(CreatePartnerDto dto)
        {
            // Mapping the incoming CreatePartnerDto to a Partner entity
            var partner = _mapper.Map<Partner>(dto);

            

            // Adding the partner to the repository
            await _partnerRepository.AddAsync(partner);
            await _unitOfWork.SaveChangesAsync();

            // Returning the newly created partner as a DTO
            return _mapper.Map<PartnerDto>(partner);
        }
        #endregion

        #region UpdatePartner
        /// <summary>
        /// Updates an existing partner by ID and returns the updated PartnerDto.
        /// </summary>
        public async Task<PartnerDto?> UpdateAsync(Guid id, UpdatePartnerDto dto)
        {
            // Fetching the existing partner by ID
            var partner = await _partnerRepository.GetByIdAsync(id);

            // Return null if partner is not found
            if (partner == null)
                return null;

            // Mapping the incoming UpdatePartnerDto to the existing Partner entity
            _mapper.Map(dto, partner);

            partner.UpdatedAt = DateTime.UtcNow;

            // Updating the partner in the repository
            _partnerRepository.Update(partner);

            await _unitOfWork.SaveChangesAsync();

            // Returning the updated partner as a DTO
            return _mapper.Map<PartnerDto>(partner);
        }
        #endregion

        #region  DeletePatner
        /// <summary>
        /// Soft deletes a partner by ID and returns a boolean indicating success.
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            // Fetching the partner by ID
            var partner = await _partnerRepository.GetByIdAsync(id);

            // Return false if partner is not found
            if (partner == null)
                return false;

            partner.IsDeleted = true;
            partner.DeletedAt = DateTime.UtcNow;
            _partnerRepository.Update(partner);

            await _unitOfWork.SaveChangesAsync();

            // Returning true to indicate successful deletion
            return true;
        }
        #endregion

        #endregion
    }
}
