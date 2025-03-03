using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Models;
using StudentManagementSystem.Models;

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
    }
}