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

                    t_Class t_class = new t_Class();
                    t_Section t_section = new t_Section();
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
                        studentData.c_class = new t_Class{
                            c_classId = (int)reader["c_classid"]
                        };

                        studentData.c_section = new t_Section{
                            c_sectionId = (int)reader["c_sectionid"]
                        };
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

        #region TeacherLogin
        public async Task<t_Teacher> TeacherLogin(t_TeacherLogin TeacherLogin){
            t_Teacher teacherData = new t_Teacher();
            var qry = "SELECT * FROM t_teachers WHERE c_temail = @c_temail AND c_tpassword = @c_tpassword;";
            try{
                using (NpgsqlCommand cmd = new NpgsqlCommand(qry, _connection)){
                    cmd.Parameters.AddWithValue("@c_temail", TeacherLogin.T_Email);
                    cmd.Parameters.AddWithValue("@c_tpassword", TeacherLogin.T_PasswordHash);
                    await _connection.OpenAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    if(reader.Read()){
                        teacherData.TeacherId = (int)reader["c_tid"];
                        teacherData.T_Name = (string)reader["c_teachername"];
                        teacherData.T_Email = (string)reader["c_temail"];
                        teacherData.T_PasswordHash = (string)reader["c_tpassword"];
                        teacherData.T_MobileNumber = Convert.ToInt64(reader["c_tmobno"]);
                        teacherData.T_DateOfBirth = Convert.ToDateTime(reader["c_tdob"]);
                        teacherData.T_Qualification = (string)reader["c_tqualification"];
                        teacherData.T_Experience = (int)reader["c_experience"];
                        teacherData.T_ExpertSubject = (string)reader["c_expert_subject"];
                        teacherData.T_SubjectId = (int)reader["c_subjectid"];
                        teacherData.T_Class_Id = (int)reader["c_class_id"];
                    }
                }
            }
            catch (Exception e){
                Console.WriteLine("----------->Teacher Login Error : " + e.Message);
            }
            finally{
                await _connection.CloseAsync();
            }
            return teacherData;
        }
        #endregion
    }
}