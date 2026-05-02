using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.Exceptions;
using GivingChampion.Common.DTO.ReviewDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.API.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Review> _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _reviewRepository = unitOfWork.Repository<Review>();
            _mapper = mapper;
        }

        public async Task<List<ReviewDto>> GetAllByProfileIdAsync(Guid profileId)
        {
            if (profileId == Guid.Empty)
                throw new BadRequestException("Profile ID is required.");

            var reviews = await _reviewRepository.ListAsync(review => review.ProfileId == profileId);

            return _mapper.Map<List<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Review ID is required.");

            var review = await _reviewRepository.GetByIdAsync(id);

            if (review == null)
                throw new NotFoundException($"Review with ID {id} was not found.");

            if (review.IsDeleted)
                throw new NotFoundException($"Review with ID {id} was not found.");

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<ReviewDto> CreateAsync(CreateReviewDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Review create data is required.");

            var review = _mapper.Map<Review>(dto);

            review.ReviewDate = DateTime.UtcNow;

            await _reviewRepository.AddAsync(review);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateReviewDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Review ID is required.");

            if (dto == null)
                throw new BadRequestException("Review update data is required.");

            var review = await _reviewRepository.GetByIdAsync(id);

            if (review == null)
                throw new NotFoundException($"Review with ID {id} was not found.");

            if (review.IsDeleted)
                throw new BadRequestException("Cannot update a deleted review.");

            _mapper.Map(dto, review);

            _reviewRepository.Update(review);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Review ID is required.");

            var review = await _reviewRepository.GetByIdAsync(id);

            if (review == null)
                throw new NotFoundException($"Review with ID {id} was not found.");

            if (review.IsDeleted)
                throw new BadRequestException("Review is already deleted.");

            review.IsDeleted = true;
            review.DeletedAt = DateTime.UtcNow;

            _reviewRepository.Update(review);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
