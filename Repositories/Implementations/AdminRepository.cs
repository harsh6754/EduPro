using System;
using System.IO;
using System.Threading.Tasks;
using Repositories.Interfaces;
using StudentManagementSystem.Models;
using Npgsql;

namespace Repositories.Implementations
{
    public class AdminRepository : IAdminInterface
    {
        private readonly NpgsqlConnection _connection;

        public AdminRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> AddStudent(t_Student student)
        {
            int newStudentId = 0;
            string? imagePath = null;

            // ✅ Generate password dynamically
            if (!string.IsNullOrEmpty(student.c_studentName) && !string.IsNullOrEmpty(student.c_studentPhone) && student.c_studentPhone.Length >= 5)
            {
                string namePart = student.c_studentName.Length >= 4 ? student.c_studentName.Substring(0, 4) : student.c_studentName;
                string phonePart = student.c_studentPhone.Substring(student.c_studentPhone.Length - 5);
                student.c_password = namePart + phonePart;
            }
            else
            {
                student.c_password = "InvalidData"; // Fallback in case of invalid input
            }

            // ✅ Handle profile picture upload (if provided)
            if (student.StudentPic != null)
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string uniqueFileName = $"{Guid.NewGuid()}_{student.StudentPic.FileName}";
                imagePath = Path.Combine(uploadFolder, uniqueFileName);
                string fullPath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await student.StudentPic.CopyToAsync(fileStream);
                }
            }

            await _connection.OpenAsync();
            try
            {
                using (var cmd = new NpgsqlCommand(@"
                INSERT INTO t_student(
                    c_name, c_email, c_dob, c_mobile_no, c_gender, c_password,
                    c_classid, c_sectionid, c_guardian_name, c_enroll_date, c_profile_pic, c_status
                ) 
                VALUES(
                    @c_name, @c_email, @c_dob, @c_mobile_no, @c_gender, @c_password,
                    @c_classid, @c_sectionid, @c_guardian_name, @c_enroll_date, @c_profile_pic, @c_status
                ) RETURNING c_studentid", _connection))
                {
                    cmd.Parameters.AddWithValue("@c_name", student.c_studentName);
                    cmd.Parameters.AddWithValue("@c_email", student.c_studentEmail);
                    cmd.Parameters.AddWithValue("@c_dob", student.c_studentDOB);
                    cmd.Parameters.AddWithValue("@c_mobile_no", student.c_studentPhone);
                    cmd.Parameters.AddWithValue("@c_gender", student.c_studentGender);
                    cmd.Parameters.AddWithValue("@c_password", student.c_password);
                    cmd.Parameters.AddWithValue("@c_classid", student.c_class.c_classId);
                    cmd.Parameters.AddWithValue("@c_sectionid", student.c_section.c_sectionId);
                    cmd.Parameters.AddWithValue("@c_guardian_name", student.c_studentGuardianDetails);
                    cmd.Parameters.AddWithValue("@c_enroll_date", student.c_studentEnrollDate);
                    cmd.Parameters.AddWithValue("@c_profile_pic", (object?)imagePath ?? DBNull.Value); // Handle null case properly
                    cmd.Parameters.AddWithValue("@c_status", "Active");

                    newStudentId = (int)await cmd.ExecuteScalarAsync();
                }
            }
            finally
            {
                await _connection.CloseAsync();
            }

            return newStudentId;
        }

        public async Task<List<t_Student>> GetAllStudents()
        {
            var students = new List<t_Student>();

            await _connection.OpenAsync();
            try
            {
                using (var cmd = new NpgsqlCommand(@"
            SELECT 
                s.c_id, s.c_name, s.c_email, s.c_dob, s.c_mobile_no, s.c_gender, s.c_password,
                s.c_guardian_name, s.c_enroll_date, s.c_profile_pic, s.c_status,
                c.c_classid, c.c_classname,
                sec.c_sectionid, sec.c_sectionname
            FROM 
                t_student s
            JOIN 
                t_class c ON s.c_classid = c.c_classid
            JOIN 
                t_section sec ON s.c_sectionid = sec.c_sectionid;", _connection))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            students.Add(new t_Student
                            {
                                c_studentId = reader.GetInt32(0),
                                c_studentName = reader.GetString(1),
                                c_studentEmail = reader.GetString(2),
                                c_studentDOB = reader.GetDateTime(3),
                                c_studentPhone = reader.GetInt64(4).ToString(),
                                c_studentGender = reader.GetString(5),
                                c_password = reader.GetString(6),
                                c_studentGuardianDetails = reader.GetString(7),
                                c_studentEnrollDate = reader.GetDateTime(8),
                                c_studentProfile = reader.IsDBNull(9) ? null : reader.GetString(9),
                                c_studentStatus = reader.GetString(10),
                                c_class = new t_Class { c_classId = reader.GetInt32(11), c_className = reader.GetString(12) },
                                c_section = new t_Section { c_sectionId = reader.GetInt32(13), c_sectionName = reader.GetString(14) }
                            });
                        }
                    }
                }
            }
            finally
            {
                await _connection.CloseAsync();
            }

            return students;
        }

        public async Task<List<t_Class>> GetAllClasses()
        {
            var classes = new List<t_Class>();

            await _connection.OpenAsync();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT c_classid, c_classname FROM t_class;", _connection))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            classes.Add(new t_Class
                            {
                                c_classId = reader.GetInt32(0),
                                c_className = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            finally
            {
                await _connection.CloseAsync();
            }

            return classes;
        }

        public async Task<List<t_Section>> GetAllSections()
        {
            var sections = new List<t_Section>();

            await _connection.OpenAsync();
            try
            {
                using (var cmd = new NpgsqlCommand(@"
            SELECT s.c_sectionid, s.c_sectionname, c.c_classid, c.c_classname 
            FROM t_section s
            JOIN t_class c ON s.c_classid = c.c_classid;", _connection))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            sections.Add(new t_Section
                            {
                                c_sectionId = reader.GetInt32(0),
                                c_sectionName = reader.GetString(1),
                                c_classId = reader.GetInt32(2),  // Assign class ID
                                c_class = new t_Class
                                {
                                    c_classId = reader.GetInt32(2),
                                    c_className = reader.GetString(3)
                                }
                            });
                        }
                    }
                }
            }
            finally
            {
                await _connection.CloseAsync();
            }

            return sections;
        }
    }
}
