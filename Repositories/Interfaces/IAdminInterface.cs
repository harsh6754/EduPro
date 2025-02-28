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

    }
}