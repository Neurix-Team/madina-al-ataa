using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.Exceptions;
using GivingChampion.Common.DTO.UserBadgeDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.API.Services
{
    public class UserBadgeService : IUserBadgeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserBadge> _userBadgeRepository;
        private readonly IMapper _mapper;

        public UserBadgeService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userBadgeRepository = unitOfWork.Repository<UserBadge>();
            _mapper = mapper;
        }

        public async Task<List<UserBadgeDto>> GetAllByProfileIdAsync(Guid profileId)
        {
            if (profileId == Guid.Empty)
                throw new BadRequestException("Profile ID is required.");

            var userBadges = await _userBadgeRepository.ListAsync(userBadge => userBadge.ProfileId == profileId);

            return _mapper.Map<List<UserBadgeDto>>(userBadges);
        }

        public async Task<UserBadgeDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("UserBadge ID is required.");

            var userBadge = await _userBadgeRepository.GetByIdAsync(id);

            if (userBadge == null)
                throw new NotFoundException($"UserBadge with ID {id} was not found.");

            if (userBadge.IsDeleted)
                throw new NotFoundException($"UserBadge with ID {id} was not found.");

            return _mapper.Map<UserBadgeDto>(userBadge);
        }

        public async Task<UserBadgeDto> CreateAsync(CreateUserBadgeDto dto)
        {
            if (dto == null)
                throw new BadRequestException("UserBadge create data is required.");

            var userBadge = _mapper.Map<UserBadge>(dto);

            await _userBadgeRepository.AddAsync(userBadge);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserBadgeDto>(userBadge);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateUserBadgeDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("UserBadge ID is required.");

            if (dto == null)
                throw new BadRequestException("UserBadge update data is required.");

            var userBadge = await _userBadgeRepository.GetByIdAsync(id);

            if (userBadge == null)
                throw new NotFoundException($"UserBadge with ID {id} was not found.");

            if (userBadge.IsDeleted)
                throw new BadRequestException("Cannot update a deleted UserBadge.");

            _mapper.Map(dto, userBadge);

            _userBadgeRepository.Update(userBadge);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("UserBadge ID is required.");

            var userBadge = await _userBadgeRepository.GetByIdAsync(id);

            if (userBadge == null)
                throw new NotFoundException($"UserBadge with ID {id} was not found.");

            if (userBadge.IsDeleted)
                throw new BadRequestException("UserBadge is already deleted.");

            userBadge.IsDeleted = true;
            userBadge.DeletedAt = DateTime.UtcNow;

            _userBadgeRepository.Update(userBadge);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
