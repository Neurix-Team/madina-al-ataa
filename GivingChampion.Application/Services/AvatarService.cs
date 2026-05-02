using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.Exceptions;
using GivingChampion.Common.DTO.AvatarDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.API.Services
{
    public class AvatarService : IAvatarService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Avatar> _avatarRepository;
        private readonly IMapper _mapper;

        public AvatarService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _avatarRepository = unitOfWork.Repository<Avatar>();
            _mapper = mapper;
        }

        public async Task<AvatarDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Avatar ID is required.");

            var avatar = await _avatarRepository.GetByIdAsync(id);

            if (avatar == null)
                throw new NotFoundException($"Avatar with ID {id} was not found.");

            if (avatar.IsDeleted)
                throw new NotFoundException($"Avatar with ID {id} was not found.");

            return _mapper.Map<AvatarDto>(avatar);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateAvatarDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Avatar ID is required.");

            if (dto == null)
                throw new BadRequestException("Avatar update data is required.");

            var avatar = await _avatarRepository.GetByIdAsync(id);

            if (avatar == null)
                throw new NotFoundException($"Avatar with ID {id} was not found.");

            if (avatar.IsDeleted)
                throw new BadRequestException("Cannot update a deleted avatar.");

            _mapper.Map(dto, avatar);

            _avatarRepository.Update(avatar);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Avatar ID is required.");

            var avatar = await _avatarRepository.GetByIdAsync(id);

            if (avatar == null)
                throw new NotFoundException($"Avatar with ID {id} was not found.");

            if (avatar.IsDeleted)
                throw new BadRequestException("Avatar is already deleted.");

            avatar.IsDeleted = true;
            avatar.DeletedAt = DateTime.UtcNow;

            _avatarRepository.Update(avatar);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
