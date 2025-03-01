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
        [HttpGet]
        public async Task<List<t_Student>> GetAll()
        {
            List<t_Student> student = new List<t_Student>();

            student = await _student.GetAll();
            return student;
        }
        [HttpPost]
        public async Task<ActionResult> Create([FromForm] t_Student student)
        {

            int result = await _student.Add(student);
            if (result > 0)
            {
                return RedirectToAction("List");
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

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            System.Console.WriteLine("Delete controller called");
            var status = await _student.Delete(id);
            if (status == 1)
            {
                ViewData["Message"] = "Contact Delete successfully";
                // return RedirectToAction("List", "Task");
                return Ok(status);
            }
            else
            {
                ViewData["Message"] = "There is some error";
                // return RedirectToAction("List", "Task");
                return BadRequest();
            }
        }

        public async Task<IActionResult> GetClasses()
        {
            var rooms = await _student.GetClasses();
            return Ok(rooms);
        }

        public IActionResult GetSectionsByClassId(int id)
        {
            var Cupboard = _student.GetSectionsByClassId(id);
            return Ok(Cupboard);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}