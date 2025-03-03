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
    public class TeacherLogin : Controller
    {
        private readonly ILogger<TeacherLogin> _logger;
        private readonly IUserLoginInterface _userLogin;

        public TeacherLogin(ILogger<TeacherLogin> logger, IUserLoginInterface userLogin)
        {
            _userLogin = userLogin;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TeacherLogins(t_TeacherLogin TeacherLogin){
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid input data." });
            }
            try{
                var teacherData = await _userLogin.TeacherLogin(TeacherLogin);
                if(teacherData != null && teacherData.TeacherId != 0){
                    HttpContext.Session.SetInt32("TeacherId", teacherData.TeacherId);
                    HttpContext.Session.SetString("T_Name", teacherData.T_Name);

                    _logger.LogInformation("User {TeacherId} logged in successfully.", teacherData.TeacherId);
                    return Json(new { 
                    success = true, 
                    message = "Teacher Login Successful",
                    teacherData = teacherData,
                    role = "Teacher"
                    });
                }
                else
                {
                    _logger.LogWarning("Login attempt failed for email: {Email}",TeacherLogin.T_Email);
                    return Json(new { success = false, message = "Username or password incorrect" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for email: {Email}", TeacherLogin.T_Email);
                return Json(new { success = false, message = "An error occurred. Please try again later." });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}