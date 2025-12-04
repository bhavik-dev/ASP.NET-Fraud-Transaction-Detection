using Project2Delivery2.DataAccessLayer.Models;

namespace Project2Delivery2.ViewModels
{
    public class TransactionDetailViewModel
    {
        public Transaction Transaction { get; set; }
        public List<FraudAlert> RelatedAlerts { get; set; }
        public decimal RiskScore { get; set; }
        public string RiskLevel { get; set; }
        public List<string> RiskFactors { get; set; }
        public int AccountTransactionCount { get; set; }
        public decimal AccountTotalSpent { get; set; }
        public bool IsFirstTransaction { get; set; }
        public bool IsNewDevice { get; set; }
        public int DeviceTransactionCount { get; set; }
    }
}