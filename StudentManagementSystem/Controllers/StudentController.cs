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
        public async Task<List<t_Student>> StudentList()
        {
            List<t_Student> student = new List<t_Student>();

            student = await _student.GetAll();
            return student;
        }
        [HttpPost]
        public async Task<ActionResult> Create(t_Student student)
        {
            if (student.StudentPic != null && student.StudentPic.Length > 0)
            {
                var fileName = student.c_studentEmail + Path.GetExtension(student.StudentPic.FileName);
                var filePath = Path.Combine("../StudentManagementSystem/wwwroot/student_images/", fileName);
                Directory.CreateDirectory(Path.Combine("../StudentManagementSystem/wwwroot/student_images/"));
                student.c_studentProfile = fileName;
                System.IO.File.Delete(filePath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    student.StudentPic.CopyTo(stream);
                }
            }
            var result = 0;
            if (student.c_studentId == 0)
            {
                result = await _student.Add(student);
                if (result > 0)
                {
                    return Ok(student);
                }
            }
            else
            {
                result = await _student.Update(student);
            }
            if (result == 0)
            {
                return BadRequest(new { success = false, message = "There was some error while adding the contact" });
            }
            else
            {
                return Ok(new { success = true, message = "Student Insterted Successfully!!!!!" });
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _student.GetOne(id.ToString());
            if (student == null)
            {
                return NotFound();
            }

            ViewBag.Classes = await _student.GetClasses(); // Fetch classes
            ViewBag.Sections = await _student.GetSectionsByClassId(student.c_class.c_classId); // Fetch sections

            return Ok(student);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(t_Student student)
        {
            if (ModelState.IsValid)
            {
                if (student.StudentPic != null)
                {
                    var filePath = Path.Combine("wwwroot/student_images", student.StudentPic.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await student.StudentPic.CopyToAsync(stream);
                    }
                    student.c_studentProfile = student.StudentPic.FileName;
                }

                int result = await _student.Update(student);
                if (result > 0)
                {
                    return Ok("sucess");
                }
                ModelState.AddModelError("", "Failed to update student.");
            }

            ViewBag.Classes = await _student.GetClasses();
            // ViewBag.Sections = await _student.GetSectionsByClassId(student.c_class.c_classId);

            return View(student);
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



    }
}