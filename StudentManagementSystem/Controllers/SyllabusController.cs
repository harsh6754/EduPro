using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Repositories.Models;
using StudentManagementSystem.Models;

namespace mvc.Controllers
{
    // [Route("[controller]")]
    public class SyllabusController : Controller
    {
        private readonly ILogger<SyllabusController> _logger;
        private readonly ITeacherInterface _teacherRepository;
        public SyllabusController(ILogger<SyllabusController> logger, ITeacherInterface teacher)
        {
            _logger = logger;
            _teacherRepository = teacher;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<List<t_Class>> GetClassName(int id)
        {
            List<t_Class> tt = await _teacherRepository.GetAllClass(id);
            Console.WriteLine(tt);
            return tt;
        }

        public async Task<List<t_subject>> GetSubjectByClass(int id)
        {
            List<t_subject> tt = await _teacherRepository.GetAllSubjectsByClass(id);
            return tt;
        }

        public async Task<List<t_topic>> GetTopicNames(int id)
        {
            List<t_topic> tt = await _teacherRepository.GetTopicNames(id);
            return tt;
        }
[HttpPost]
public async Task<IActionResult> EntryCompleteTopics([FromBody] t_syllabuscomplete completeDetails)
{
    Console.WriteLine(completeDetails);
    
    if (completeDetails == null || completeDetails.c_class_id == 0)
    {
        Console.WriteLine("❌ Error: Invalid data received.");
        return BadRequest("Invalid data received.");
    }

    var res = await _teacherRepository.EntryCompletedTopics(completeDetails);
    return Json(res);
}

        // public async Task<IActionResult> EntryCompleteTopics(t_syllabuscomplete completeDetails)
        // {
        //     Console.WriteLine(completeDetails);
        //     var res = await _teacherRepository.EntryCompletedTopics(completeDetails);
        //     return Json(res);
        // }

        public async Task<List<t_syllabuscomplete>> DispCompleteTopics()
        {
            List<t_syllabuscomplete> tt = await _teacherRepository.DispSyllabusProgress();
            return tt;
        }

        public async Task<IActionResult> UpdateSyllabusProgress(t_syllabuscomplete completeDetails)
        {
            var res = await _teacherRepository.UpdateSyllabusProgress(completeDetails);
            return Json(res);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}