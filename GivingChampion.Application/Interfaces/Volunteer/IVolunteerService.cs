using GivingChampion.Common.DTO.VolunteerDto;
using GivingChampion.Common.DTO.VolunteerOrder;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.Volunteer
{
    public interface IVolunteerService
    {


        #region Query Methods

        //#region GetAllVolunteers

        //Task<List<VolunteerDto>> GetAllAsync();

        #endregion
        Task<Result<PagedList<VolunteerDto>>> GetAllAsync(PageParameters pageParameters);


        Task<VolunteerDto?> GetByIdAsync(Guid id);

        


    }
}

