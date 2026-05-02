using AutoMapper;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Common.DTO.Donor;
using GivingChampion.Persistance.Interfaces;
using Microsoft.Extensions.Logging;

namespace GivingChampion.Application.Services
{
    public class DonorService : IDonorService
    {
        private readonly IDonorRepository _donorRepository;
        //private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DonorService> _logger;

        public DonorService(
            IDonorRepository donorRepository,
            //IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<DonorService> logger)
        {
            _donorRepository = donorRepository;
            //_userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        //public async Task<DonorDto> CreateDonorAsync(CreateDonorDto dto, Guid userId)
        //{
        //    // Check if donor profile already exists for this user
        //    if (await _donorRepository.ExistsByUserIdAsync(userId))
        //        throw new InvalidOperationException("Donor profile already exists for this user.");
        //    var donor = _mapper.Map<Donor>(dto);
        //    donor.UserId = userId;
        //    donor.CreatedAt = DateTime.UtcNow;

        //    await _donorRepository.CreateAsync(donor);

        //    _logger.LogInformation("Donor profile created for user {UserId}", userId);

        //    var donorDto = _mapper.Map<DonorDto>(donor);
        //    return donorDto;
        //}

        public async Task<DonorDto> GetMyDonorProfileAsync(Guid userId)
        {
            var donor = await _donorRepository.GetByUserIdAsync(userId);
            if (donor == null)
                throw new InvalidOperationException("Donor profile not found.");
            var dto = _mapper.Map<DonorDto>(donor);
            return dto;
        }

        public async Task<DonorDto> GetDonorByUserIdAsync(Guid userId)
        {
            var donor = await _donorRepository.GetByUserIdAsync(userId);
            if (donor == null)
                throw new InvalidOperationException("Donor profile not found.");
            var dto = _mapper.Map<DonorDto>(donor);
            return dto;
        }

        public async Task<DonorDto> UpdateDonorAsync(UpdateDonorDto dto, Guid userId)
        {
            var donor = await _donorRepository.GetByUserIdAsync(userId);
            if (donor == null)
                throw new InvalidOperationException("Donor profile not found.");
            _mapper.Map(dto, donor);
            donor.UpdatedAt = DateTime.UtcNow;

            await _donorRepository.UpdateAsync(donor);
            await _unitOfWork.SaveChangesAsync();

            var updatedDto = _mapper.Map<DonorDto>(donor);
            return updatedDto;
        }

        public async Task<bool> SoftDeleteDonorAsync(Guid userId)
        {
            await _donorRepository.SoftDeleteAsync(userId);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Donor profile soft deleted for user {UserId}", userId);
            return true;
        }
    }
}
