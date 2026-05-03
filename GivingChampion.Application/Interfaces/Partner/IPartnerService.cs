using GivingChampion.Application.DTO.Partner;
using GivingChampion.Application.DTO.PartnerDto;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.Partner
{
    public interface IPartnerService
    {
        // Query Methods

        Task<Result<PagedList<PartnerDto>>> GetAllAsync(PageParameters pageParameters);
        Task<PartnerDto?> GetByIdAsync(Guid id); // Get a partner by ID

            // Command Methods

            Task<PartnerDto> CreateAsync(CreatePartnerDto dto); // Create a new partner
            Task<PartnerDto?> UpdateAsync(Guid id, UpdatePartnerDto dto); // Update existing partner
            Task<bool> DeleteAsync(Guid id); // Soft delete a partner
        }
    }

