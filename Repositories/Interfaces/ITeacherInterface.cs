using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Models;
using StudentManagementSystem.Models;

namespace Repositories.Interfaces
{
    public interface ITeacherInterface
    {
        Task<int> Register(t_Teacher teacherData);

        Task<int> Add_Material(t_material materialData);

        Task<List<t_student_view>> GetStudentsByTeacherId(int id);

        Task<List<t_classschedule>> GetUpcomingClassesForTeacher(int teacherId);

        Task<t_material> GetLatestUploadedFile(int teacherId);


        Task<int> EntryCompletedTopics(t_syllabuscomplete completeDetails);

        Task<List<t_syllabuscomplete>> DispSyllabusProgress();

        Task<int> UpdateSyllabusProgress(t_syllabuscomplete completeDetails);

        Task<List<t_topic>> GetTopicNames(int id);

        Task<List<t_Class>> GetAllClass(int id);
        Task<List<t_subject>> GetAllSubjectsByClass(int id);
    }
}