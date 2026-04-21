using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Common.DTO.User;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using GivingChampion.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GivingChampion.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUserRepository _userRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IUserRepository userRepository,
            IDonorRepository donorRepository,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _donorRepository = donorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // ====================== READ OPERATIONS ======================
        public async Task<PagedList<GetUserDto>> GetAllUsersAsync(PageParameters pageParameters, string? search = null)
        {
            var users = await _userRepository.GetAllAsync(pageParameters, search);

            // Map entities to DTOs using AutoMapper
            return _mapper.MapPagedList<ApplicationUser, GetUserDto>(users);
        }

        public async Task<GetUserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            var dto = _mapper.Map<GetUserDto>(user);
            dto.Roles = roles.ToList();        // Roles must be set manually

            return dto;
        }

        public async Task<GetUserDto?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            var dto = _mapper.Map<GetUserDto>(user);
            dto.Roles = roles.ToList();

            return dto;
        }

        // ====================== CREATE OPERATIONS ======================
        public async Task<Result<GetUserDto>> CreateUserAsync(CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return Result<GetUserDto>.Failure("Email and Password are required.");

            if (await _userRepository.ExistsByEmailAsync(dto.Email))
                return Result<GetUserDto>.Failure("User with this email already exists.");

            // Use AutoMapper to create ApplicationUser from DTO
            var user = _mapper.Map<ApplicationUser>(dto);
            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return Result<GetUserDto>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));

            // Assign default role
            await _userManager.AddToRolesAsync(user, new[] { "User", "Volunteer", "Donor" });

            var donor = new Donor
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TotalDonated = 0,
                PreferedCategory = 0,           // or default value
                CreatedAt = DateTime.UtcNow
            };

            await _donorRepository.CreateAsync(donor);

            _logger.LogInformation("New user created: {Email}", dto.Email);

            return await GetUserByIdAsync(user.Id) is var created
                ? Result<GetUserDto>.Success(created)
                : Result<GetUserDto>.Failure("Failed to retrieve created user");
        }

        public async Task<Result<GetUserDto>> CreateUserByAdminAsync(CreateUserDto dto)
        {
            // Can be extended later with admin-specific logic (e.g. auto-confirm email)
            return await CreateUserAsync(dto);
        }

        public async Task<Result<GetUserDto>> CreateAdminUserAsync(CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return Result<GetUserDto>.Failure("Email and Password are required.");

            if (await _userRepository.ExistsByEmailAsync(dto.Email))
                return Result<GetUserDto>.Failure("User already exists.");

            var user = _mapper.Map<ApplicationUser>(dto);
            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return Result<GetUserDto>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRolesAsync(user, new[] { "Admin", "User", "Volunteer", "Donor" });

            var donor = new Donor
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TotalDonated = 0,
                PreferedCategory = 0,           // or default value
                CreatedAt = DateTime.UtcNow
            };

            await _donorRepository.CreateAsync(donor);

            _logger.LogInformation("Admin user created: {Email}", dto.Email);

            return await GetUserByIdAsync(user.Id) is var created
                ? Result<GetUserDto>.Success(created)
                : Result<GetUserDto>.Failure("Failed to retrieve created user");
        }

        // ====================== UPDATE & ROLE OPERATIONS ======================
        public async Task<Result<bool>> UpdateUserByAdminAsync(Guid id, UpdateUser dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return Result<bool>.Failure("User not found.");

            // Use AutoMapper to update existing entity (preserves Id and Identity fields)
            _mapper.Map(dto, user);

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task<Result<bool>> AssignRolesAsync(Guid userId, IEnumerable<string> roles)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result<bool>.Failure("User not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var validRoles = roles.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct().ToList();

            var result = await _userManager.AddToRolesAsync(user, validRoles);

            return result.Succeeded
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task<Result<bool>> RemoveRolesAsync(Guid userId, IEnumerable<string> roles)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result<bool>.Failure("User not found.");

            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            return result.Succeeded
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        // ====================== DELETE OPERATIONS ======================
        public async Task<Result<bool>> DeleteMyAccountAsync(Guid userId, string? reason = null)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result<bool>.Failure("User not found.");

            var result = await _userManager.DeleteAsync(user);
            await _donorRepository.SoftDeleteAsync(user.Id);


            return result.Succeeded
                ? Result<bool>.Success(true)
                : Result<bool>.Failure("Failed to delete account.");
        }

        public async Task<Result<bool>> DeleteUserByAdminAsync(Guid id, string? reason = null)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return Result<bool>.Failure("User not found.");

            _logger.LogWarning("Admin deleted user {UserId}. Reason: {Reason}", id, reason ?? "No reason provided");

            var result = await _userManager.DeleteAsync(user);

            await _donorRepository.SoftDeleteAsync(id);

            return result.Succeeded
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }
}
