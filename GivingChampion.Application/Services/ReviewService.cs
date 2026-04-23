using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.ReviewDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.API.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<List<ReviewDto>> GetAllByProfileIdAsync(Guid profileId)
        {
            var reviews = await _reviewRepository.GetAllByProfileIdAsync(profileId);
            return _mapper.Map<List<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto?> GetByIdAsync(Guid id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            return review == null ? null : _mapper.Map<ReviewDto>(review);
        }

        public async Task<ReviewDto> CreateAsync(CreateReviewDto dto)
        {
            var review = _mapper.Map<Review>(dto);
            review.ReviewDate = DateTime.UtcNow;
            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();
            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateReviewDto dto)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return false;

            _mapper.Map(dto, review);
            _reviewRepository.Update(review);
            await _reviewRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return false;

            review.IsDeleted = true;
            review.DeletedAt = DateTime.UtcNow;
            _reviewRepository.Update(review);
            await _reviewRepository.SaveChangesAsync();
            return true;
        }
    }
}
