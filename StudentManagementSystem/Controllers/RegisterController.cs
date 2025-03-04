using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Repositories.Models;

namespace StudentManagementSystem.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ITeacherInterface _teacherInterface;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(ITeacherInterface teacherInterface, ILogger<RegisterController> logger)
        {
            _teacherInterface = teacherInterface;
            _logger = logger;
        }

        // ✅ GET: Load Registration Form
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Ensure this view exists
        }

        // ✅ POST: Register Teacher
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> RegisterTeacher([FromBody] t_Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid data",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var result = await _teacherInterface.Register(teacher);

            if (result == 1)
            {
                HttpContext.Session.SetInt32("TeacherId", teacher.TeacherId);
                // HttpContext.Session.SetString("UserName", UserData.c_UserName);
                return Json(new { success = true, message = "Registration Successful" });
            }
            else
            {
                return Json(new { success = false, message = "Database insertion failed." });
            }
        }


        // ✅ GET: Success Page
        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }
    }
}
