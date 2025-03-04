using System;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using Repositories.Interfaces;
using Repositories.Models;
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
        SELECT 
    cs.c_scheduleid, 
    cs.c_classid, 
    cs.c_starttime, 
    cs.c_endtime, 
    cs.c_weekday, 
    cs.c_subjectid, 
    s.c_subjectname,  -- Joining subject name
    cs.c_teacherid
FROM 
    t_classschedule cs
JOIN 
    t_subjects s ON cs.c_subjectid = s.c_subid  -- Join with subjects table
WHERE 
    cs.c_teacherid = @teacherId
    AND (cs.c_weekday = TO_CHAR(NOW(), 'Day') OR cs.c_starttime > CURRENT_TIME)
ORDER BY 
    cs.c_starttime ASC";


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
                                TeacherId = reader["c_teacherid"] as int?,
                                SubjectName = reader["c_subjectname"].ToString().Trim() // ✅ Added Subject Name
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




        public async Task<int> Register(t_Teacher teacherData)
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



        public async Task<t_material> GetLatestUploadedFile(int teacherId)
        {
            try
            {
                if (_conn.State != System.Data.ConnectionState.Open)
                {
                    await _conn.OpenAsync();
                }

                string query = @"
    SELECT c_material_id, c_filename, c_filetype, c_uploaddate, c_subject_id, c_teacher_id, c_filepath
    FROM t_materials
    WHERE c_teacher_id = @TeacherId
    ORDER BY c_uploaddate DESC
    LIMIT 2;";


                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@TeacherId", teacherId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new t_material
                            {
                                MaterialId = reader.GetInt32(0),
                                FileName = reader.GetString(1),
                                FileType = reader.GetString(2),
                                UploadDate = reader.GetDateTime(3),
                                SubjectId = reader.GetInt32(4),
                                TeacherId = reader.GetInt32(5),
                                FilePath = reader.GetString(6)
                            };

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }


        public async Task<int> EntryCompletedTopics(t_syllabuscomplete completeDetails)
        {
            try
            {
                await _conn.OpenAsync();
                Console.WriteLine($"📌 Received Class ID: {completeDetails.c_class_id}");

                  // ✅ Check if c_classid exists in t_class
        string checkQuery = "SELECT COUNT(*) FROM t_class WHERE c_classid = @c_classid";
        using (var checkCmd = new NpgsqlCommand(checkQuery, _conn))
        {
            checkCmd.Parameters.AddWithValue("@c_classid", completeDetails.c_class_id);
            int count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());

            if (count == 0)
            {
                Console.WriteLine("❌ Error: Class ID does not exist in t_class!");
                return 0; // Stop insertion
            }
        }
                string qry = "insert into t_syllabuscompletedetails (c_classid,c_subjectid,c_teacherid,c_topicnames,c_lecturedate) values (@c_classid,@c_subjectid,@c_teacherid,@c_topicsName,@c_lecturedate)";
                var cmd = new NpgsqlCommand(qry, _conn);
                cmd.Parameters.AddWithValue("@c_classid", completeDetails.c_class_id);
                cmd.Parameters.AddWithValue("@c_subjectid", completeDetails.c_subject_id);
                cmd.Parameters.AddWithValue("@c_teacherid", 8);
                // cmd.Parameters.AddWithValue("@c_topicsName", completeDetails.c_topicsName);
                cmd.Parameters.AddWithValue("@c_topicsName", 
    string.IsNullOrEmpty(completeDetails.c_topicsName) ? (object)DBNull.Value : completeDetails.c_topicsName);

                cmd.Parameters.AddWithValue("@c_lecturedate", Convert.ToDateTime(completeDetails.c_lectureDate));
                await cmd.ExecuteNonQueryAsync();
                return 1;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error while complete topics:" + ex.Message);
                return 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<List<t_syllabuscomplete>> DispSyllabusProgress()
        {
            List<t_syllabuscomplete> tt = new List<t_syllabuscomplete>();
            try
            {
                await _conn.OpenAsync();
                string qry = @"select sc.*,c.c_classname,c.c_classid,s.c_subid,s.c_subjectname,t.c_teachername
                            from t_syllabuscompletedetails sc
                            inner join t_class c
                            on sc.c_classid = c.c_classid
                            inner join t_subjects s
                            on sc.c_subjectid = s.c_subid
                            inner join t_teachers t
                            on sc.c_teacherid = t.c_tid";
                var cmd = new NpgsqlCommand(qry, _conn);
                var dr = await cmd.ExecuteReaderAsync();
                if (dr.HasRows)
                {
                    while (await dr.ReadAsync())
                    {
                        // t_syllabusCompleteDetails scd = new t_syllabusCompleteDetails();
                        tt.Add(new t_syllabuscomplete
                        {
                            c_lecture_id = Convert.ToInt32(dr["c_lectureid"]),
                            c_class_id = Convert.ToInt32(dr["c_classid"]),
                            c_subject_id = Convert.ToInt32(dr["c_subjectid"]),
                            c_teacher_id = Convert.ToInt32(dr["c_teacherid"]),
                            c_topicsName = dr["c_topicnames"].ToString(),
                            c_lectureDate = dr["c_lecturedate"].ToString(),
                            class1 = new t_Class
                            {
                                c_classId = Convert.ToInt32(dr["c_classid"]),
                                c_className = dr["c_classname"].ToString(),
                            },
                            subjects1 = new t_subject
                            {
                                c_subid = Convert.ToInt32(dr["c_subid"]),
                                c_subjectname = dr["c_subjectname"].ToString()
                            },
                            t_TeacherGet1 = new t_Teacher
                            {
                                T_Name = dr["c_teachername"].ToString(),
                            }
                        });

                        // tt.Add(scd);
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error while Displaying Progress of syllabus" + ex.Message);
            }
            finally
            {
                await _conn.CloseAsync();
            }
            return tt;
        }

        public async Task<int> UpdateSyllabusProgress(t_syllabuscomplete completeDetails)
        {
            try
            {
                await _conn.OpenAsync();
                string qry = "update t_syllabuscompletedetails set c_classid=@c_classid,c_subjectid=@c_subjectid,c_teacherid=@c_teacherid,c_topicnames=@c_topicnames,c_lecturedate=@c_lecturedate where c_lectureid=@c_lectureid";
                var cmd = new NpgsqlCommand(qry, _conn);
                cmd.Parameters.AddWithValue("@c_classid", completeDetails.c_class_id);
                cmd.Parameters.AddWithValue("@c_subjectid", completeDetails.c_subject_id);
                cmd.Parameters.AddWithValue("@c_teacherid", completeDetails.c_teacher_id);
                cmd.Parameters.AddWithValue("@c_topicnames", completeDetails.c_topicsName);
                cmd.Parameters.AddWithValue("@c_lecturedate", Convert.ToDateTime(completeDetails.c_lectureDate));
                cmd.Parameters.AddWithValue("@c_lectureid", completeDetails.c_lecture_id);
                await cmd.ExecuteNonQueryAsync();
                return 1;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error while update syllabus progress:" + ex.Message);
                return 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<List<t_topic>> GetTopicNames(int subjectId) // ✅ Accept subjectId as a parameter
{
    List<t_topic> topics = new List<t_topic>();

    try
    {
        await _conn.OpenAsync();

        string qry = "SELECT * FROM t_topics WHERE c_subject_id = @c_subject_id";
        using (var cmd = new NpgsqlCommand(qry, _conn))
        {
            cmd.Parameters.AddWithValue("@c_subject_id", subjectId); // ✅ Correctly passing subjectId

            using (var dr = await cmd.ExecuteReaderAsync())
            {
                while (await dr.ReadAsync())
                {
                    topics.Add(new t_topic
                    {
                        c_tid = dr.GetInt32(dr.GetOrdinal("c_topic_id")),
                        c_topicname = dr.GetString(dr.GetOrdinal("c_topicname")),
                        c_subject_id = dr.GetInt32(dr.GetOrdinal("c_subject_id")),
                    });
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error while fetching topic names: " + ex.Message);
    }
    finally
    {
        await _conn.CloseAsync();
    }

    return topics;
}


        public async Task<List<t_Class>> GetAllClass(int id)
{
    List<t_Class> classList = new List<t_Class>();

    try
    {
        await _conn.OpenAsync();

        // ✅ Fixed SQL Query (Fetching Both Class ID & Name)
        string qry = @"
            SELECT c.c_classid, c.c_classname 
            FROM t_teachers t
            INNER JOIN t_class c ON t.c_class_id = c.c_classid
            WHERE t.c_tid = @teacherId";

        using (var cmd = new NpgsqlCommand(qry, _conn))
        {
            // ✅ Correct Parameter Name
            cmd.Parameters.AddWithValue("@teacherId", id);

            using (var dr = await cmd.ExecuteReaderAsync())
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        classList.Add(new t_Class
                        {
                            c_classId = Convert.ToInt32(dr["c_classid"]), // ✅ Corrected Field Name
                            c_className = dr["c_classname"].ToString()
                        });
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error while fetching class: " + ex.Message);
    }
    finally
    {
        await _conn.CloseAsync();
    }

    return classList;
}


        public async Task<List<t_subject>> GetAllSubjectsByClass(int id)
        {
            List<t_subject> ts = new List<t_subject>();
            try
            {
                await _conn.OpenAsync();
                string qry = "select * from t_subjects where c_classid=@c_classid";
                var cmd = new NpgsqlCommand(qry, _conn);
                cmd.Parameters.AddWithValue("@c_classid", id);
                var dr = await cmd.ExecuteReaderAsync();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        t_subject t = new t_subject();
                        t.c_subid = Convert.ToInt32(dr["c_subid"]);
                        t.c_subid = Convert.ToInt32(dr["c_subid"]);
                        t.c_subjectname = dr["c_subjectname"].ToString();
                        ts.Add(t);
                    }
                }
            }
            catch (System.Exception)
            {
                System.Console.WriteLine("Error while fetching ");
            }
            finally
            {
                await _conn.CloseAsync();
            }
            return ts;
        }


    }
}
