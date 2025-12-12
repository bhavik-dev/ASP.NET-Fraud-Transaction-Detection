using System.ComponentModel.DataAnnotations;

namespace Project2Delivery2.DataAccessLayer.Models
{
    public class ModelVersion
    {
        [Key]
        public int ModelVersionId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Metrics { get; set; }
        public string FileLocation { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
