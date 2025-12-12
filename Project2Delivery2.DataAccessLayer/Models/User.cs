using System.ComponentModel.DataAnnotations;

namespace Project2Delivery2.DataAccessLayer.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<FraudAlert>? AssignedAlerts { get; set; }
        public ICollection<Feedback>? CreatedFeedbacks { get; set; }
    }
}
