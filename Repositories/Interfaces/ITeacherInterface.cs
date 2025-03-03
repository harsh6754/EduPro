using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Interfaces
{
    public interface ITeacherInterface
    {
          Task<int> Register(t_teacher teacherData);

          Task<int> Add_Material(t_material materialData);

           Task<List<t_student_view>> GetStudentsByTeacherId(int id);

           Task<List<t_classschedule>> GetUpcomingClassesForTeacher(int teacherId);
    }
}