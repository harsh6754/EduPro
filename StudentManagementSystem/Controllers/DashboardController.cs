using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;

namespace mvc.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ITeacherInterface _teacherRepository;

        public DashboardController(ITeacherInterface teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

         public IActionResult Db()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        // ✅ Get Students for the logged-in Teacher
        [HttpGet]
        public async Task<IActionResult> GetStudentsByTeacher()
        {
            try
            {
                // ✅ Retrieve teacherId from session
                int? teacherId = HttpContext.Session.GetInt32("TeacherId");

                if (teacherId == null)
                {
                    return Unauthorized(new { success = false, message = "Session expired or teacher not logged in." });
                }

                // ✅ Fetch students based on the teacher's class
                var students = await _teacherRepository.GetStudentsByTeacherId(teacherId.Value);
                // int teacherId = 9;

                // ✅ Fetch students based on the teacher's class
                // var students = await _teacherRepository.GetStudentsByTeacherId(teacherId);

                if (students == null || students.Count == 0)
                {
                    return NotFound(new { success = false, message = "No students found for this teacher." });
                }

                return Ok(new { success = true, data = students });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Server error", error = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetUpcomingClasses()
        {
            try
            {
              
            int? teacherId = HttpContext.Session.GetInt32("TeacherId");

                if (teacherId == null)
                {
                    return Unauthorized(new { success = false, message = "Session expired or teacher not logged in." });
                }
                var upcomingClasses = await _teacherRepository.GetUpcomingClassesForTeacher(teacherId.Value);

                if (upcomingClasses == null || upcomingClasses.Count == 0)
                {
                    return NotFound(new { success = false, message = "No upcoming classes found." });
                }

                return Ok(new { success = true, data = upcomingClasses });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Server error", error = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetLatestFile()
        {
            try
            {
 // ✅ Retrieve teacherId from session
                int? teacherId = HttpContext.Session.GetInt32("TeacherId");

                if (teacherId == null)
                {
                    return Unauthorized(new { success = false, message = "Session expired or teacher not logged in." });
                }


                // ✅ Retrieve teacherId from session (set manually if needed)
                // int teacherId =  8; // Temporary hardcoded ID

                var latestFile = await _teacherRepository.GetLatestUploadedFile(teacherId.Value);

                if (latestFile != null)
                {
                    return Ok(latestFile);
                }

                return NotFound(new { success = false, message = "No files found for this teacher." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Server error", error = ex.Message });
            }
        }
        


    }
}
