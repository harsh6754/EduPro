using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Repositories.Models;
using System;

namespace StudentManagementSystem.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ILogger<ProfileController> logger)
        {
            _logger = logger;
        }
public IActionResult Index()
        {
            return View();
        }
       [HttpGet]
public IActionResult TeacherDashboard()
{
    var teacherDataJson = HttpContext.Session.GetString("TeacherData");

    if (string.IsNullOrEmpty(teacherDataJson))
    {
        return RedirectToAction("Login");
    }

    var teacherData = JsonConvert.DeserializeObject<t_Teacher>(teacherDataJson); // Deserialize back to object

    return View(teacherData);
}

}
}