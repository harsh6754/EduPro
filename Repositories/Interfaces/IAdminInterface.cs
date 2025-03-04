using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Models;
using StudentManagementSystem.Models;
using static Repositories.Models.t_Teacher;

namespace Repositories.Interfaces
{
    public interface IAdminInterface
    {
        Task<List<t_Exam>> GetAllETimetableData();
        Task<t_Exam> GetETimetableData(int classid);
        Task<int> Update(t_Exam exam);
        Task<int> Add(t_Exam  examVM);
        Task<int> Delete(int classid);
        Task<List<t_Class>> GetAllClass();


        Task<int> AddStudent(t_Student student); // ✅

        Task<int> AssignSubClass(t_teacher_Assign teacher);

        Task<List<t_subject>> GetAllSubjects();

        Task<List<t_teacherGet>> GetAllTeachers();

        Task<int> CreateTimeTable(t_Timetable timetable);

        Task<int> UpdateTimeTable(t_Timetable timetable);
    
        Task<int> DeleteTimeTable(int id);

        Task<List<t_Timetable>> DispTimeTable(int classid);

        Task<bool> SendNotification(NotificationModel notification);

        Task<int> GetUnreadNotificationCount(int userId);  // (B) Get Unread Notification Count
        Task<List<NotificationModel>> GetAllNotifications(int userId);  // (C) Get All Notifications
        Task<bool> MarkAsRead(int notificationId); 

        Task<bool> IsSlotAvailableAsync(string startTime, string endTime, int classId, int teacherId, string weekday);

        Task<int> CreateSyllabus(t_syllabus syllabus);

        Task<int> UpdateSyllabus(t_syllabus syllabus);

        Task<List<t_syllabus>> DispSyllabus();

        Task<int> DeleteSyllabus(int id);

        Task<int> EntryCompletedTopics(t_syllabuscomplete completeDetails);

        Task<List<t_syllabuscomplete>> DispSyllabusProgress();

        Task<int> UpdateSyllabusProgress(t_syllabuscomplete completeDetails);

        Task<List<t_topic>> GetTopicNames();
        Task<List<t_Student>> GetAllStudents();

                List<TeacherTreeViewModel> GetTeachersWithStudents();
        Task<List<t_Student>> GetStudentCountPClass();

    }
}