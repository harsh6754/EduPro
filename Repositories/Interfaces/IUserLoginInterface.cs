using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Models;
using StudentManagementSystem.Models;

namespace Repositories.Interfaces
{
    public interface IUserLoginInterface
    {
        Task<t_Student> Login(t_Login Login);
    }
}