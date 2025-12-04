using System.ComponentModel.DataAnnotations;

namespace Project2Delivery2.DataAccessLayer.Models
{
    public class AuditLog
    {
        [Key]
        public long AuditId { get; set; }
        public string Entity { get; set; }
        public long EntityId { get; set; }
        public string Action { get; set; }
        public string Data { get; set; }
        public string PerformedBy { get; set; }
        public DateTime PerformedAt { get; set; }
    }
}
