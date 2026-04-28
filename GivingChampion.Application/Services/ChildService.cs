using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Common.DTO.Child;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GivingChampion.Application.Services
{
    public class ChildService : IChildService
    {
        private readonly IChildRepository _childRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<ChildService> _logger;

        public ChildService(
            IChildRepository childRepository,
            IUserRepository userRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ILogger<ChildService> logger)
        {
            _childRepository = childRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<ChildDto>> CreateChildAsync(CreateChildDto dto, Guid parentId)
        {
            if (dto == null)
                throw new BadRequestException("Child data is required.");

            if (parentId == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid parent user token.");

            var parent = await _userRepository.GetByIdAsync(parentId);

            if (parent == null)
                throw new NotFoundException($"Parent user with ID {parentId} was not found.");

            if (parent.IsDeleted)
                throw new BadRequestException("Cannot create child for a deleted parent account.");

            var child = _mapper.Map<Child>(dto);

            child.ParentId = parentId;
            child.Status = ObjectStatus.Pending;

            await _childRepository.CreateAsync(child);

            var childDto = _mapper.Map<ChildDto>(child);

            return Result<ChildDto>.Success(childDto);
        }

        public async Task<Result<List<ChildDto>>> GetMyChildrenAsync(Guid parentId)
        {
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

        public async Task<Result> ApproveChildAsync(Guid childId, Guid approvedById)
        {
            if (childId == Guid.Empty)
                throw new BadRequestException("Child ID is required.");

            if (approvedById == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid admin user token.");

            var child = await _childRepository.GetByIdAsync(childId);

            if (child == null)
                throw new NotFoundException($"Child with ID {childId} was not found.");

            if (child.Status != ObjectStatus.Pending)
                throw new BadRequestException("Child is not in pending status.");

            if (child.User != null)
            {
                child.User.EmailConfirmed = true;
                child.User.LockoutEnabled = false;

                var updateResult = await _userManager.UpdateAsync(child.User);

                if (!updateResult.Succeeded)
                {
                    var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                    throw new BadRequestException(errors);
                }
            }

            await _childRepository.ApproveAsync(childId, approvedById);

            _logger.LogInformation(
                "Child approved by admin {AdminId}. ChildId: {ChildId}",
                approvedById,
                childId
            );

            return Result.Success();
        }

        public async Task<Result> RejectChildAsync(RejectChildDto dto, Guid rejectedById)
        {
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

            return Result.Success();
        }
    }
}