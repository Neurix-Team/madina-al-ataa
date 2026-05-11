using AutoMapper;
using GivingChampion.Application.DTO.Volunteer;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.Volunteer;
using GivingChampion.Application.Services;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GivingChampion.Application.Services
{
    public class VolunteerService : BaseService, IVolunteerService
    {
        #region Fields

        private readonly IVolunteerRepository _volunteerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        #endregion

        public VolunteerService(
            IVolunteerRepository volunteerRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _volunteerRepository = volunteerRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Query Methods

        public async Task<Result<PagedList<VolunteerDto>>> GetAllAsync(PageParameters pageParameters)
        {
            var volunteers = await _volunteerRepository.GetAllAsync(pageParameters);

            var dtos = _mapper.MapPagedList<Volunteer, VolunteerDto>(volunteers);

            return Result<PagedList<VolunteerDto>>.Success(dtos);
        }

        public async Task<VolunteerDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Volunteer ID is required.");

            var volunteer = await _volunteerRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Volunteer with ID {id} was not found.");

            return _mapper.Map<VolunteerDto>(volunteer);
        }

        public async Task<Result<VolunteerDto>> GetMyVolunteerProfileAsync()
        {
            var userId = UserId;

            var volunteer = await _volunteerRepository.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("Volunteer profile was not found.");

            return Result<VolunteerDto>.Success(_mapper.Map<VolunteerDto>(volunteer));
        }

        public async Task<Result<VolunteerDto>> GetVolunteerByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("User ID is required.");

            var volunteer = await _volunteerRepository.GetByUserIdAsync(userId)
                ?? throw new NotFoundException($"Volunteer profile with user ID {userId} was not found.");

            return Result<VolunteerDto>.Success(_mapper.Map<VolunteerDto>(volunteer));
        }

        #endregion

        #region Command Methods

        public async Task<Result<VolunteerDto>> UpdateVolunteerAsync(UpdateVolunteerDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Volunteer update data is required.");

            var userId = UserId;

            var volunteer = await _volunteerRepository.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("Volunteer profile was not found.");

            _mapper.Map(dto, volunteer);

            volunteer.UpdatedAt = DateTime.UtcNow;

            _volunteerRepository.Update(volunteer);

            await _unitOfWork.SaveChangesAsync();

            return Result<VolunteerDto>.Success(_mapper.Map<VolunteerDto>(volunteer));
        }

        #endregion
    }
}