using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.Volunteer;
using GivingChampion.Application.DTO.VolunteerDto;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services
{
    public class VolunteerService : IVolunteerService
    {
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly IMapper _mapper;

        public VolunteerService(
            IVolunteerRepository volunteerRepository,
            IMapper mapper)
        {
            _volunteerRepository = volunteerRepository;
            _mapper = mapper;
        }

        public async Task<VolunteerDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Volunteer ID is required.");

            var volunteer = await _volunteerRepository.GetByIdAsync(id);

            if (volunteer == null)
                throw new NotFoundException($"Volunteer with ID {id} was not found.");

            if (volunteer.IsDeleted)
                throw new NotFoundException($"Volunteer with ID {id} was not found.");

            return _mapper.Map<VolunteerDto>(volunteer);
        }
    }
}