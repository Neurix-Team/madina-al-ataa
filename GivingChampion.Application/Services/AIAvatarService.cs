using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.DTO.AiAvatarDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.API.Services
{
    public class AiAvatarService : IAiAvatarService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<AiAvatar> _aiAvatarRepository;
        private readonly IMapper _mapper;

        public AiAvatarService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _aiAvatarRepository = unitOfWork.Repository<AiAvatar>();
            _mapper = mapper;
        }

        public async Task<AiAvatarDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("AI Avatar ID is required.");

            var aiAvatar = await _aiAvatarRepository.GetByIdAsync(id);

            if (aiAvatar == null)
                throw new NotFoundException($"AI Avatar with ID {id} was not found.");

            if (aiAvatar.IsDeleted)
                throw new NotFoundException($"AI Avatar with ID {id} was not found.");

            return _mapper.Map<AiAvatarDto>(aiAvatar);
        }

        public async Task<AiAvatarDto> CreateAsync(CreateAiAvatarDto dto)
        {
            if (dto == null)
                throw new BadRequestException("AI Avatar create data is required.");

            var aiAvatar = _mapper.Map<AiAvatar>(dto);

            await _aiAvatarRepository.AddAsync(aiAvatar);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AiAvatarDto>(aiAvatar);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateAiAvatarDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("AI Avatar ID is required.");

            if (dto == null)
                throw new BadRequestException("AI Avatar update data is required.");

            var existingAiAvatar = await _aiAvatarRepository.GetByIdAsync(id);

            if (existingAiAvatar == null)
                throw new NotFoundException($"AI Avatar with ID {id} was not found.");

            if (existingAiAvatar.IsDeleted)
                throw new BadRequestException("Cannot update a deleted AI Avatar.");

            _mapper.Map(dto, existingAiAvatar);

            _aiAvatarRepository.Update(existingAiAvatar);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("AI Avatar ID is required.");

            var aiAvatar = await _aiAvatarRepository.GetByIdAsync(id);

            if (aiAvatar == null)
                throw new NotFoundException($"AI Avatar with ID {id} was not found.");

            if (aiAvatar.IsDeleted)
                throw new BadRequestException("AI Avatar is already deleted.");

            aiAvatar.IsDeleted = true;
            aiAvatar.DeletedAt = DateTime.UtcNow;

            _aiAvatarRepository.Update(aiAvatar);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
