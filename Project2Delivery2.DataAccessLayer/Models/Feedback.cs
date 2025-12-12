using System.ComponentModel.DataAnnotations;

namespace Project2Delivery2.DataAccessLayer.Models
{
    public class Feedback
    {
        [Key]
        public long FeedbackId { get; set; }
        public long RelatedId { get; set; }
        public string RelatedType { get; set; }
        public string Label { get; set; }
        public string Notes { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
