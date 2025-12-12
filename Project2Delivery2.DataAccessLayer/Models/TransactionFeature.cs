using System.ComponentModel.DataAnnotations;

namespace Project2Delivery2.DataAccessLayer.Models
{
    public class TransactionFeature
    {
        [Key]
        public int FeatureId { get; set; }
        public long TransactionId { get; set; }
        public string FeatureJson { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
