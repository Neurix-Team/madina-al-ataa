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
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IMapper _mapper;

        public ServiceRequestService(
            IServiceRequestRepository serviceRequestRepository,
            IMapper mapper)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _mapper = mapper;
        }

        public async Task<List<ServiceRequestDto>> GetAllAsync()
        {
            var serviceRequests = await _serviceRequestRepository.GetAllAsync();

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
            var approvedRequests = await _serviceRequestRepository.GetApprovedRequestsAsync();

            return _mapper.Map<List<ServiceRequestDto>>(approvedRequests);
        }

        public async Task<List<ServiceRequestDto>> GetByStatusAsync(RequestStatus status)
        {
            var all = await _serviceRequestRepository.GetAllAsync();

            var filtered = all
                .Where(sr => sr.Status == status)
                .ToList();

            return _mapper.Map<List<ServiceRequestDto>>(filtered);
        }

        public async Task<List<ServiceRequestDto>> GetByPartnerIdAsync(Guid partnerId)
        {
            if (partnerId == Guid.Empty)
                throw new BadRequestException("Partner ID is required.");

            var serviceRequests = await _serviceRequestRepository.GetByPartnerIdAsync(partnerId);

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
            await _serviceRequestRepository.SaveChangesAsync();

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

            await _serviceRequestRepository.UpdateAsync(serviceRequest);
            await _serviceRequestRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);

            if (serviceRequest == null)
                throw new NotFoundException("Service request not found.");

            await _serviceRequestRepository.SoftDeleteAsync(serviceRequest);
            await _serviceRequestRepository.SaveChangesAsync();

            return true;
        }
    }
}