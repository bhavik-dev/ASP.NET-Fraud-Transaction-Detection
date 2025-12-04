using System.ComponentModel.DataAnnotations;

namespace Project2Delivery2.Models
{
    public class FileUploadViewModel
    {
        [Required(ErrorMessage = "Please select at least one file")]
        public List<IFormFile> Files { get; set; }

        public string Category { get; set; }
    }
}