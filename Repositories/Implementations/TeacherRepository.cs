using System;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using StudentManagementSystem.Interfaces;
using StudentManagementSystem.Models;

namespace Repositories.Implementations
{
    public class TeacherRepository : ITeacherInterface
    {
        private readonly NpgsqlConnection _conn;

        public TeacherRepository(NpgsqlConnection connection)
        {
            _conn = connection;
        }

        public async Task<int> Add_Material(t_material materialData)
        {
            try
            {
                if (_conn.State != System.Data.ConnectionState.Open)
                {
                    await _conn.OpenAsync();
                }

                string query = @"INSERT INTO t_materials 
    (c_teacher_id, c_filename, c_filetype,  c_Uploaddate, c_Subject_id,c_filepath) 
    VALUES 
    (@TeacherId, @FileName, @FileType, @UploadDate, @SubjectId, @FilePath) 
    RETURNING c_material_id;";  // Use correct column for returning ID

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@TeacherId", materialData.TeacherId); // ✅ Added TeacherId
                    cmd.Parameters.AddWithValue("@FileName", materialData.FileName);
                    cmd.Parameters.AddWithValue("@FileType", materialData.FileType);
                    cmd.Parameters.AddWithValue("@FilePath", materialData.FilePath);
                    cmd.Parameters.AddWithValue("@UploadDate", materialData.UploadDate);
                    cmd.Parameters.AddWithValue("@SubjectId", (object?)materialData.SubjectId ?? DBNull.Value);

                    int materialId = (int)await cmd.ExecuteScalarAsync(); // Get inserted Material ID
                    return materialId;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in Add_Material: " + ex.Message);
                return 0; // Indicate failure
            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }
        }
        public async Task<List<t_student_view>> GetStudentsByTeacherId(int teacherId)
        {
            try
            {
                if (_conn.State != System.Data.ConnectionState.Open)
                {
                    await _conn.OpenAsync();
                }

                // Step 1: Get the teacher's class ID
                string getClassIdQuery = "SELECT c_class_id FROM t_teachers WHERE c_tid = @teacherId";
                int classId;

                using (NpgsqlCommand classCmd = new NpgsqlCommand(getClassIdQuery, _conn))
                {
                    classCmd.Parameters.AddWithValue("@teacherId", teacherId);
                    var result = await classCmd.ExecuteScalarAsync();
                    if (result == null)
                    {
                        return new List<t_student_view>(); // No class found for the teacher
                    }
                    classId = Convert.ToInt32(result);
                }

                // Step 2: Get students from the same class
                string query = @"
            SELECT c_name, c_classid 
            FROM t_student 
            WHERE c_classid = @classId";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@classId", classId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var students = new List<t_student_view>();

                        while (await reader.ReadAsync())
                        {
                            var student = new t_student_view
                            {
                                c_studentName = reader["c_name"].ToString(),
                                c_classId = Convert.ToInt32(reader["c_classid"])
                            };
                            students.Add(student);
                        }

                        return students;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetStudentsByTeacherId: " + ex.Message);
                return new List<t_student_view>(); // Return empty list instead of null
            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }
        }

        public async Task<List<t_classschedule>> GetUpcomingClassesForTeacher(int teacherId)
        {
            try
            {
                if (_conn.State != System.Data.ConnectionState.Open)
                {
                    await _conn.OpenAsync();
                }

                // ✅ Query to get upcoming classes for a teacher
                string query = @"
        SELECT c_scheduleid, c_classid, c_starttime, c_endtime, c_weekday, c_subjectid, c_teacherid
        FROM t_classschedule
        WHERE c_teacherid = @teacherId
        AND (c_weekday = TO_CHAR(NOW(), 'Day') OR c_starttime > CURRENT_TIME)
        ORDER BY c_starttime ASC";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@teacherId", teacherId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var schedules = new List<t_classschedule>();

                        while (await reader.ReadAsync())
                        {
                            var schedule = new t_classschedule
                            {
                                ScheduleId = Convert.ToInt32(reader["c_scheduleid"]),
                                ClassId = reader["c_classid"] as int?,
                                StartTime = reader["c_starttime"] as TimeSpan?,
                                EndTime = reader["c_endtime"] as TimeSpan?,
                                Weekday = reader["c_weekday"].ToString().Trim(),
                                SubjectId = reader["c_subjectid"] as int?,
                                TeacherId = reader["c_teacherid"] as int?
                            };
                            schedules.Add(schedule);
                        }

                        return schedules;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetUpcomingClassesForTeacher: " + ex.Message);
                return new List<t_classschedule>(); // Return empty list if error
            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }
        }




        public async Task<int> Register(t_teacher teacherData)
        {
            try
            {
                if (_conn.State != System.Data.ConnectionState.Open)
                {
                    await _conn.OpenAsync();
                }

                string query = @"INSERT INTO t_teachers 
                    (c_TeacherName, c_temail, c_tpassword, c_tmobno, c_tdob, c_tQualification, 
                     c_experience, c_expert_subject, c_class_id, c_subjectId) 
                    VALUES 
                    (@c_TeacherName, @c_temail, @c_tpassword, @c_tmobno, @c_tdob, @c_tQualification, 
                     @c_experience, @c_expert_subject, @c_class_id, @c_subjectId)";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_TeacherName", teacherData.T_Name);
                    cmd.Parameters.AddWithValue("@c_temail", teacherData.T_Email);
                    cmd.Parameters.AddWithValue("@c_tpassword", teacherData.T_PasswordHash);
                    // long mobileNumber = long.TryParse(teacherData.T_MobileNumber, out long parsedMobile) ? parsedMobile : 0;
                    cmd.Parameters.AddWithValue("@c_tmobno", NpgsqlDbType.Bigint, teacherData.T_MobileNumber);
                    // cmd.Parameters.AddWithValue("@c_tmobno", mobileNumber);
                    cmd.Parameters.AddWithValue("@c_tdob", teacherData.T_DateOfBirth);
                    cmd.Parameters.AddWithValue("@c_tQualification", teacherData.T_Qualification);
                    cmd.Parameters.AddWithValue("@c_experience", teacherData.T_Experience);
                    cmd.Parameters.AddWithValue("@c_expert_subject", teacherData.T_ExpertSubject);
                    cmd.Parameters.AddWithValue("@c_class_id", (object?)teacherData.T_Class_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@c_subjectId", (object?)teacherData.T_SubjectId ?? DBNull.Value);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0 ? 1 : 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Register: " + ex.Message);
                return 0;
            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }
        }
    }
}
