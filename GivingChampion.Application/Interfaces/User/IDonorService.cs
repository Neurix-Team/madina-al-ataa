using GivingChampion.Application.DTO.Donor;
using GivingChampion.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.User
{
    public interface IDonorService
    {
        //Task<DonorDto> CreateDonorAsync(CreateDonorDto dto, Guid userId);
        Task<DonorDto> GetMyDonorProfileAsync();
        Task<DonorDto> GetDonorByUserIdAsync(Guid userId);
        Task<DonorDto> UpdateDonorAsync(UpdateDonorDto dto);
        Task<bool> SoftDeleteDonorAsync(Guid userId);
    }
}
