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
   
    public class TeacherDashboard : Controller
    {
        private readonly ILogger<TeacherDashboard> _logger;

        private readonly IAdminInterface _admin;

        public TeacherDashboard(ILogger<TeacherDashboard> logger, IAdminInterface admin)
        {
            _logger = logger;
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
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}