using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implementations
{
    public class TeacherRatingRepository : ITeacherRating
    {
        private readonly NpgsqlConnection _connection;
        public TeacherRatingRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<TeacherInfo>> GetTeachersByClassIdAsync(int classId)
{
    var teachers = new List<TeacherInfo>();
    await _connection.OpenAsync();

    string query = @"
        SELECT t.c_tid, t.c_teachername
FROM t_teachers t
INNER JOIN t_student s ON t.c_class_id = s.c_class_id
WHERE s.c_id = @StudentId;
";

    using (var cmd = new NpgsqlCommand(query, _connection))
    {
        cmd.Parameters.AddWithValue("studentId", classId);
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                teachers.Add(new TeacherInfo
                {
                    c_tid = reader.GetInt32(0),
                    c_teachername = reader.GetString(1)
                });
            }
        }
    }

    return teachers;
}

        public async Task<t_TeacherRating> InsertTeacherRatingAsync(int c_stud_id, int c_teacher_id, int c_rating)
        {
            t_TeacherRating teacherRating = null;

            try
            {
                await _connection.OpenAsync();

                // Step 1: Get the class ID of the student
                string getClassIdSql = "SELECT c_classid FROM t_student WHERE c_id = @StudentId";

                int c_class_id;
                using (var cmd = new NpgsqlCommand(getClassIdSql, _connection))
                {
                    cmd.Parameters.AddWithValue("StudentId", c_stud_id);
                    var result = await cmd.ExecuteScalarAsync();
                    if (result == null)
                    {
                        Console.WriteLine("[ERROR] Student ID does not exist or has no class assigned.");
                        return null;
                    }
                    c_class_id = Convert.ToInt32(result);
                }

                // Step 2: Find the teacher for this class
                string getTeacherIdSql = "SELECT c_tid FROM t_teachers WHERE c_class_id = @ClassId ORDER BY c_tid ASC LIMIT 1";

                int c_teacher_id_db;
                using (var cmd = new NpgsqlCommand(getTeacherIdSql, _connection))
                {
                    cmd.Parameters.AddWithValue("ClassId", c_class_id);
                    var result = await cmd.ExecuteScalarAsync();
                    if (result == null)
                    {
                        Console.WriteLine("[ERROR] No teacher found for this class.");
                        return null;
                    }
                    c_teacher_id_db = Convert.ToInt32(result);
                }

                // Step 3: Insert into t_teacher_rating
                string insertSql = @"
            INSERT INTO t_teacher_rating (c_stud_id, c_teacher_id, c_rating)
            VALUES (@StudentId, @TeacherId, @Rating)
            RETURNING c_stud_id, c_teacher_id, c_rating;";

                using (var cmd = new NpgsqlCommand(insertSql, _connection))
                {
                    cmd.Parameters.AddWithValue("StudentId", c_stud_id);
                    cmd.Parameters.AddWithValue("TeacherId", c_teacher_id_db);
                    cmd.Parameters.AddWithValue("Rating", c_rating);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            teacherRating = new t_TeacherRating
                            {
                                c_stud_id = reader.GetInt32(reader.GetOrdinal("c_stud_id")),
                                c_teacher_id = reader.GetInt32(reader.GetOrdinal("c_teacher_id")),
                                c_rating = reader.GetInt32(reader.GetOrdinal("c_rating"))
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] InsertTeacherRatingAsync failed: {ex}");
            }
            finally
            {
                await _connection.CloseAsync();
            }

            return teacherRating;
        }

    }
}


