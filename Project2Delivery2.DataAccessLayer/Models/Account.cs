using System.ComponentModel.DataAnnotations;

namespace Project2Delivery2.DataAccessLayer.Models
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string HolderName { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
