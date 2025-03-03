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
    public class TeacherRatingController : Controller
    {
        private readonly ILogger<TeacherRatingController> _logger;

        private readonly ITeacherRating _teacherRatingRepository;

        public TeacherRatingController(ILogger<TeacherRatingController> logger, ITeacherRating teacherRatingRepository)
        {
            _teacherRatingRepository = teacherRatingRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
public async Task<IActionResult> GetTeachersByStudentId(int studentId)
{
    try
    {
        if (studentId <= 0)
        {
            return BadRequest("Invalid student ID.");
        }

        // Fetch classId based on studentId
        var classId = await _teacherRatingRepository.GetTeachersByClassIdAsync(studentId);
        if (classId == null)
        {
            return NotFound("Student not found or class ID missing.");
        }
        return Ok(GetTeachersByStudentId);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error retrieving teachers: {ex.Message}");
    }
}


[HttpPost]
public async Task<IActionResult> InsertTeacherRating(t_TeacherRating teacherRating)
{
    // Log incoming request
    Console.WriteLine($"Received Rating: {teacherRating.c_rating}");

    if (teacherRating.c_rating < 1 || teacherRating.c_rating > 5)
    {
        return BadRequest("Rating must be between 1 and 5.");
    }

    try
    {
        var result = await _teacherRatingRepository.InsertTeacherRatingAsync(
            teacherRating.c_stud_id, 
            teacherRating.c_teacher_id, 
            teacherRating.c_rating
        );

        if (result == null)
            return BadRequest("Failed to insert rating. Ensure the class has an assigned teacher.");

        return Ok(result);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error inserting teacher rating: {ex.Message}");
    }
}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}