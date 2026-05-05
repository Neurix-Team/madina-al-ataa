using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Common.Enums
{
    public enum ActivityAction
    {
        OrderCreated = 0,
        OrderApproved = 1,
        OrderRejected = 2,
        TaskAssigned = 3,
        TaskStarted = 4, 
        ProgressUpdated = 5,
        TaskCompleted = 6,
        OrderCancelled = 7,
        RequestCreated = 8,
        RequestUpdated = 9,
        RequestDeleted = 10,
        
    }
}
