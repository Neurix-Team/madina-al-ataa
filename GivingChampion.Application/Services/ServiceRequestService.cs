using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.ServiceRequestService;
using GivingChampion.Application.DTO.ServiceRequestDto;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Application.DTO.ActivityDto;
using GivingChampion.Application.Interfaces;
using Microsoft.AspNetCore.Http;
namespace GivingChampion.Application.Services
{
    public class ServiceRequestService : BaseService, IServiceRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ServiceRequest> _serviceRequestRepository;
        private readonly IActivityService _activityService;
        private readonly IMapper _mapper;

        public ServiceRequestService(
         IUnitOfWork unitOfWork,
         IMapper mapper,
         IActivityService activityService,
         IHttpContextAccessor httpContextAccessor)
         : base(httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _serviceRequestRepository = unitOfWork.Repository<ServiceRequest>();
            _mapper = mapper;
            _activityService = activityService;
        }

        // Gets all service requests and maps them from Entity list to DTO list.
        public async Task<Result<PagedList<ServiceRequestDto>>> GetAllAsync(PageParameters pageParameters)
        {
            var serviceRequests = await _serviceRequestRepository.GetAllAsync(pageParameters);

            var dtos = _mapper.MapPagedList<ServiceRequest, ServiceRequestDto>(serviceRequests);

            return Result<PagedList<ServiceRequestDto>>.Success(dtos);
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
        // Gets all service requests filtered by specific status
        public async Task<Result<PagedList<ServiceRequestDto>>> GetByStatusAsync(
        RequestStatus status,
        PageParameters pageParameters)
        {
            var filtered = await PagedList<ServiceRequest>.CreateAsync(
                _serviceRequestRepository.Query().Where(serviceRequest => serviceRequest.Status == status),
                pageParameters.PageNumber,
                pageParameters.PageSize);

            var dtos = _mapper.MapPagedList<ServiceRequest, ServiceRequestDto>(filtered);

            return Result<PagedList<ServiceRequestDto>>.Success(dtos);
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

            await _activityService.AddAsync(new CreateActivityDto
            {
                UserId = UserId,
                EntityId = serviceRequest.Id,
                EntityType = ActivityEntityType.Request,
                Action = ActivityAction.RequestCreated,
                Description = $"Service request '{serviceRequest.Title}' created."
            });

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

            await _activityService.AddAsync(new CreateActivityDto
            {
                UserId = UserId,
                EntityId = serviceRequest.Id,
                EntityType = ActivityEntityType.Request,
                Action = ActivityAction.RequestUpdated,
                Description = $"Service request '{serviceRequest.Title}' updated."
            });

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
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            _serviceRequestRepository.Update(serviceRequest);
            await _unitOfWork.SaveChangesAsync();

            await _activityService.AddAsync(new CreateActivityDto
            {
                UserId = UserId,
                EntityId = serviceRequest.Id,
                EntityType = ActivityEntityType.Request,
                Action = ActivityAction.RequestDeleted,
                Description = $"Service request '{serviceRequest.Title}' deleted."
            });

            return true;
        }
    }
}
