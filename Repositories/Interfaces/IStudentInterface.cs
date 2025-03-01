using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagementSystem.Models;

namespace Repositories.Interfaces
{
    public interface IStudentInterface
    {
        Task<List<t_Student>> GetAll();
        Task<List<t_Student>> GetAllByUser(string studentid);
        Task<t_Student> GetOne(string studentId);
        Task<int> Add(t_Student studentData);
        Task<int> Update(t_Student studentData);
        Task<int> Delete(int studentid);
        Task<List<t_Class>> GetClasses();
        Task<List<t_Section>> GetSectionsByClassId(int id);
    }
}