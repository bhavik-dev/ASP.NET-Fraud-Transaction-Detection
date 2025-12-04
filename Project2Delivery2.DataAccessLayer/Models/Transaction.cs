using System.ComponentModel.DataAnnotations;

namespace Project2Delivery2.DataAccessLayer.Models
{
    public class Transaction
    {
        public long TransactionId { get; set; }

        [Required(ErrorMessage = "Transaction reference is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Reference must be between 5 and 50 characters")]
        public string TransactionRef { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid Account ID")]
        public int AccountId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid Merchant ID")]
        public int MerchantId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid Device ID")]
        public int DeviceId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be 3 characters")]
        public string Currency { get; set; }

        public DateTime Timestamp { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public Account? Account { get; set; }
        public Merchant? Merchant { get; set; }
        public Device? Device { get; set; }

        public ICollection<FraudAlert>? FraudAlerts { get; set; }
        public ICollection<TransactionFeature>? TransactionFeatures { get; set; }
        public ICollection<ModelScore>? ModelScores { get; set; }
        public ICollection<Feedback>? Feedbacks { get; set; }
    }
}