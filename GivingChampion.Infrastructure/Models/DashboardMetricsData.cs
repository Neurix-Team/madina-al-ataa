namespace GivingChampion.Persistance.Models
{
    public sealed class DashboardMetricsData
    {
        public int TotalUsers { get; set; }
        public int TotalDonors { get; set; }
        public int TotalVolunteers { get; set; }
        public int TotalPartners { get; set; }
        public int TotalChildren { get; set; }
        public int TotalDonationRequests { get; set; }
        public int PendingDonationRequests { get; set; }
        public int CompletedDonationRequests { get; set; }
        public int TotalServiceRequests { get; set; }
        public int PendingServiceRequests { get; set; }
        public int CompletedServiceRequests { get; set; }
        public int TotalDonationOrders { get; set; }
        public int PendingDonationOrders { get; set; }
        public int CompletedDonationOrders { get; set; }
        public int TotalVolunteerOrders { get; set; }
        public int PendingVolunteerOrders { get; set; }
        public int CompletedVolunteerOrders { get; set; }
        public decimal TotalDonationAmount { get; set; }
        public decimal TotalDonationAmountRemaining { get; set; }
    }
}
