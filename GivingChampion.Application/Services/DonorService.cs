using AutoMapper;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Common.DTO.Donor;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GivingChampion.Application.Services
{
    public class DonorService : IDonorService
    {
        private readonly IDonorRepository _donorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DonorService> _logger;

        public DonorService(
            IDonorRepository donorRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<DonorService> logger)
        {
            _donorRepository = donorRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<DonorDto>> CreateDonorAsync(CreateDonorDto dto, Guid userId)
        {
            // Check if donor profile already exists for this user
            if (await _donorRepository.ExistsByUserIdAsync(userId))
                return Result<DonorDto>.Failure("Donor profile already exists for this user.");

            var donor = _mapper.Map<Donor>(dto);
            donor.Id = Guid.NewGuid();
            donor.UserId = userId;
            donor.TotalDonated = 0;
            donor.CreatedAt = DateTime.UtcNow;

            await _donorRepository.CreateAsync(donor);

            _logger.LogInformation("Donor profile created for user {UserId}", userId);

            var donorDto = _mapper.Map<DonorDto>(donor);
            return Result<DonorDto>.Success(donorDto);
        }

        public async Task<Result<DonorDto>> GetMyDonorProfileAsync(Guid userId)
        {
            var donor = await _donorRepository.GetByUserIdAsync(userId);
            if (donor == null)
                return Result<DonorDto>.Failure("Donor profile not found.");

            var dto = _mapper.Map<DonorDto>(donor);
            return Result<DonorDto>.Success(dto);
        }

        public async Task<Result<DonorDto>> GetDonorByUserIdAsync(Guid userId)
        {
            var donor = await _donorRepository.GetByUserIdAsync(userId);
            if (donor == null)
                return Result<DonorDto>.Failure("Donor profile not found.");

            var dto = _mapper.Map<DonorDto>(donor);
            return Result<DonorDto>.Success(dto);
        }

        public async Task<Result<DonorDto>> UpdateDonorAsync(UpdateDonorDto dto, Guid userId)
        {
            var donor = await _donorRepository.GetByUserIdAsync(userId);
            if (donor == null)
                return Result<DonorDto>.Failure("Donor profile not found.");

            _mapper.Map(dto, donor);
            donor.UpdatedAt = DateTime.UtcNow;

            await _donorRepository.UpdateAsync(donor);

            var updatedDto = _mapper.Map<DonorDto>(donor);
            return Result<DonorDto>.Success(updatedDto);
        }

        public async Task<Result> SoftDeleteDonorAsync(Guid userId)
        {
            await _donorRepository.SoftDeleteAsync(userId);
            _logger.LogInformation("Donor profile soft deleted for user {UserId}", userId);
            return Result.Success();
        }
    }
}