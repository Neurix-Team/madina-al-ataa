using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Application.DTO.Child;
using GivingChampion.Application.DTO.User;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GivingChampion.Application.Services
{
    public class ChildService : BaseService, IChildService
    {
        private readonly IChildRepository _childRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ChildService> _logger;

        public ChildService(
            IChildRepository childRepository,
            IUserRepository userRepository,
            IUserService userService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ChildService> logger,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _childRepository = childRepository;
            _userRepository = userRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<ChildDto>> CreateChildAsync(CreateChildDto dto)
        {
            var parentId = UserId;

            if (dto == null)
                throw new BadRequestException("Child data is required.");

            if (parentId == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid parent user token.");

            var parent = await _userRepository.GetByIdAsync(parentId);

            if (parent == null)
                throw new NotFoundException($"Parent user with ID {parentId} was not found.");

            if (parent.IsDeleted)
                throw new BadRequestException("Cannot create child for a deleted parent account.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BadRequestException("Child email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequestException("Child password is required.");

            if (dto.BirthDay == default)
                throw new BadRequestException("Child birthday is required.");

            var childUserResult = await _userService.CreateChildUserAsync(new CreateUserDto
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                BirthDay = dto.BirthDay,
                Password = dto.Password
            });

            if (!childUserResult.Succeeded || childUserResult.Value == null)
                throw new BadRequestException(childUserResult.Error ?? "Failed to create child user.");

            var childUserDto = childUserResult.Value;
            var childUser = await _userRepository.GetByIdAsync(childUserDto.Id);

            if (childUser == null)
                throw new NotFoundException("Failed to retrieve created child user.");

            var child = _mapper.Map<Child>(dto);

            child.ParentId = parentId;
            child.UserId = childUserDto.Id;
            child.Status = ObjectStatus.Pending;

            try
            {
                await _childRepository.CreateAsync(child);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                await _userService.DeleteUserByAdminAsync(childUserDto.Id);
                throw;
            }

            child.User = childUser;

            var childDto = _mapper.Map<ChildDto>(child);

            return Result<ChildDto>.Success(childDto);
        }

        public async Task<Result<List<ChildDto>>> GetMyChildrenAsync()
        {
            var parentId = UserId;

            if (parentId == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid parent user token.");

            var parent = await _userRepository.GetByIdAsync(parentId);

            if (parent == null)
                throw new NotFoundException($"Parent user with ID {parentId} was not found.");

            var children = await _childRepository.GetByParentIdAsync(parentId);

            var dtos = _mapper.Map<List<ChildDto>>(children);

            return Result<List<ChildDto>>.Success(dtos);
        }

        public async Task<Result<List<ChildDto>>> GetPendingApprovalsAsync()
        {
            var pending = await _childRepository.GetPendingApprovalsAsync();

            var dtos = _mapper.Map<List<ChildDto>>(pending);

            return Result<List<ChildDto>>.Success(dtos);
        }

        public async Task<Result> ApproveChildAsync(Guid childId)
        {
            var approvedById = UserId;

            if (childId == Guid.Empty)
                throw new BadRequestException("Child ID is required.");

            if (approvedById == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid admin user token.");

            var child = await _childRepository.GetByIdAsync(childId);

            if (child == null)
                throw new NotFoundException($"Child with ID {childId} was not found.");

            if (child.Status != ObjectStatus.Pending)
                throw new BadRequestException("Child is not in pending status.");

            await _userService.ApproveChildUserAsync(child.UserId);

            await _childRepository.ApproveAsync(childId, approvedById);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Child approved by admin {AdminId}. ChildId: {ChildId}",
                approvedById,
                childId
            );

            return Result.Success();
        }

        public async Task<Result> RejectChildAsync(RejectChildDto dto)
        {
            var rejectedById = UserId;

            if (dto == null)
                throw new BadRequestException("Reject child data is required.");

            if (dto.ChildId == Guid.Empty)
                throw new BadRequestException("Child ID is required.");

            if (rejectedById == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid admin user token.");

            if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                throw new BadRequestException("Rejection reason is required.");

            var child = await _childRepository.GetByIdAsync(dto.ChildId);

            if (child == null)
                throw new NotFoundException($"Child with ID {dto.ChildId} was not found.");

            if (child.Status != ObjectStatus.Pending)
                throw new BadRequestException("Only pending children can be rejected.");

            await _childRepository.RejectAsync(
                dto.ChildId,
                dto.RejectionReason,
                rejectedById
            );
            await _unitOfWork.SaveChangesAsync();

            _logger.LogWarning(
                "Child rejected by admin {AdminId}. ChildId: {ChildId}, Reason: {Reason}",
                rejectedById,
                dto.ChildId,
                dto.RejectionReason
            );

            return Result.Success();
        }

        public async Task<Result> SoftDeleteChildAsync(Guid childId)
        {
            if (childId == Guid.Empty)
                throw new BadRequestException("Child ID is required.");

            var child = await _childRepository.GetByIdAsync(childId);

            if (child == null)
                throw new NotFoundException($"Child with ID {childId} was not found.");

            if (child.IsDeleted)
                throw new BadRequestException("Child is already deleted.");

            await _childRepository.SoftDeleteAsync(childId);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}
