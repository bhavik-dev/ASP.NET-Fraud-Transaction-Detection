using System.ComponentModel.DataAnnotations;

namespace Project2Delivery2.DataAccessLayer.Models
{
    public class FraudAlert
    {
        [Key]
        public long AlertId { get; set; }
        public long TransactionId { get; set; }
        public string AlertLevel { get; set; }
        public string Status { get; set; }
        public int? AssignedTo { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Transaction? Transaction { get; set; }
        public User? AssignedUser { get; set; }
    }
}
