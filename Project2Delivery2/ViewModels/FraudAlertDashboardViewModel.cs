using Project2Delivery2.DataAccessLayer.Models;

namespace Project2Delivery2.ViewModels
{ 
    public class FraudAlertDashboardViewModel
    {
        public int TotalAlerts { get; set; }
        public int OpenAlerts { get; set; }
        public int UnderReviewAlerts { get; set; }
        public int ResolvedAlerts { get; set; }
        public int HighPriorityAlerts { get; set; }
        public List<FraudAlert> RecentAlerts { get; set; }
        public List<FraudAlert> HighRiskAlerts { get; set; }
        public decimal ResolutionRate { get; set; }
        public decimal FalsePositiveRate { get; set; }
    }
}