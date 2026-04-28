using AutoMapper;
using GivingChampion.Application.Interfaces.Volunteer;
using GivingChampion.Common.DTO.VolunteerDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingChampion.Application.Services
{
    public class VolunteerService : IVolunteerService
    {
        #region Feild
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly IMapper _mapper; 
        #endregion

        #region Constructor

        public VolunteerService(IVolunteerRepository volunteerRepository, IMapper mapper)
        {
            _volunteerRepository = volunteerRepository;
            _mapper = mapper;
        }

        #endregion

        #region Query Methods

        #region GetAllVolunteer
        public async Task<List<VolunteerDto>> GetAllAsync()
        {
            // Get all volunteers from the repository (as entities)
            var volunteers = await _volunteerRepository.GetAllAsync();

            // Map to VolunteerDto to return to controller
            return _mapper.Map<List<VolunteerDto>>(volunteers);
        }

        #endregion

        #region GetVolunteerById
        public async Task<VolunteerDto?> GetByIdAsync(Guid id)
        {
            // Get a volunteer by ID from the repository (as entity)
            var volunteer = await _volunteerRepository.GetByIdAsync(id);

            // Return null if volunteer is not found
            if (volunteer == null)
                return null;

            // Map to VolunteerDto to return to controller
            return _mapper.Map<VolunteerDto>(volunteer);
        }
        #endregion

        #endregion

        #region Command Methods

        #region CreateVolunteer

        //public async Task<VolunteerDto> CreateAsync(CreateVolunteerDto dto)
        //{
        //    // Map the incoming CreateVolunteerDto to Volunteer entity
        //    var volunteer = _mapper.Map<Volunteer>(dto);

        //    // Add the volunteer to the repository
        //    await _volunteerRepository.AddAsync(volunteer);
        //    await _volunteerRepository.SaveChangesAsync();

        //    // Map to VolunteerDto to return to controller
        //    return _mapper.Map<VolunteerDto>(volunteer);
        //} 
        #endregion

        #region UpdateVolunteer
        public async Task<VolunteerDto?> UpdateAsync(Guid id, UpdateVolunteerDto dto)
        {
            // Get the existing volunteer by ID
            var volunteer = await _volunteerRepository.GetByIdAsync(id);

            // Return null if volunteer does not exist
            if (volunteer == null)
                return null;

            // Map the incoming UpdateVolunteerDto to the existing volunteer entity
            _mapper.Map(dto, volunteer);
            volunteer.UserId = volunteer.UserId; // Ensure UserId remains unchanged

            // Update the volunteer in the repository
            _volunteerRepository.Update(volunteer);
            await _volunteerRepository.SaveChangesAsync();

            // Map to VolunteerDto to return the updated volunteer
            return _mapper.Map<VolunteerDto>(volunteer);
        } 
        #endregion

        #endregion
    }
}