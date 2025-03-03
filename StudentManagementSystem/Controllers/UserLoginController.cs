using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using StudentManagementSystem.Models;
using Repositories.Models;

namespace StudentManagementSystem.Controllers
{
    public class UserLoginController : Controller
    {
        private readonly ILogger<UserLoginController> _logger;
        private readonly IUserLoginInterface _userLogin;

        public UserLoginController(ILogger<UserLoginController> logger, IUserLoginInterface userLogin)
        {
            _logger = logger;
            _userLogin = userLogin;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
public async Task<IActionResult> CommonLogin(t_Login Login)
{
    if (!ModelState.IsValid)
    {
        return Json(new { success = false, message = "Invalid input data." });
    }

    try
    {
        var studentData = await _userLogin.Login(Login);
        if (studentData != null && studentData.c_studentId != 0)
        {
            // Store data in session
            HttpContext.Session.SetInt32("StudentId", studentData.c_studentId);
            HttpContext.Session.SetString("StudentName", studentData.c_studentName);
            HttpContext.Session.SetString("StudentEmail", studentData.c_studentEmail);
            HttpContext.Session.SetString("UserRole", "Student");

            _logger.LogInformation("User {StudentId} logged in successfully.", studentData.c_studentId);

            // Return data to be stored in sessionStorage on frontend
            return Json(new
            {
                success = true,
                message = "Login Successful",
                studentData = studentData,
                // studentId = studentData.c_studentId,
                // studentName = studentData.c_studentName,
                // email = studentData.c_studentEmail,
                role = "Student"
            });
        }
        else
        {
            _logger.LogWarning("Login attempt failed for email: {Email}", Login.c_StudentEmail);
            return Json(new { success = false, message = "Username or password incorrect" });
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred during login for email: {Email}", Login.c_StudentEmail);
        return Json(new { success = false, message = "An error occurred. Please try again later." });
    }
}


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
