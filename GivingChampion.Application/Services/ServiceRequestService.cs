using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.ServiceRequestService;
using GivingChampion.Common.DTO.ServiceRequestDto;
using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ServiceRequest> _serviceRequestRepository;
        private readonly IMapper _mapper;

        public ServiceRequestService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _serviceRequestRepository = unitOfWork.Repository<ServiceRequest>();
            _mapper = mapper;
        }

        public async Task<List<ServiceRequestDto>> GetAllAsync()
        {
            var serviceRequests = await _serviceRequestRepository.ListAsync();

            return _mapper.Map<List<ServiceRequestDto>>(serviceRequests);
        }

        public async Task<ServiceRequestDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);

            if (serviceRequest == null)
                throw new NotFoundException("Service request not found.");

            return _mapper.Map<ServiceRequestDto>(serviceRequest);
        }

        public async Task<List<ServiceRequestDto>> GetApprovedRequestsAsync()
        {
            var approvedRequests = await _serviceRequestRepository.ListAsync(
                serviceRequest => serviceRequest.Status == RequestStatus.Approved);

            return _mapper.Map<List<ServiceRequestDto>>(approvedRequests);
        }

        public async Task<List<ServiceRequestDto>> GetByStatusAsync(RequestStatus status)
        {
            var filtered = await _serviceRequestRepository.ListAsync(
                serviceRequest => serviceRequest.Status == status);

            return _mapper.Map<List<ServiceRequestDto>>(filtered);
        }

        public async Task<List<ServiceRequestDto>> GetByPartnerIdAsync(Guid partnerId)
        {
            if (partnerId == Guid.Empty)
                throw new BadRequestException("Partner ID is required.");

            var serviceRequests = await _serviceRequestRepository.ListAsync(
                serviceRequest => serviceRequest.PartnerId == partnerId);

            return _mapper.Map<List<ServiceRequestDto>>(serviceRequests);
        }

        public async Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Service request create data is required.");

            var serviceRequest = _mapper.Map<ServiceRequest>(dto);

            serviceRequest.Status = RequestStatus.Approved;
            serviceRequest.CreatedAt = DateTime.UtcNow;

            await _serviceRequestRepository.AddAsync(serviceRequest);
            await _unitOfWork.SaveChangesAsync();

            var createdServiceRequest = await _serviceRequestRepository.GetByIdAsync(serviceRequest.Id);

            if (createdServiceRequest == null)
                throw new InvalidOperationException("Service request was created but could not be retrieved.");

            return _mapper.Map<ServiceRequestDto>(createdServiceRequest);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateServiceRequestDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            if (dto == null)
                throw new BadRequestException("Service request update data is required.");

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);

            if (serviceRequest == null)
                throw new NotFoundException("Service request not found.");

            if (serviceRequest.Status is RequestStatus.Completed or RequestStatus.InProgress)
            {
                throw new ConflictException(
                    $"Cannot update this service request because its current status is {serviceRequest.Status}."
                );
            }

            _mapper.Map(dto, serviceRequest);

            serviceRequest.UpdatedAt = DateTime.UtcNow;

            _serviceRequestRepository.Update(serviceRequest);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);

            if (serviceRequest == null)
                throw new NotFoundException("Service request not found.");

            serviceRequest.IsDeleted = true;
            serviceRequest.DeletedAt = DateTime.UtcNow;
            _serviceRequestRepository.Update(serviceRequest);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
