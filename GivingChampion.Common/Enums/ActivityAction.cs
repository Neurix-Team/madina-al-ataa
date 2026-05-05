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
        PartnerCreated = 11,
        PartnerUpdated = 12,
        PartnerDeleted = 13,
        DonationOrderCreated = 14,
        DonationOrderUpdated = 15,
        DonationOrderApproved = 16,
        DonationOrderRejected = 17,
        DonationRequestCreated = 18,
        DonationRequestUpdated = 19,
        DonationRequestApproved = 20,
        DonationRequestRejected = 21,
        DonationRequestDeleted = 22,
    }
}
