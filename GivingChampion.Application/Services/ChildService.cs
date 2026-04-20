using AutoMapper;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Common.DTO.Child;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<Result<ChildDto>> CreateChildAsync(CreateChildDto dto)
        {
            var parentId = Guid.Parse(Thread.CurrentPrincipal?.Identity?.Name ?? ""); // Better to get from Claims in controller and pass down

            // In real usage, parentId should be passed from controller
            // For now, we'll assume it's handled in controller and passed

            // TODO: In production, inject ICurrentUserService or pass parentId as parameter

            var child = _mapper.Map<Child>(dto);
            child.ParentId = parentId;   // Will be set from controller

            // Note: The child's ApplicationUser should be created BEFORE or together with Child
            // For simplicity, we assume it's created in this service (you can adjust)

            await _childRepository.CreateAsync(child);

            var childDto = _mapper.Map<ChildDto>(child);
            return Result<ChildDto>.Success(childDto);
        }

        public async Task<Result<List<ChildDto>>> GetMyChildrenAsync(Guid parentId)
        {
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

        /// <summary>
        /// Admin approves child → activates child's user account
        /// </summary>
        public async Task<Result> ApproveChildAsync(Guid childId, Guid approvedById)
        {
            var child = await _childRepository.GetByIdAsync(childId);
            if (child == null)
                return Result.Failure("Child not found");

            if (child.Status != ObjectStatus.Pending)
                return Result.Failure("Child is not in pending status");

            // Activate the child's user account
            if (child.User != null)
            {
                child.User.EmailConfirmed = true;
                child.User.LockoutEnabled = false;
                await _userManager.UpdateAsync(child.User);
            }

            await _childRepository.ApproveAsync(childId, approvedById);

            _logger.LogInformation("Child approved by admin {AdminId}. ChildId: {ChildId}", approvedById, childId);

            return Result.Success();
        }

        public async Task<Result> RejectChildAsync(RejectChildDto dto, Guid rejectedById)
        {
            var child = await _childRepository.GetByIdAsync(dto.ChildId);
            if (child == null)
                return Result.Failure("Child not found");

            await _childRepository.RejectAsync(dto.ChildId, dto.RejectionReason, rejectedById);

            _logger.LogWarning("Child rejected by admin {AdminId}. ChildId: {ChildId}, Reason: {Reason}",
                rejectedById, dto.ChildId, dto.RejectionReason);

            return Result.Success();
        }

        public async Task<Result> SoftDeleteChildAsync(Guid childId)
        {
            await _childRepository.SoftDeleteAsync(childId);
            return Result.Success();
        }
    }
}