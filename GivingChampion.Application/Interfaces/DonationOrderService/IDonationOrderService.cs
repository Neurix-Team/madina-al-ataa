using GivingChampion.Common.DTO.DonationOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.DonationOrderService
{
    public interface IDonationOrderService
    {
      
            Task<DonationOrderDetailsDto> GetByIdAsync(Guid id);
            Task<IEnumerable<DonationOrderReadDto>> GetAllAsync();
            Task CreateAsync(CreateDonationOrderDto donationOrderDto);
            Task UpdateAsync(UpdateDonationOrderDTO donationOrderDto, Guid id);
        }
    }

