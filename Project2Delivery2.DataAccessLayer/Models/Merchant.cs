using System.ComponentModel.DataAnnotations;

namespace Project2Delivery2.DataAccessLayer.Models
{
    public class Merchant
    {
        [Key]
        public int MerchantId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal MerchantRiskScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }

}
