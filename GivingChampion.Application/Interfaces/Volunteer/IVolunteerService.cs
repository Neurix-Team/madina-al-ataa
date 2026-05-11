using GivingChampion.Application.DTO.Volunteer;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.Volunteer
{
    public interface IVolunteerService
    {
        Task<Result<VolunteerDto>> GetMyVolunteerProfileAsync();

        Task<Result<VolunteerDto>> GetVolunteerByUserIdAsync(Guid userId);

        Task<Result<VolunteerDto>> UpdateVolunteerAsync(UpdateVolunteerDto dto);


    }
}

