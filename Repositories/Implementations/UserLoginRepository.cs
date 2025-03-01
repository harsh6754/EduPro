using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Models;
using StudentManagementSystem.Models;

namespace Repositories.Implementations
{
    public class UserLoginRepository : IUserLoginInterface
    {
        private readonly NpgsqlConnection _connection;

        public UserLoginRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        #region Login
        public async Task<t_Student> Login(t_Login Login){
            t_Student studentData = new t_Student();
            var qry = "SELECT * FROM t_student WHERE c_email=@c_email AND c_password=@c_password;";
            try{
                using (NpgsqlCommand cmd = new NpgsqlCommand(qry, _connection))
                {
                    cmd.Parameters.AddWithValue("@c_email", Login.c_StudentEmail);
                    cmd.Parameters.AddWithValue("@c_password", Login.c_password);
                    await _connection.OpenAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        studentData.c_studentId = (int)reader["c_id"];
                        studentData.c_studentName = (string)reader["c_name"];
                        studentData.c_studentEmail = (string)reader["c_email"];
                        studentData.c_studentPhone = Convert.ToString(reader["c_mobile_no"]);
                        studentData.c_studentDOB = Convert.ToDateTime(reader["c_DOB"]);
                        studentData.c_studentGender = (string)reader["c_gender"];
                        studentData.c_studentGuardianDetails = (string)reader["c_guardian_name"];
                        studentData.c_studentEnrollDate = Convert.ToDateTime(reader["c_enroll_date"]);
                        studentData.c_studentProfile = (string)reader["c_profile_pic"];
                        studentData.c_studentStatus = (string)reader["c_status"];
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("----------->Login Error : " + e.Message);
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return studentData;
        }
        #endregion
    }
}