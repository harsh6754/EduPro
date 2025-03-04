using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduProj.Models;
using Repositories.Models;
using StudentManagementSystem.Models;

namespace Repositories.Interfaces
{
    public interface IStudentInterface
    {
        Task<List<t_Student>> GetAll();
        Task<t_Student> GetOne(string studentId);
        Task<int> Add(t_Student studentData);
        Task<int> Update(t_Student studentData);
        Task<int> Delete(int studentid);
        Task<List<t_Class>> GetClasses();
        Task<List<t_Section>> GetSectionsByClassId(int id);

        Task<List<t_subject>> GetAllSubjects();
         Task<List<t_Teacher>> GetAllTeachers();
        Task<List<vm_Material>> GetAllMaterials();
        Task<vm_Material> GetMaterialById(int id);
        Task<List<vm_Material>> GetMaterialsBySubjectIds(List<int> subjectIds);
        Task<List<t_Teacher>> GetTeachersByIds(IEnumerable<int> teacherIds);
    }
}