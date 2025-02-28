using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentInterface _student;

        public StudentController(IStudentInterface student)
        {
            _student = student;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetOne(int id)
        {
            t_Student student = new t_Student();
            if (id != 0)
            {
                student = await _student.GetOne(id.ToString());
            }

            return Ok(student);
        }
        [HttpPost]
        public async Task<ActionResult> Create(t_Student student)
        {
            student.c_studentId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));  // Set before checking ModelState

            if (ModelState.IsValid)
            {
                if (student.c_studentId == 0)  // New Task
                {
                    int result = await _student.Add(student);
                    if (result > 0)
                    {
                        return RedirectToAction("List");
                    }
                }
                else  // Existing Task
                {
                    int result = await _student.Update(student);
                    if (result > 0)
                    {
                        return RedirectToAction("List");
                    }
                }
            }

            foreach (var error in ModelState)
            {
                Console.WriteLine($"Key: {error.Key}");
                foreach (var err in error.Value.Errors)
                {
                    Console.WriteLine($"Error: {err.ErrorMessage}");
                }
            }

            return Ok(student);
        }

        public async Task<ActionResult> List()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Message"] = "Session expired. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            string result = userId.ToString();

            List<t_Student> tasks = await _student.GetAllByUser(userId.ToString());
            return View(tasks);
        }
        public async Task<ActionResult> Delete(string id)
        {

            int status = await _student.Delete(id);
            if (status == 1)
            {
                ViewData["Message"] = "Contact Delete successfully";
                return RedirectToAction("List", "Task");
            }
            else
            {
                ViewData["Message"] = "There is some error";
                return RedirectToAction("List", "Task");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}