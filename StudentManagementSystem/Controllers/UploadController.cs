using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Repositories.Interfaces;
using Repositories.Models;

namespace mvc.Controllers
{
    public class UploadController : Controller
    {
        private readonly ITeacherInterface _teacherRepository;

        public UploadController(ITeacherInterface teacherInterface)
        {
            _teacherRepository = teacherInterface;
        }

        // GET: Upload/Index
        public IActionResult Index()
        {
            return View(); // Returns the Index view
        }

        [HttpPost]
        public async Task<IActionResult> AddMaterial([FromForm] t_material materialData, [FromForm] IFormFile File)
        {
            Console.WriteLine("Received API Request for AddMaterial");

            // Check if the file is null or empty
            if (File == null || File.Length == 0)
            {
                return BadRequest(new { success = false, message = "No file uploaded" });
            }

            Console.WriteLine("Received file: " + File.FileName); // Log file name for debugging

            // Validate the model
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine("Model Error: " + error.ErrorMessage);
                    }
                }
                return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });
            }

            try
            {
                // Ensure Upload Directory Exists
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "StudentManagementSystem/wwwroot", "Teaching_Material");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Console.WriteLine("Created directory: " + uploadsFolder);
                }

                // Generate a unique file name
                string fileExtension = Path.GetExtension(File.FileName);
                 string fileName = File.FileName;
                // string fileName = Guid.NewGuid().ToString() + fileExtension;
                string filePath = Path.Combine(uploadsFolder, fileName);

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await File.CopyToAsync(stream);
                    Console.WriteLine("File saved at: " + filePath);
                }

                // Set the FilePath in materialData
                materialData.FilePath = "/Teaching_Material/" + fileName; // Relative path to store in DB
                Console.WriteLine("Material Data FilePath: " + materialData.FilePath);

                // Save the material data (e.g., to a database)
                var result = await _teacherRepository.Add_Material(materialData);

                if (result > 0)
                {
                    return Ok(new { success = true, message = "Material uploaded successfully!" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Error while adding the material" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Server error", error = ex.Message });
            }
        }


        // Error handling view (optional)
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
