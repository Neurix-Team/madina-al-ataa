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

namespace GivingChampion.Application.Services
{
    public class ServiceRequestService : BaseService, IServiceRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IGenericRepository<VolunteerOrder> _volunteerOrderRepository;
        private readonly IMapper _mapper;
        private readonly ILevelRepository _levelRepository;

        public ServiceRequestService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IActivityService activityService,
            ILevelRepository levelRepository,
            IServiceRequestRepository serviceRequestRepository,
            IGenericRepository<VolunteerOrder> volunteerOrderRepository,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor, activityService)
        {
            _unitOfWork = unitOfWork;
            _serviceRequestRepository = serviceRequestRepository;
            _volunteerOrderRepository = volunteerOrderRepository;
            _mapper = mapper;
            _levelRepository = levelRepository;
        }

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

        public async Task<Result<PagedList<ServiceRequestDto>>> GetApprovedRequestsAsync(
      PageParameters pageParameters)
        {
            var approvedRequests = await _serviceRequestRepository.GetApprovedRequestsAsync(pageParameters);

            var dtos = _mapper.MapPagedList<ServiceRequest, ServiceRequestDto>(approvedRequests);

            return Result<PagedList<ServiceRequestDto>>.Success(dtos);
        }

        public async Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Service request create data is required.");

            await ValidateRequiredLevelAsync(dto.RequiredLevelId);

            var serviceRequest = _mapper.Map<ServiceRequest>(dto);

            if (serviceRequest.Id == Guid.Empty)
                serviceRequest.Id = Guid.NewGuid();

            serviceRequest.Status = RequestStatus.Approved;
            serviceRequest.CreatedAt = DateTime.UtcNow;
            serviceRequest.IsDeleted = false;

            await _serviceRequestRepository.AddAsync(serviceRequest);
            await _unitOfWork.SaveChangesAsync();

            await AddActivityAsync(
                serviceRequest.Id,
                ActivityEntityType.Request,
                ActivityAction.RequestCreated,
                $"Service request '{serviceRequest.Title}' created.");

            return _mapper.Map<ServiceRequestDto>(serviceRequest);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateServiceRequestDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            if (dto == null)
                throw new BadRequestException("Service request update data is required.");

            var serviceRequest = await _serviceRequestRepository.GetByIdForUpdateAsync(id);

            if (serviceRequest == null)
                throw new NotFoundException("Service request not found.");

            if (serviceRequest.Status is RequestStatus.Completed or RequestStatus.InProgress)
            {
                throw new ConflictException(
                    $"Cannot update this service request because its current status is {serviceRequest.Status}."
                );
            }

            await ValidateRequiredLevelAsync(dto.RequiredLevelId);

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

            await _serviceRequestRepository.UpdateAsync(serviceRequest);
            await _unitOfWork.SaveChangesAsync();

            await AddActivityAsync(
      serviceRequest.Id,
      ActivityEntityType.Request,
      ActivityAction.RequestUpdated,
      $"Service request '{serviceRequest.Title}' updated.");

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            var serviceRequest = await _serviceRequestRepository.GetByIdForUpdateAsync(id);

            if (serviceRequest == null)
                throw new NotFoundException("Service request not found.");

            await _serviceRequestRepository.SoftDeleteAsync(serviceRequest);
            await _unitOfWork.SaveChangesAsync();

            await AddActivityAsync(
    serviceRequest.Id,
    ActivityEntityType.Request,
    ActivityAction.RequestDeleted,
    $"Service request '{serviceRequest.Title}' deleted.");

            return true;
        }

        private async Task ValidateRequiredLevelAsync(Guid requiredLevelId)
        {
            if (requiredLevelId == Guid.Empty)
                throw new BadRequestException("Required level is required.");

            var level = await _levelRepository.GetByIdAsync(requiredLevelId);

            if (level == null)
                throw new NotFoundException("Required level not found.");
        }

        
        }
    }
