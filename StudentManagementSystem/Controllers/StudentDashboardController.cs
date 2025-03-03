using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace StudentManagementSystem.Controllers
{
    public class StudentDashboardController : Controller
    {
        private readonly ILogger<StudentDashboardController> _logger;

        public StudentDashboardController(ILogger<StudentDashboardController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TimeTable()
        {
            return View();
        }

        public IActionResult UpcomingExam(){
            return View();
        }

        public IActionResult TeacherRating(){
            return View();
        }

        public IActionResult SyllabusProgress(){
            return View();
        }

  
        public IActionResult Logout(){
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}