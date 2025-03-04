using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Models;
using StudentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Repositories.Models.t_Teacher;

public class AdminController : Controller
{
    private readonly IAdminInterface _admin;

    public AdminController(IAdminInterface admin)
    {
        _admin = admin;
    }

    public ActionResult Index()
    {
        return View();
    }

    public IActionResult AddSyllabus()
    {
        return View();
    }
    public ActionResult ManageSchedule()
    {
        return View();
    }

    public ActionResult CreateSchedule()
    {
        return View();
    }

    public async Task<List<t_topic>> GetTopicNames()
    {
        List<t_topic> tt = await _admin.GetTopicNames();
        return tt;
    }

     public ActionResult SentNotification()
    {
        return View();
    }

    public async Task<List<t_Student>> GetAllStudent()
    {
        List<t_Student> st = await _admin.GetAllStudents();
        return st;
    }
    public ActionResult Notifications()
    {
        return View();
    }
    [HttpGet("getStudentClasses")]
    public async Task<IActionResult> GetAllClass()
    {
        try
        {
            var classes = await _admin.GetAllClass();
            return Ok(classes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpGet("GetAllSubjects")]
    public async Task<List<t_subject>> GetAllSubjects()
    {
        return await _admin.GetAllSubjects();
    }

    [HttpGet("GetAllTeachers")]
    public async Task<List<t_teacherGet>> GetAllTeachers()
    {
        return await _admin.GetAllTeachers();
    }

    [HttpPost("AssignSubject")]
    public async Task<IActionResult> AssignClassSub([FromForm] t_teacher_Assign teacher)
    {
        var res = await _admin.AssignSubClass(teacher);
        return Json(res);
    }

    [HttpGet("GetSchedules")]
    public async Task<IActionResult> DispTimeTable(int classId)
    {
        Console.WriteLine("DispTimeTable AdminController");
        List<t_Timetable> timetableList = await _admin.DispTimeTable(classId);
        if (timetableList != null)
        {
            return Ok(new { success = true, timetableList = timetableList });
        }

        return BadRequest(new { success = false, message = "Error while fetching timetable." });
    }

    [HttpPost("CreateSchedule")]
    public async Task<IActionResult> CreateTimeTable([FromForm] t_Timetable timetable)
    {
        Console.WriteLine("CreateTimeTable AdminController");
        if (!ModelState.IsValid)
        {
            Console.WriteLine("Invalid");
            return BadRequest(ModelState); // Return validation errors
        }
        try
        {
            var res = await _admin.CreateTimeTable(timetable);
            Console.WriteLine("Success");
            return Ok(new { success = res == 1 });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AdminController {ex.Message}");
            return StatusCode(500, "An error occurred while creating the schedule: " + ex.Message);
        }
    }

    [HttpPost("UpdateSchedule")]
    public async Task<IActionResult> UpdateTimeTable([FromBody] t_Timetable timetable)
    {
        if (timetable == null)
        {
            return BadRequest("Invalid request body");
        }
        try
        {
            // timetable.c_starttime = DateTime.Parse(timetable.c_starttime).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            // timetable.c_endtime = DateTime.Parse(timetable.c_endtime).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var res = await _admin.UpdateTimeTable(timetable);
            return Ok(new { success = res == 1 });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while updating the schedule: " + ex.Message);
        }
    }

    [HttpDelete("DeleteSchedule")]
    public async Task<IActionResult> DeleteTimeTable(int id)
    {
        var res = await _admin.DeleteTimeTable(id);
        return Ok(new { success = res == 1 });
    }

    [HttpPost("SendNotification")]
    public async Task<IActionResult> SendNotification([FromForm] NotificationModel notification)
    {
        if (notification == null)
            return BadRequest("Invalid notification data.");

        bool isSent = await _admin.SendNotification(notification);

        if (isSent)
            return Ok(new { success = true, message = "Notification sent successfully" });
        else
            return StatusCode(500, "Failed to send notification");
    }

    // (B) Get Unread Notification Count
    [HttpGet("GetUnreadNotificationCount/{userId}")]
    public async Task<IActionResult> GetUnreadNotificationCount(int userId)
    {
        int count = await _admin.GetUnreadNotificationCount(userId);
        return Ok(new { count });
    }

    // (C) Get All Notifications for User
    [HttpGet("GetAllNotifications/{userId}")]
    public async Task<IActionResult> GetAllNotifications(int userId)
    {
        var notifications = await _admin.GetAllNotifications(userId);
        return Ok(notifications);
    }

    // (D) Mark Notification as Read
    [HttpPost("MarkAsRead/{notificationId}")]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        bool isMarked = await _admin.MarkAsRead(notificationId);

        if (isMarked)
            return Ok(new { success = true });
        else
            return StatusCode(500, "Failed to mark notification as read");
    }

    [HttpPost("CheckSlotAvailability")]
    public async Task<IActionResult> CheckSlotAvailability([FromForm] SlotAvailabilityRequest request)
    {
        try
        {
            Console.WriteLine(TimeSpan.Parse(request.StartTime));
            var isAvailable = await _admin.IsSlotAvailableAsync(
                request.StartTime, request.EndTime, request.ClassId, request.TeacherId, request.weekday);

            return Ok(new { isAvailable });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while checking slot availability." });
        }
    }

    public async Task<IActionResult> CreateSyllabus([FromBody] t_syllabus syllabus)
    {
        if (syllabus == null)
        {
            Console.WriteLine("Received NULL syllabus object.");
            return Json(0); // Return 0 if data is null
        }

        Console.WriteLine($"Received Data - Class ID: {syllabus.c_class_id}, Subject ID: {syllabus.c_subject_id}, Topics: {syllabus.c_topicName}");

        var res = await _admin.CreateSyllabus(syllabus);
        return Json(res);
    }
    
    public async Task<IActionResult> UpdateSyllabus([FromForm] t_syllabus syllabus)
    {
        var res = await _admin.UpdateSyllabus(syllabus);
        return Json(res);
    }

    public async Task<List<t_syllabus>> DispSyllabus()
    {
        List<t_syllabus> ts = await _admin.DispSyllabus();
        return ts;
    }

    public async Task<IActionResult> DeleteSyllabus(int id)
    {
        var res = await _admin.DeleteSyllabus(id);
        return Json(res);
    }

    public async Task<IActionResult> EntryCompleteTopics(t_syllabuscomplete completeDetails)
    {
        var res = await _admin.EntryCompletedTopics(completeDetails);
        return Json(res);
    }

    public async Task<List<t_syllabuscomplete>> DispCompleteTopics()
    {
        List<t_syllabuscomplete> tt = await _admin.DispSyllabusProgress();
        return tt;
    }

    public async Task<IActionResult> UpdateSyllabusProgress(t_syllabuscomplete completeDetails)
    {
        var res = await _admin.UpdateSyllabusProgress(completeDetails);
        return Json(res);
    }
}
