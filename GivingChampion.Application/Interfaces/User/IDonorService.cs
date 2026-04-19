using GivingChampion.Common.DTO.Donor;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.User
{
    public interface IDonorService
    {
        Task<Result<DonorDto>> CreateDonorAsync(CreateDonorDto dto, Guid userId);
        Task<Result<DonorDto>> GetMyDonorProfileAsync(Guid userId);
        Task<Result<DonorDto>> GetDonorByUserIdAsync(Guid userId);
        Task<Result<DonorDto>> UpdateDonorAsync(UpdateDonorDto dto, Guid userId);
        Task<Result> SoftDeleteDonorAsync(Guid userId);
    }
}
