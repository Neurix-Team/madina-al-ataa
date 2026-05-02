using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.Volunteer;
using GivingChampion.Common.DTO.VolunteerDto;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services
{
    public class VolunteerService : IVolunteerService
    {
        #region Feild

        private readonly IVolunteerRepository _volunteerRepository;
        private readonly IMapper _mapper;

        #endregion

        public VolunteerService(
            IVolunteerRepository volunteerRepository,
            IMapper mapper)
        {
            _volunteerRepository = volunteerRepository;
            _mapper = mapper;
        }


        #region Query Methods

        #region GetAllVolunteer

        public async Task<Result<PagedList<VolunteerDto>>> GetAllAsync(PageParameters pageParameters)
        {
            var volunteers = await _volunteerRepository.GetAllAsync(pageParameters);

            var dtos = _mapper.MapPagedList<Volunteer, VolunteerDto>(volunteers);

            return Result<PagedList<VolunteerDto>>.Success(dtos);
        }

        #endregion

        #region GetVolunteerById

        public async Task<VolunteerDto?> GetByIdAsync(Guid id)
        {
            var volunteer = await _volunteerRepository.GetByIdAsync(id);

            if (volunteer == null)
                throw new NotFoundException($"Volunteer with ID {id} was not found.");

            return _mapper.Map<VolunteerDto>(volunteer);
        }

        #endregion

        #endregion
    }
}