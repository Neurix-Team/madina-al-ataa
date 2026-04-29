using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.ServiceRequestService;
using GivingChampion.Common.DTO.ServiceRequestDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using GivingChampion.Common.Enums;
namespace GivingChampion.Application.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        #region Fields

        // Repository used to access service request data from the persistence layer.
        private readonly IServiceRequestRepository _serviceRequestRepository;

        // AutoMapper used to map between DTOs and Entities.
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        // Injects the service request repository and AutoMapper.
        public ServiceRequestService(
            IServiceRequestRepository serviceRequestRepository,
            IMapper mapper)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _mapper = mapper;
        }

        #endregion

        #region Get Methods

        // Gets all service requests and maps them from Entity list to DTO list.
        public async Task<List<ServiceRequestDto>> GetAllAsync()
        {
            var serviceRequests = await _serviceRequestRepository.GetAllAsync();

            return _mapper.Map<List<ServiceRequestDto>>(serviceRequests);
        }

        // Gets a single service request by id.
        // Returns null if the service request does not exist.
        public async Task<ServiceRequestDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);

            if (serviceRequest == null)
                return null;

            if (serviceRequest.IsDeleted)
                throw new NotFoundException($"Service request with ID {id} was not found.");

            return _mapper.Map<ServiceRequestDto>(serviceRequest);
        }

        //// Gets all service requests with Pending status.
        //public async Task<List<ServiceRequestDto>> GetPendingAsync()
        //{
        //    var serviceRequests = await _serviceRequestRepository.GetPendingAsync();

        //    return _mapper.Map<List<ServiceRequestDto>>(serviceRequests);
        //}

        public async Task<List<ServiceRequestDto>> GetApprovedRequestsAsync()
        {
            var approvedRequests = await _serviceRequestRepository.GetApprovedRequestsAsync();

            return _mapper.Map<List<ServiceRequestDto>>(approvedRequests);
        }
        // Gets all service requests filtered by specific status
        public async Task<List<ServiceRequestDto>> GetByStatusAsync(RequestStatus status)
        {
            // fetch all and filter in memory using repository methods if no direct repo method exists
            var all = await _serviceRequestRepository.GetAllAsync();
            var filtered = all.Where(sr => sr.Status == status).ToList();

            return _mapper.Map<List<ServiceRequestDto>>(filtered);
        }

        // Gets all service requests related to a specific partner.
        public async Task<List<ServiceRequestDto>> GetByPartnerIdAsync(Guid partnerId)
        {
            if (partnerId == Guid.Empty)
                throw new BadRequestException("Partner ID is required.");

            var serviceRequests = await _serviceRequestRepository.GetByPartnerIdAsync(partnerId);

            return _mapper.Map<List<ServiceRequestDto>>(serviceRequests);
        }

        #endregion

        #region Create Method

        // Creates a new service request.
        // Maps CreateServiceRequestDto to ServiceRequest entity, saves it, then returns it as ServiceRequestDto.
        public async Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Service request create data is required.");

            var serviceRequest = _mapper.Map<ServiceRequest>(dto);
            serviceRequest.Status = RequestStatus.Approved;
            await _serviceRequestRepository.AddAsync(serviceRequest);

            await _serviceRequestRepository.SaveChangesAsync();

            // Reload the created entity to include related data such as Partner.
            var createdServiceRequest = await _serviceRequestRepository.GetByIdAsync(serviceRequest.Id);

            if (createdServiceRequest == null)
                throw new InvalidOperationException("Service request was created but could not be retrieved.");

            return _mapper.Map<ServiceRequestDto>(createdServiceRequest);
        }

        #endregion

        #region Update Method

        // Updates an existing service request.
        // Returns false if the service request does not exist.
        public async Task<bool> UpdateAsync(Guid id, UpdateServiceRequestDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            if (dto == null)
                throw new BadRequestException("Service request update data is required.");

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);

            if (serviceRequest == null)
                return false;

            // Maps the new values from the DTO into the existing entity.
            _mapper.Map(dto, serviceRequest);

            _serviceRequestRepository.UpdateAsync(serviceRequest);
            await _serviceRequestRepository.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Delete Method

        // Soft deletes an existing service request.
        // Returns false if the service request does not exist.
        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);

            if (serviceRequest == null)
                return false;

            _serviceRequestRepository.SoftDeleteAsync(serviceRequest);
            await _serviceRequestRepository.SaveChangesAsync();

            return true;
        }

        #endregion
    }
}