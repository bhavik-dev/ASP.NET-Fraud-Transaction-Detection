using Microsoft.AspNetCore.Mvc;
using Project2Delivery2.Models;

namespace Project2Delivery2.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly long _maxFileSize = 10 * 1024 * 1024; // 10 MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };

        public FileUploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        // GET: FileUpload/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: FileUpload/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                TempData["ErrorMessage"] = "Please select at least one file to upload.";
                return RedirectToAction("Index");
            }

            var uploadedFiles = new List<string>();
            var errors = new List<string>();

            foreach (var file in files)
            {
                // Check file size
                if (file.Length > _maxFileSize)
                {
                    errors.Add($"{file.FileName}: File size exceeds 10MB limit");
                    continue;
                }

                // Check file extension
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                {
                    errors.Add($"{file.FileName}: File type not allowed. Allowed types: PDF, JPG, PNG, DOCX");
                    continue;
                }

                // Check for empty file
                if (file.Length == 0)
                {
                    errors.Add($"{file.FileName}: File is empty");
                    continue;
                }

                try
                {
                    // Create uploads folder if it doesn't exist
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueFileName = Guid.NewGuid().ToString() + extension;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    uploadedFiles.Add(file.FileName);
                }
                catch (Exception ex)
                {
                    errors.Add($"{file.FileName}: Upload failed - {ex.Message}");
                }
            }

            // Set success/error messages
            if (uploadedFiles.Count > 0)
            {
                TempData["SuccessMessage"] = $"Successfully uploaded {uploadedFiles.Count} file(s): {string.Join(", ", uploadedFiles)}";
            }

            if (errors.Count > 0)
            {
                TempData["ErrorMessage"] = string.Join("<br/>", errors);
            }

            return RedirectToAction("Index");
        }
    }
}