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
        PartnerCreated = 7,
        PartnerUpdated = 8,
        PartnerDeleted = 9,
        DonationOrderCreated = 10,
        DonationOrderUpdated = 11,
        DonationOrderApproved = 12,
        DonationOrderRejected = 13,
        DonationRequestCreated = 14,
        DonationRequestUpdated = 15,
        DonationRequestApproved = 16,
        DonationRequestRejected = 17,
        DonationRequestDeleted = 18,
        MissionCreated = 19,
        MissionUpdated = 20,
        MissionDeleted = 21, 
        MissionApproved = 22,
        MissionRejected = 23,
        MissionCompleted = 24,
        MissionStarted = 25,
        UserMissionStarted = 26,
        UserMissionUpdated = 27,
        UserMissionCompleted = 28,
        UserMissionDeleted = 29,

        OrderCancelled = 30,
        RequestCreated = 31,
        RequestUpdated = 32,
        RequestDeleted = 33,
    }
}
