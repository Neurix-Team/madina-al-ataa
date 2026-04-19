using GivingChampion.Common.DTO.Partner;
using GivingChampion.Common.DTO.PartnerDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.Partner
{
    public interface IPartnerService
    {
            // Query Methods

            Task<List<PartnerDto>> GetAllAsync(); // Get all partners
            Task<PartnerDto?> GetByIdAsync(Guid id); // Get a partner by ID

            // Command Methods

            Task<PartnerDto> CreateAsync(CreatePartnerDto dto); // Create a new partner
            Task<PartnerDto?> UpdateAsync(Guid id, UpdatePartnerDto dto); // Update existing partner
            Task<bool> DeleteAsync(Guid id); // Soft delete a partner
        }
    }

