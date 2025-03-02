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
        private readonly IAdminInterface _student;

        public StudentController(IAdminInterface student)
        {
            _student = student;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> StudentList()
        {
            var studentList = await _student.GetAll();
            return Ok(studentList);
        }

        
    }
}