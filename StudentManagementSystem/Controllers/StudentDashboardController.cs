using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EduProj.Models;
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
        private readonly IAdminInterface _admin;

        private readonly IStudentInterface _student;



        public StudentDashboardController(ILogger<StudentDashboardController> logger, ITeacherRating teach, IStudentInterface student,IAdminInterface admin)
        {
            _logger = logger;
            _teach = teach;
            _student = student;
            _admin = admin;
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
        

            var teachingMaterials = await _teach.GetLatestUploadedFile();

            ViewBag.TeachingMaterials = new List<t_material> { teachingMaterials };
            return View();
        }

            [HttpGet]
        public async Task<IActionResult> GetAllETables()
        {
            List<t_Exam> exam = await _admin.GetAllETimetableData();
            return Json(exam);
        }



        public ActionResult ManageSchedule()
    {
        return View();
    }
        

        public IActionResult Logout()
        {
            return View();
        }



        [HttpGet]
         public async Task<IActionResult> Material()
        {
            // var classid = HttpContext.Session.GetString("ClassId");
            var materials = await _student.GetAllMaterials(); // Await the task
                                                              // Fetch all subjects and store in ViewBag
            var subjects = await _student.GetAllSubjects();
            ViewBag.Subjects = subjects;

            // Fetch all teachers and store in ViewBag
            var teachers = await _student.GetAllTeachers();
            ViewBag.Teachers = teachers;
            return View(materials);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadMaterial(int id)
        {
            var material = await _student.GetMaterialById(id); // Await the Task
            if (material == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine("wwwroot/Teaching_Material", material.c_fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/octet-stream", material.c_fileName);
        }

        [HttpPost]
        public async Task<IActionResult> FilterMaterials(List<int> subjectIds)
        {
            if (subjectIds == null || subjectIds.Count == 0)
            {
                return PartialView("_MaterialsPartial", new List<vm_Material>()); // Return empty if no subjects selected
            }

            var materials = await _student.GetMaterialsBySubjectIds(subjectIds);

            // Fetch only teachers related to the filtered materials
            var teacherIds = materials
                .Select(m => m.c_teacher_id)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct();

            var teachers = await _student.GetTeachersByIds(teacherIds.ToList());
            ViewBag.Teachers = teachers;

            return PartialView("_MaterialsPartial", materials);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}