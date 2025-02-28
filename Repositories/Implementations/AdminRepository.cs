using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Interfaces;
using StudentManagementSystem.Models;
using Npgsql;

namespace Repositories.Implementations
{
    public class AdminRepository: IAdminInterface
    {
        private readonly NpgsqlConnection _connection;
            public AdminRepository(NpgsqlConnection connection)
            {
                _connection = connection;
            }
        public async Task<int> AddStudent(t_Student student)
        {
            return 1;
        }   
    }
}