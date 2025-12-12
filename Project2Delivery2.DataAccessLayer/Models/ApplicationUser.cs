using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Project2Delivery2.DataAccessLayer.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(20)]
        public string PostalCode { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.Now;
    }
}