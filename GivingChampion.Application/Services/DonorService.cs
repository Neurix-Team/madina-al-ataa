using AutoMapper;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Application.DTO.Donor;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GivingChampion.Application.Services
{
    public class DonorService : BaseService, IDonorService
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
            ILogger<DonorService> logger,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _donorRepository = donorRepository;
            //_userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

 
        public async Task<DonorDto> GetMyDonorProfileAsync()
        {
            var donor = await _donorRepository.GetByUserIdAsync(UserId);
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

        public async Task<DonorDto> UpdateDonorAsync(UpdateDonorDto dto)
        {
            var donor = await _donorRepository.GetByUserIdAsync(UserId);
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
