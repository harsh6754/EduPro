using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagementSystem.Models;


namespace Repositories.Interfaces
{
    public interface IAdminInterface
    {
         Task<int> AddStudent(t_Student student); // ✅

        // Task<int> AssignSubClass(t_teacherUpdate teacher);

        Task<List<t_Class>> GetAllClass();
        Task<List<t_subjects>> GetAllSubjects();

        Task<List<t_teacherGet>> GetAllTeachers();
    }
}