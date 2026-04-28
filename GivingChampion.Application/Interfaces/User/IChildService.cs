using GivingChampion.Common.DTO.Child;
using GivingChampion.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.User
{
    public interface IChildService
    {
        Task<Result<ChildDto>> CreateChildAsync(CreateChildDto dto, Guid parentId);
        Task<Result<List<ChildDto>>> GetMyChildrenAsync(Guid parentId);
        Task<Result<List<ChildDto>>> GetPendingApprovalsAsync();
        Task<Result> ApproveChildAsync(Guid childId, Guid approvedById);
        Task<Result> RejectChildAsync(RejectChildDto dto, Guid rejectedById);
        Task<Result> SoftDeleteChildAsync(Guid childId);
    }
}
