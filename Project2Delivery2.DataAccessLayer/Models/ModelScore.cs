using System.ComponentModel.DataAnnotations;

namespace Project2Delivery2.DataAccessLayer.Models
{
    public class ModelScore
    {
        [Key]
        public long ModelScoreId { get; set; }
        public long TransactionId { get; set; }
        public int ModelVersionId { get; set; }
        public decimal Score { get; set; }
        public string Explanation { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
