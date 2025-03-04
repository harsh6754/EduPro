using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Repositories.Models;

namespace StudentManagementSystem.Controllers
{
    public class StudentDashboardController : Controller
    {
        private readonly ILogger<StudentDashboardController> _logger;
        private readonly ITeacherRating _teach;


        public StudentDashboardController(ILogger<StudentDashboardController> logger, ITeacherRating teach)
        {
            _logger = logger;
            _teach = teach;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TimeTable()
        {
            return View();
        }

        public IActionResult UpcomingExam()
        {
            return View();
        }

        public IActionResult TeacherRating()
        {
            return View();
        }

        public IActionResult SyllabusProgress()
        {
            return View();
        }

        public async Task<IActionResult> ViewMaterial()
        {
            // Static Data for Teaching Materials
        //     var teachingMaterials = new List<t_material>
        // {
        //     new t_material { MaterialId = 1, FileName = "Algebra_Basics.pdf", FileType = "PDF", UploadDate = DateTime.Parse("2024-02-10"), SubjectId = 1, FilePath = "/Content/Materials/Algebra_Basics.pdf", TeacherId = 1 },
        //     new t_material { MaterialId = 2, FileName = "Newton_Laws.docx", FileType = "DOCX", UploadDate = DateTime.Parse("2024-02-15"), SubjectId = 2, FilePath = "/Content/Materials/Newton_Laws.docx", TeacherId = 1 },
        //     new t_material { MaterialId = 3, FileName = "Grammar_Tips.pdf", FileType = "PDF", UploadDate = DateTime.Parse("2023-12-05"), SubjectId = 3, FilePath = "/Content/Materials/Grammar_Tips.pdf", TeacherId = 1 },
        //     new t_material { MaterialId = 4, FileName = "Trigonometry.pdf", FileType = "PDF", UploadDate = DateTime.Parse("2023-11-20"), SubjectId = 2, FilePath = "/Content/Materials/Trigonometry.pdf", TeacherId = 1 },
        //     new t_material { MaterialId = 5, FileName = "Chemistry_101.pdf", FileType = "PDF", UploadDate = DateTime.Parse("2022-06-15"), SubjectId = 1, FilePath = "/Content/Materials/Chemistry_101.pdf", TeacherId = 1 }
        // };

            var teachingMaterials = await _teach.GetLatestUploadedFile();

            ViewBag.TeachingMaterials = new List<t_material> { teachingMaterials };
            return View();
        }

        public IActionResult Logout()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}