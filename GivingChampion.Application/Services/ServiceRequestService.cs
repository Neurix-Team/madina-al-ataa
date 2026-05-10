using AutoMapper;
using GivingChampion.API.Repositories;
using GivingChampion.Application.DTO.ActivityDto;
using GivingChampion.Application.DTO.ServiceRequestDto;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Interfaces.ServiceRequestService;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Application.Services
{
    public class ServiceRequestService : BaseService, IServiceRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ServiceRequest> _serviceRequestRepository;
        private readonly IGenericRepository<VolunteerOrder> _volunteerOrderRepository;
        private readonly IActivityService _activityService;
        private readonly IMapper _mapper;
        private readonly ILevelRepository _levelRepository;

        public ServiceRequestService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IActivityService activityService,
            ILevelRepository levelRepository,
            IGenericRepository<VolunteerOrder> volunteerOrderRepository,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _serviceRequestRepository = unitOfWork.Repository<ServiceRequest>();
            _volunteerOrderRepository = volunteerOrderRepository;
            _mapper = mapper;
            _activityService = activityService;
            _levelRepository = levelRepository;
        }

        public async Task<Result<PagedList<ServiceRequestDto>>> GetAllAsync(PageParameters pageParameters)
        {

            var query = _serviceRequestRepository.Query()
                .Include(sr => sr.RequiredLevel)
                .Where(sr => !sr.IsDeleted);

            var serviceRequests = await PagedList<ServiceRequest>.CreateAsync(
                query,
                pageParameters.PageNumber,
                pageParameters.PageSize);

            var dtos = _mapper.MapPagedList<ServiceRequest, ServiceRequestDto>(serviceRequests);

            return Result<PagedList<ServiceRequestDto>>.Success(dtos);
        }

        public async Task<ServiceRequestDto?> GetByIdAsync(Guid id)
        {

            if (id == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            var serviceRequest = await _serviceRequestRepository.Query()
                .Include(sr => sr.RequiredLevel)
                .FirstOrDefaultAsync(sr => sr.Id == id && !sr.IsDeleted);

            if (serviceRequest == null)
                throw new NotFoundException("Service request not found.");

            return _mapper.Map<ServiceRequestDto>(serviceRequest);
        }

        public async Task<List<ServiceRequestDto>> GetApprovedRequestsAsync()
        {
            var approvedRequests = await _serviceRequestRepository.Query()
                .Include(sr => sr.RequiredLevel)
                .Where(sr =>
                    sr.Status == RequestStatus.Approved &&
                    !sr.IsDeleted)
                .ToListAsync();

            return _mapper.Map<List<ServiceRequestDto>>(approvedRequests);
        }

        public async Task<Result<PagedList<ServiceRequestDto>>> GetByStatusAsync(
            RequestStatus status,
            PageParameters pageParameters)
        {

            var query = _serviceRequestRepository.Query()
                .Include(sr => sr.RequiredLevel)
                .Where(sr =>
                    sr.Status == status &&
                    !sr.IsDeleted);

            var filtered = await PagedList<ServiceRequest>.CreateAsync(
                query,
                pageParameters.PageNumber,
                pageParameters.PageSize);

            var dtos = _mapper.MapPagedList<ServiceRequest, ServiceRequestDto>(filtered);

            return Result<PagedList<ServiceRequestDto>>.Success(dtos);
        }

        public async Task<List<ServiceRequestDto>> GetByPartnerIdAsync(Guid partnerId)
        {
            if (partnerId == Guid.Empty)
                throw new BadRequestException("Partner ID is required.");

            var serviceRequests = await _serviceRequestRepository.Query()
                .Include(sr => sr.RequiredLevel)
                .Where(sr =>
                    sr.PartnerId == partnerId &&
                    !sr.IsDeleted)
                .ToListAsync();

            return _mapper.Map<List<ServiceRequestDto>>(serviceRequests);
        }

        public async Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto)
        {

            if (dto == null)
                throw new BadRequestException("Service request create data is required.");

            if (dto.RequiredLevelId == Guid.Empty)
                throw new BadRequestException("Required level is required.");

            var level = await _levelRepository.GetByIdAsync(dto.RequiredLevelId);

            if (level == null)
                throw new NotFoundException("Required level not found.");

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

            var createdServiceRequest = await _serviceRequestRepository.Query()
                .Include(sr => sr.RequiredLevel)
                .FirstOrDefaultAsync(sr => sr.Id == serviceRequest.Id && !sr.IsDeleted);

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

            if (serviceRequest == null || serviceRequest.IsDeleted)
                throw new NotFoundException("Service request not found.");

            if (serviceRequest.Status is RequestStatus.Completed or RequestStatus.InProgress)
            {
                throw new ConflictException(
                    $"Cannot update this service request because its current status is {serviceRequest.Status}."
                );
            }

            if (dto.RequiredLevelId == Guid.Empty)
                throw new BadRequestException("Required level is required.");

            var level = await _levelRepository.GetByIdAsync(dto.RequiredLevelId);

            if (level == null)
                throw new NotFoundException("Required level not found.");

            var currentOrdersCount = await _volunteerOrderRepository.CountAsync(o =>
                o.ServiceRequestId == id &&
                !o.IsDeleted);

            if (dto.MaxOrders < currentOrdersCount)
            {
                throw new BadRequestException(
                    $"Max orders cannot be less than current active orders count ({currentOrdersCount})."
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

            if (serviceRequest == null || serviceRequest.IsDeleted)
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
