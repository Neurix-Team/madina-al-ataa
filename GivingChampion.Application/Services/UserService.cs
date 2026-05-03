using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Application.DTO.User;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GivingChampion.Application.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUserRepository _userRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IAvatarRepository _avatarRepository;
        private readonly ILevelRepository _levelRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IUserRepository userRepository,
            IDonorRepository donorRepository,
            IVolunteerRepository volunteerRepository,
            IProfileRepository profileRepository,
            IAvatarRepository avatarRepository,
            ILevelRepository levelRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UserService> logger,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _donorRepository = donorRepository;
            _volunteerRepository = volunteerRepository;
            _profileRepository = profileRepository;
            _avatarRepository = avatarRepository;
            _levelRepository = levelRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // ====================== READ OPERATIONS ======================

        public async Task<PagedList<GetUserDto>> GetAllUsersAsync(
            PageParameters pageParameters,
            string? search = null)
        {
            var users = await _userRepository.GetAllAsync(pageParameters, search);

            return _mapper.MapPagedList<ApplicationUser, GetUserDto>(users);
        }

        public async Task<GetUserDto?> GetUserByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Invalid user ID.");

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User with ID {id} was not found.");

            return await MapUserWithRolesAsync(user);
        }

        public async Task<GetUserDto?> GetUserByIdForCurrentUserAsync(Guid id, bool isAdmin)
        {
            if (!isAdmin && id != UserId)
                throw new ForbiddenException("You are not allowed to access this user.");

            return await GetUserByIdAsync(id);
        }

        public async Task<GetUserDto?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException("Email is required.");

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                throw new NotFoundException($"User with email '{email}' was not found.");

            return await MapUserWithRolesAsync(user);
        }

        // ====================== CREATE OPERATIONS ======================

        public async Task<Result<GetUserDto>> CreateUserAsync(CreateUserDto dto)
        {
            ValidateCreateUserDto(dto);

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                if (!existingUser.IsDeleted)
                    throw new ConflictException("User with this email already exists.");

                var restoredUser = await RestoreDeletedUserAsync(existingUser, dto.Password);

                return Result<GetUserDto>.Success(restoredUser);
            }

            var user = _mapper.Map<ApplicationUser>(dto);

            user.EmailConfirmed = true;
            user.UserName = dto.Email;

            var createResult = await _userManager.CreateAsync(user, dto.Password);

            if (!createResult.Succeeded)
                throw new BadRequestException(BuildIdentityErrorMessage(createResult));

            await AddRolesOrThrowAsync(user, new[] { "User", "Volunteer", "Donor" });

            await _donorRepository.CreateAsync(user.Id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("New user created: {Email}", dto.Email);

            var createdUser = await GetUserByIdAsync(user.Id);

            if (createdUser == null)
                throw new NotFoundException("Failed to retrieve created user.");

            return Result<GetUserDto>.Success(createdUser);
        }

        public async Task<Result<GetUserDto>> CreateUserByAdminAsync(CreateUserDto dto)
        {
            return await CreateUserAsync(dto);
        }

        public async Task<Result<GetUserDto>> CreateChildUserAsync(CreateUserDto dto)
        {
            ValidateCreateUserDto(dto);

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                if (!existingUser.IsDeleted)
                    throw new ConflictException("User with this email already exists.");

                throw new ConflictException("A deleted user with this email already exists.");
            }

            var user = _mapper.Map<ApplicationUser>(dto);

            user.EmailConfirmed = false;
            user.UserName = dto.Email;
            user.BirthDay = DateTime.SpecifyKind(dto.BirthDay.Date, DateTimeKind.Utc);

            var createResult = await _userManager.CreateAsync(user, dto.Password);

            if (!createResult.Succeeded)
                throw new BadRequestException(BuildIdentityErrorMessage(createResult));

            try
            {
                await AddRolesOrThrowAsync(user, new[] { "Child" });
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                throw;
            }

            _logger.LogInformation("Child user created: {Email}", dto.Email);

            var createdUser = await GetUserByIdAsync(user.Id);

            if (createdUser == null)
                throw new NotFoundException("Failed to retrieve created child user.");

            return Result<GetUserDto>.Success(createdUser);
        }

        public async Task<Result> ApproveChildUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("Invalid user ID.");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new NotFoundException($"User with ID {userId} was not found.");

            if (user.IsDeleted)
                throw new BadRequestException("Cannot approve a deleted child user.");

            user.EmailConfirmed = true;
            user.LockoutEnabled = false;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                throw new BadRequestException(BuildIdentityErrorMessage(updateResult));

            await AddMissingRolesOrThrowAsync(user, new[] { "User", "Donor", "Volunteer" });
            await EnsureApprovedUserProfileAsync(user.Id);

            _logger.LogInformation("Child user approved and profile setup completed: {UserId}", user.Id);

            return Result.Success();
        }

        public async Task<Result<GetUserDto>> CreateAdminUserAsync(CreateUserDto dto)
        {
            ValidateCreateUserDto(dto);

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null && !existingUser.IsDeleted)
                throw new ConflictException("User already exists.");

            if (existingUser != null && existingUser.IsDeleted)
            {
                var restoredUser = await RestoreDeletedUserAsync(existingUser, dto.Password);

                await AddMissingRolesOrThrowAsync(existingUser, new[] { "Admin", "User", "Volunteer", "Donor" });

                return Result<GetUserDto>.Success(restoredUser);
            }

            var user = _mapper.Map<ApplicationUser>(dto);

            user.EmailConfirmed = true;
            user.UserName = dto.Email;

            var createResult = await _userManager.CreateAsync(user, dto.Password);

            if (!createResult.Succeeded)
                throw new BadRequestException(BuildIdentityErrorMessage(createResult));

            await AddRolesOrThrowAsync(user, new[] { "Admin", "User", "Volunteer", "Donor" });

            await _donorRepository.CreateAsync(user.Id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Admin user created: {Email}", dto.Email);

            var createdUser = await GetUserByIdAsync(user.Id);

            if (createdUser == null)
                throw new NotFoundException("Failed to retrieve created admin user.");

            return Result<GetUserDto>.Success(createdUser);
        }

        // ====================== UPDATE & ROLE OPERATIONS ======================

        public async Task<Result<bool>> UpdateUserByAdminAsync(Guid id, UpdateUser dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Invalid user ID.");

            if (dto == null)
                throw new BadRequestException("User update data is required.");

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User with ID {id} was not found.");

            _mapper.Map(dto, user);

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                throw new BadRequestException(BuildIdentityErrorMessage(updateResult));

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> AssignRolesAsync(Guid userId, IEnumerable<string> roles)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("Invalid user ID.");

            if (roles == null)
                throw new BadRequestException("Roles are required.");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new NotFoundException($"User with ID {userId} was not found.");

            var validRoles = roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .Distinct()
                .ToList();

            if (!validRoles.Any())
                throw new BadRequestException("At least one role is required.");

            foreach (var role in validRoles)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role);

                if (!roleExists)
                    throw new BadRequestException($"Role '{role}' does not exist.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!removeResult.Succeeded)
                    throw new BadRequestException(BuildIdentityErrorMessage(removeResult));
            }

            var addResult = await _userManager.AddToRolesAsync(user, validRoles);

            if (!addResult.Succeeded)
                throw new BadRequestException(BuildIdentityErrorMessage(addResult));

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> RemoveRolesAsync(Guid userId, IEnumerable<string> roles)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("Invalid user ID.");

            if (roles == null)
                throw new BadRequestException("Roles are required.");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new NotFoundException($"User with ID {userId} was not found.");

            var rolesToRemove = roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .Distinct()
                .ToList();

            if (!rolesToRemove.Any())
                throw new BadRequestException("At least one role is required.");

            var currentRoles = await _userManager.GetRolesAsync(user);

            var invalidAssignedRoles = rolesToRemove
                .Where(role => !currentRoles.Contains(role))
                .ToList();

            if (invalidAssignedRoles.Any())
                throw new BadRequestException($"User does not have role(s): {string.Join(", ", invalidAssignedRoles)}");

            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            if (!removeResult.Succeeded)
                throw new BadRequestException(BuildIdentityErrorMessage(removeResult));

            return Result<bool>.Success(true);
        }

        // ====================== DELETE OPERATIONS ======================

        public async Task<Result<bool>> DeleteMyAccountAsync(string? reason = null)
        {
            var userId = UserId;

            if (userId == Guid.Empty)
                throw new BadRequestException("Invalid user ID.");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new NotFoundException($"User with ID {userId} was not found.");

            var deleteResult = await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
                throw new BadRequestException(BuildIdentityErrorMessage(deleteResult));

            await _donorRepository.SoftDeleteAsync(user.Id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogWarning(
                "User deleted own account. UserId: {UserId}. Reason: {Reason}",
                userId,
                reason ?? "No reason provided"
            );

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteUserByAdminAsync(Guid id, string? reason = null)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Invalid user ID.");

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User with ID {id} was not found.");

            var deleteResult = await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
                throw new BadRequestException(BuildIdentityErrorMessage(deleteResult));

            await _donorRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogWarning(
                "Admin soft-deleted user {UserId}. Reason: {Reason}",
                id,
                reason ?? "No reason provided"
            );

            return Result<bool>.Success(true);
        }

        // ====================== PRIVATE HELPERS ======================

        private void ValidateCreateUserDto(CreateUserDto dto)
        {
            if (dto == null)
                throw new BadRequestException("User data is required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BadRequestException("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequestException("Password is required.");

            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new BadRequestException("Full name is required.");
        }

        private async Task<GetUserDto> RestoreDeletedUserAsync(
            ApplicationUser existingUser,
            string newPassword)
        {
            existingUser.IsDeleted = false;
            existingUser.DeletedAt = null;
            existingUser.EmailConfirmed = true;

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);

            var resetPasswordResult = await _userManager.ResetPasswordAsync(
                existingUser,
                resetToken,
                newPassword
            );

            if (!resetPasswordResult.Succeeded)
                throw new BadRequestException(BuildIdentityErrorMessage(resetPasswordResult));

            var updateResult = await _userManager.UpdateAsync(existingUser);

            if (!updateResult.Succeeded)
                throw new BadRequestException(BuildIdentityErrorMessage(updateResult));

            _logger.LogInformation("Soft-deleted user restored: {Email}", existingUser.Email);

            var restoredUser = await GetUserByIdAsync(existingUser.Id);

            if (restoredUser == null)
                throw new NotFoundException("Failed to retrieve restored user.");

            return restoredUser;
        }

        private async Task CreateDonorProfileAsync(Guid userId)
        {
            
        }

        private async Task EnsureApprovedUserProfileAsync(Guid userId)
        {
            if (!await _donorRepository.ExistsByUserIdAsync(userId))
            {
                await _donorRepository.CreateAsync(userId);
                await _unitOfWork.SaveChangesAsync();
            }

            var volunteer = await _volunteerRepository.GetByUserIdAsync(userId);

            if (volunteer == null)
            {
                await _volunteerRepository.AddAsync(userId);
                await _unitOfWork.SaveChangesAsync();
            }

            var profile = await _profileRepository.GetByUserIdAsync(userId);

            if (profile == null)
            {
                var level = await _levelRepository.GetFirstLevelAsync();

                if (level == null)
                    throw new NotFoundException("Default level was not found.");

                profile = await _profileRepository.AddAsync(userId, level.Id);
                await _unitOfWork.SaveChangesAsync();
            }

            var avatar = await _avatarRepository.GetByProfileIdAsync(profile.Id);

            if (avatar == null)
            {
                await _avatarRepository.AddAsync(profile.Id);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task AddRolesOrThrowAsync(
            ApplicationUser user,
            IEnumerable<string> roles)
        {
            var roleList = roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .Distinct()
                .ToList();

            if (!roleList.Any())
                throw new BadRequestException("At least one role is required.");

            foreach (var role in roleList)
        {
                var roleExists = await _roleManager.RoleExistsAsync(role);

                if (!roleExists)
                    throw new BadRequestException($"Role '{role}' does not exist.");
            }

            var addResult = await _userManager.AddToRolesAsync(user, roleList);

            if (!addResult.Succeeded)
                throw new BadRequestException(BuildIdentityErrorMessage(addResult));
        }

        private async Task AddMissingRolesOrThrowAsync(
            ApplicationUser user,
            IEnumerable<string> roles)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);

            var missingRoles = roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .Distinct()
                .Where(r => !currentRoles.Contains(r))
                .ToList();

            if (!missingRoles.Any())
                return;

            await AddRolesOrThrowAsync(user, missingRoles);
        }

        private async Task<GetUserDto> MapUserWithRolesAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var dto = _mapper.Map<GetUserDto>(user);
            dto.Roles = roles.ToList();

            return dto;
        }

        private static string BuildIdentityErrorMessage(IdentityResult result)
        {
            return string.Join("; ", result.Errors.Select(e => e.Description));
        }
    }
}
