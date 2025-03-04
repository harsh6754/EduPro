using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Repositories.Models;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ILogger<AdminDashboardController> _logger;
        private readonly IAdminInterface _exam;


        public AdminDashboardController(ILogger<AdminDashboardController> logger,IAdminInterface exam)
        {
            _logger = logger;
            _exam = exam;
        }

        public IActionResult Index()
        {
            return View();
        }



        public IActionResult AddExamTimetable()
        {
            return View();
        }
        public IActionResult ExamTimeTable()
        {
            return View();
        }
        public IActionResult EditExamTimetable()
        {
            return View();
        }

        public IActionResult TeachersList()
        {
            return View();
        }

         public IActionResult StudentCount()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentCountPerClass()
        {
            List<t_Student> data = await _exam.GetStudentCountPClass();
            return Json(data);
        }

         public IActionResult GetTeachersWithStudents()
        {
            List<TeacherTreeViewModel> data = _exam.GetTeachersWithStudents();
            return Json(data);
        }
        public IActionResult DisplayAllETable()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllETables()
        {
            List<t_Exam> exam = await _exam.GetAllETimetableData();
            return Json(exam);
        }
        // public async Task<IActionResult> DisplayETable()
        // {
        //     int ClassId = Convert.ToInt32(HttpContext.Session.GetInt32("ClassId"));
        //     ExamVM examVM = await _exam.GetETimetableData(ClassId);
        //     return Json(examVM);
        // }
        [HttpPost]
        public async Task<IActionResult> Update(t_Exam exam)
        {
            if (ModelState.IsValid)
            {
                var ClassId = exam.CClassId;
                Console.WriteLine("controller classid: "+ClassId);
                HttpContext.Session.SetInt32("ClassId", Convert.ToInt32(ClassId));
                if (exam.Image != null && exam.Image.Length > 0)
                {
                    // Random random = new Random();
                    var fileName = exam.CClassId + "_standard_Timetable" + Path.GetExtension(exam.Image.FileName);
                    var filePath = Path.Combine("./wwwroot/exam_timetable/", fileName);
                    Directory.CreateDirectory(Path.Combine("./wwwroot/exam_timetable/"));
                    exam.CImage = fileName;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        exam.Image.CopyTo(stream);
                    }
                }
                var status = await _exam.Update(exam);
                if (status == 1)
                {
                    return new JsonResult(new { success = true, message = "Timetable Updated Successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Error in updating the timetable" });
                }
            }
            else
            {
                return BadRequest(ModelState); // Return validation errors
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add(t_Exam exam)
        {
            if (ModelState.IsValid)
            {
                var ClassId = exam.CClassId;
                HttpContext.Session.SetInt32("ClassId", Convert.ToInt32(ClassId));
                if (exam.Image != null && exam.Image.Length > 0)
                {
                    var fileName = exam.CClassId + "_standard_Timetable" + Path.GetExtension(exam.Image.FileName);
                    var filePath = Path.Combine("./wwwroot/exam_timetable/", fileName);
                    Directory.CreateDirectory(Path.Combine("./wwwroot/exam_timetable/"));
                    exam.CImage = fileName;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        exam.Image.CopyTo(stream);
                    }
                }
                var status = await _exam.Add(exam);
                if (status == 1)
                {
                    return new JsonResult(new { success = true, message = "Timetable added successfully!" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Error in adding the timetable!" });
                }
            }
            else
            {
                return BadRequest(ModelState); // Return validation errors
            }
        }
        public async Task<IActionResult> DeletExamTimetable(int id)
        {
            if (ModelState.IsValid)
            {
                var status = await _exam.Delete(id);
                if (status == 1)
                {
                    return new JsonResult(new { success = true, message = "Timetable deleted successfully!" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Error in deleting the timetable!" });
                }
            }
            else
            {
                return BadRequest(ModelState); // Return validation errors
            }
        }
        public async Task<IActionResult> GetClasses()
        {
            List<t_Class> classes = await _exam.GetAllClass();
            return Json(classes);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}