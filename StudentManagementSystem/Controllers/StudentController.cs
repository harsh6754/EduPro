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
    // [Route("[controller]")]
    public class StudentController : Controller
    {

        private readonly IAdminInterface _adminInterface;

        public StudentController(IAdminInterface adminInterface)
        {
            _adminInterface = adminInterface;
        }

        public async Task<ActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("c_studentId") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #region KendoIndex
        public async Task<ActionResult> KendoIndex()
        {
            if (HttpContext.Session.GetInt32("c_studentId") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region KendoStudentList
        public async Task<ActionResult> GetAllStudents(t_Student student)
        {
            List<t_Student> students = await _adminInterface.GetAllStudents();
            return Json(students);
        }

        #endregion

        #region InsertStudent
        [HttpPost]
        public async Task<ActionResult> AddStudent(t_Student student)
        {
            if (ModelState.IsValid)
            {
                if (student.StudentPic != null && student.StudentPic.Length > 0)
                {
                    var filename = student.c_studentEmail + Path.GetExtension(student.StudentPic.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", filename);
                    if (!Directory.Exists(Path.GetDirectoryName(path)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    }

                    var filePath = Path.Combine(path, filename);
                    student.c_studentProfile = filePath;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await student.StudentPic.CopyToAsync(stream);
                    }
                }else{

                }
                // student.c_studentId = (int)HttpContext.Session.GetInt32("c_studentId");
                var result = 0;
                if (student.c_studentId == 0)
                {
                    result = await _adminInterface.AddStudent(student);
                }
                if (result == 0)
                {
                    return BadRequest(new { success = false, message = "There was some error while adding the contact" });
                }
                else
                {
                    return Ok(new { success = true, message = "contact Insterted Successfully!!!!!" });
                }
            }
            else
            {
                return BadRequest(new { success = false, message = "Please enter all the required fields" });
            }
            var errors = ModelState.Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(err => err.ErrorMessage).ToArray()
                    );
            return BadRequest(new { success = false, message = errors });
        }
        #endregion

        #region GetAllClasses
        public async Task<ActionResult> GetAllClasses()
        {
            List<t_Class> classes = await _adminInterface.GetAllClasses();
            return Json(classes);
        }
        #endregion

        #region GetAllSections
        public async Task<ActionResult> GetAllSections()
        {
            List<t_Section> sections = await _adminInterface.GetAllSections();
            return Json(sections);
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}