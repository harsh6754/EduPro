using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Models;
using StudentManagementSystem.Models;
using static Repositories.Models.t_Teacher;

namespace Repositories.Implementations
{
    public class AdminDashboardRepository : IAdminInterface
    {
        private readonly NpgsqlConnection _connection;
        public AdminDashboardRepository(NpgsqlConnection conn){
            _connection = conn;
        }

        public async Task<List<t_Exam>> GetAllETimetableData()
        {
            List<t_Exam> eList = new List<t_Exam>();
            try
            {
                await _connection.OpenAsync();
                string qry = @"
                SELECT DISTINCT ON (t.c_classid) 
                    t.c_eid,
                    t.c_classid, 
                    t.c_image,
                    c.c_classname
                FROM t_timetable t 
                LEFT JOIN t_class c 
                ON
                t.c_classid = c.c_classid
                ORDER BY t.c_classid, t.c_eid;";
                using (NpgsqlCommand cmd = new NpgsqlCommand(qry, _connection))
                {
                    using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            t_Exam examVM = new t_Exam()
                            {
                                CEid = Convert.ToInt32(reader["c_eid"]),
                                CClassId = Convert.ToInt32(reader["c_classid"]),
                                CImage = reader["c_image"].ToString(),
                                Class = new t_Class()
                                {
                                    c_classId = Convert.ToInt32(reader["c_classid"]),
                                    c_className = reader["c_classname"].ToString()
                                }
                            };
                            eList.Add(examVM);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return eList;
        }
        public async Task<t_Exam> GetETimetableData(int classid)
        {
            Console.WriteLine("getClassid: " + classid);
            t_Exam exam = new t_Exam();
            try
            {
                await _connection.OpenAsync();
                string query = @"SELECT 
                t.c_eid,
                t.c_classid, 
                t.c_image,
                c.c_classname
                FROM t_timetable t 
                LEFT JOIN t_class c
                ON 
                t.c_classid=c.c_classid
                WHERE t.c_classid=@c_classid;";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@c_classid", classid);
                    using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            exam.CEid = Convert.ToInt32(reader["c_eid"]);
                            exam.CClassId = Convert.ToInt32(reader["c_classid"]);
                            exam.CImage = reader["c_image"].ToString();
                            exam.Class = new t_Class()
                            {
                                c_classId = Convert.ToInt32(reader["c_classid"]),
                                c_className = reader["c_classname"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return exam;
        }
        public async Task<int> Update(t_Exam exam)
        {
            Console.WriteLine(exam.CImage);
            Console.WriteLine(exam.CEid);//coming 0
            try
            {
                await _connection.OpenAsync();
                string query = @"UPDATE t_timetable SET c_image=@c_image WHERE c_classid=@c_classid;";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@c_image", exam.CImage);
                    cmd.Parameters.AddWithValue("@c_classid", Convert.ToInt32(exam.CClassId));
                    await cmd.ExecuteNonQueryAsync();
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        public async Task<int> Add(t_Exam exam)
        {
            Console.WriteLine(exam.CImage);
            try
            {
                await _connection.OpenAsync();
                string query = @"INSERT INTO t_timetable(c_classid, c_image)VALUES(@c_classid, @c_image);";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@c_image", exam.CImage);
                    cmd.Parameters.AddWithValue("@c_classid", Convert.ToInt32(exam.CClassId));
                    await cmd.ExecuteNonQueryAsync();
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        public async Task<List<t_Class>> GetAllClass()
        {
            List<t_Class> c = new List<t_Class>();
            try
            {
                await _connection.OpenAsync();
                string qry = "SELECT * FROM t_class";
                var cmd = new NpgsqlCommand(qry, _connection);
                var dr = await cmd.ExecuteReaderAsync();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        t_Class t = new t_Class();
                        t.c_classId = Convert.ToInt32(dr["c_classid"]);
                        t.c_className = dr["c_classname"].ToString();
                        c.Add(t);
                    }
                }

            }
            catch (System.Exception)
            {
                System.Console.WriteLine("Error while fetching class");
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return c;
        }
        public async Task<int> Delete(int classid)
        {
            Console.WriteLine("Delete Classid: " + classid);
            // List<t_Class> c = new List<t_Class>();
            try
            {
                await _connection.OpenAsync();
                string query = "DELETE FROM t_timetable WHERE c_classid=@c_classid";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@c_classid", classid);
                    await cmd.ExecuteNonQueryAsync();
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        public async Task<int> AddStudent(t_Student student)
        {
            return 1;
        }

        public async Task<int> AssignSubClass(t_teacher_Assign teacher)
        {
            try
            {
                await _connection.OpenAsync();
                string qry = "update t_teachers set c_subjectid=@c_subjectid,c_class_id=@c_class_id where c_tid=@c_tid";
                var cmd = new NpgsqlCommand(qry,_connection);
                cmd.Parameters.AddWithValue("@c_subjectid",teacher.c_subject_id);
                cmd.Parameters.AddWithValue("@c_class_id",teacher.c_class_id);
                cmd.Parameters.AddWithValue("@c_tid",teacher.c_tid);
                await cmd.ExecuteNonQueryAsync();
                return 1;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error while Assig class and subject :" + e.Message);
                return 0;
            }
            finally{
                await _connection.CloseAsync();
            }
        }

        public async Task<List<t_subject>> GetAllSubjects()
        {
            List<t_subject> ts = new List<t_subject>();
            try
            {
                await _connection.OpenAsync();
                string qry = "select * from t_subjects";
                var cmd = new NpgsqlCommand(qry, _connection);
                var dr = await cmd.ExecuteReaderAsync();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        t_subject t = new t_subject();
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
                await _connection.CloseAsync();
            }
            return ts;
        }

        public async Task<List<t_teacherGet>> GetAllTeachers()
        {
            List<t_teacherGet> ta = new List<t_teacherGet>();
            try
            {
                await _connection.OpenAsync();
                string qry = "select * from t_teachers";
                var cmd = new NpgsqlCommand(qry,_connection);
                var dr = await cmd.ExecuteReaderAsync();
                if(dr.HasRows){
                    while (await dr.ReadAsync())
                    {
                        t_teacherGet tg = new t_teacherGet();
                        tg.c_tid = Convert.ToInt32(dr["c_tid"]);
                        tg.c_tName = dr["c_teachername"].ToString();
                        ta.Add(tg);
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error while fetching all Teachers:" +ex.Message);
            }
            finally{
                await _connection.CloseAsync();
            }
            return ta;
        }
 

public async Task<List<t_Timetable>> DispTimeTable(int classid)
{
    List<t_Timetable> tc = new List<t_Timetable>();
    try
    {
        await _connection.OpenAsync();
        string qry = @"
            SELECT 
                tt.c_scheduleid, 
                tt.c_classid,  
                tt.c_subjectid, 
                tt.c_teacherid, 
                tt.c_weekday, 
                tt.c_starttime, 
                tt.c_endtime, 
                s.c_subjectname, 
                t.c_teachername AS TeacherName
            FROM t_classschedule tt
            INNER JOIN t_subjects s ON tt.c_subjectid = s.c_subid
            INNER JOIN t_teachers t ON tt.c_teacherid = t.c_tid
            WHERE tt.c_classid = @Id 
            ORDER BY tt.c_starttime";
        var cmd = new NpgsqlCommand(qry, _connection);
        cmd.Parameters.AddWithValue("@Id", classid);
        var dr = await cmd.ExecuteReaderAsync();

        if (dr.HasRows)
        {
            while (await dr.ReadAsync())
            {
                t_Timetable tt = new t_Timetable
                {
                    c_scheduleid = dr.GetInt32(0),
                    c_classid = dr.GetInt32(1),
                    c_subjectid = dr.GetInt32(2),
                    c_teacherid = dr.GetInt32(3),
                    c_weekday = dr.GetString(4),
                    c_starttime = dr.GetTimeSpan(5),
                    c_endtime = dr.GetTimeSpan(6),
                    c_SubjectName = dr.GetString(7),
                    c_TeacherName = dr.GetString(8)
                };
                
                tc.Add(tt);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error while fetching timetable: " + ex.Message);
    }
    finally
    {
        await _connection.CloseAsync();
    }
    return tc;
}

public async Task<int> CreateTimeTable(t_Timetable timetable)
{
    Console.WriteLine("CreateTimeTable AdminController");
    try
    {
        await _connection.OpenAsync();
        string qry = "INSERT INTO t_classschedule (c_classid, c_starttime, c_endtime, c_weekday, c_subjectid, c_teacherid) VALUES (@classid, @starttime, @endtime, @weekday, @subjectid, @teacherid) RETURNING c_scheduleid";
        using var cmd = new NpgsqlCommand(qry, _connection);
        cmd.Parameters.AddWithValue("@classid", timetable.c_classid);
        cmd.Parameters.AddWithValue("@starttime", timetable.c_starttime);
        cmd.Parameters.AddWithValue("@endtime", timetable.c_endtime);
        cmd.Parameters.AddWithValue("@weekday", timetable.c_weekday);
        cmd.Parameters.AddWithValue("@subjectid", timetable.c_subjectid);
        cmd.Parameters.AddWithValue("@teacherid", timetable.c_teacherid);
        return await cmd.ExecuteScalarAsync() != null ? 1 : 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error inserting timetable: {ex.Message}");
        return 0;
    }
    finally
    {
        await _connection.CloseAsync();
    }
}

public async Task<int> UpdateTimeTable(t_Timetable timetable)
{
    try
    {
        await _connection.OpenAsync();
        string qry = "UPDATE t_classschedule SET c_starttime = @starttime, c_endtime = @endtime, c_weekday = @weekday, c_subjectid = @subjectid, c_teacherid = @teacherid WHERE c_scheduleid = @scheduleid";
        using var cmd = new NpgsqlCommand(qry, _connection);
        cmd.Parameters.AddWithValue("@scheduleid", timetable.c_scheduleid);
        cmd.Parameters.AddWithValue("@starttime", timetable.c_starttime);
        cmd.Parameters.AddWithValue("@endtime", timetable.c_endtime);
        cmd.Parameters.AddWithValue("@weekday", timetable.c_weekday);
        cmd.Parameters.AddWithValue("@subjectid", timetable.c_subjectid);
        cmd.Parameters.AddWithValue("@teacherid", timetable.c_teacherid);
        return await cmd.ExecuteNonQueryAsync() > 0 ? 1 : 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating timetable: {ex.Message}");
        return 0;
    }
    finally
    {
        await _connection.CloseAsync();
    }
}

public async Task<int> DeleteTimeTable(int scheduleId)
{
    try
    {
        await _connection.OpenAsync();
        string qry = "DELETE FROM t_classschedule WHERE c_scheduleid = @scheduleid";
        using var cmd = new NpgsqlCommand(qry, _connection);
        cmd.Parameters.AddWithValue("@scheduleid", scheduleId);
        return await cmd.ExecuteNonQueryAsync() > 0 ? 1 : 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error deleting timetable: {ex.Message}");
        return 0;
    }
    finally
    {
        await _connection.CloseAsync();
    }
}

public async Task<bool> SendNotification(NotificationModel notification)
    {
            await _connection.OpenAsync();

            using (var cmd = new NpgsqlCommand(@"INSERT INTO Notifications 
                 (receiverId, title, message) 
                 VALUES (@receiverId, @title, @message)", _connection))
            {
                cmd.Parameters.AddWithValue("@receiverId", notification.ReceiverId);
                cmd.Parameters.AddWithValue("@title", notification.Title);
                cmd.Parameters.AddWithValue("@message", notification.Message);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        public async Task<int> GetUnreadNotificationCount(int userId)
    {
            await _connection.OpenAsync();

            using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM Notifications WHERE receiverId = @userId AND status = false", _connection))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
        }

    // (C) Get All Notifications for User
    public async Task<List<NotificationModel>> GetAllNotifications(int userId)
    {
        List<NotificationModel> notifications = new List<NotificationModel>();

            await _connection.OpenAsync();

            using (var cmd = new NpgsqlCommand("SELECT id, receiverId, title, message, status, createdAt FROM Notifications WHERE receiverId = @userId AND status=@Status ORDER BY createdAt DESC", _connection))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@Status", false);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        notifications.Add(new NotificationModel
                        {
                            Id = (int)reader["id"],
                            ReceiverId = (int)reader["receiverid"],
                            Title = reader["title"].ToString(),
                            Message = reader["message"].ToString(),
                            Status = (bool)reader["status"],
                            CreatedAt = (DateTime)reader["createdat"]
                        });
                    }
                }
            }
            return notifications;
        }
    // (D) Mark Notification as Read
    public async Task<bool> MarkAsRead(int notificationId)
    {
            await _connection.OpenAsync();

            using (var cmd = new NpgsqlCommand("UPDATE Notifications SET status = true WHERE id = @notificationId", _connection))
            {
                cmd.Parameters.AddWithValue("@notificationId", notificationId);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
    }

    public async Task<bool> IsSlotAvailableAsync(string startTime, string endTime, int classId, int teacherId, string weekday)
    {
        try
        {
            await _connection.OpenAsync();

            // Query to check for overlapping lectures
            var query = @"
                SELECT COUNT(*) 
                    FROM t_classschedule 
                    WHERE (c_classid = @ClassId AND c_teacherid = @TeacherId AND c_weekday = @Weekday)
                    AND (
                        (@StartTime BETWEEN c_starttime AND c_endtime) 
                        OR (@EndTime BETWEEN c_starttime AND c_endtime) 
                        OR (c_starttime BETWEEN @StartTime AND @EndTime) 
                        OR (c_endtime BETWEEN @StartTime AND @EndTime) 
                    )";


            using (var cmd = new NpgsqlCommand(query, _connection))
            {
                
                cmd.Parameters.AddWithValue("@ClassId", classId);
                cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                cmd.Parameters.AddWithValue("@StartTime", TimeSpan.Parse(startTime));
                cmd.Parameters.AddWithValue("@EndTime", TimeSpan.Parse(endTime));
                cmd.Parameters.AddWithValue("@Weekday", weekday);
                var count = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(count) == 0; 
            }
        }   
        catch(Exception e)
        {
            Console.WriteLine($"Error in AdminRepository {e.Message}");
            return false;
        } 
    }

    public async Task<int> CreateSyllabus(t_syllabus syllabus)
        {
            try
            {
                if (syllabus == null)
                {
                    Console.WriteLine("Syllabus object is NULL in database method.");
                    return 0;
                }
                await _connection.OpenAsync();
                string qry = "insert into t_syllabus (c_subject_id,c_topicname,c_classid) values(@c_subject_id,@c_topicname,@c_classid)";
                var cmd = new NpgsqlCommand(qry, _connection);
                cmd.Parameters.AddWithValue("@c_subject_id", Convert.ToInt32(syllabus.c_subject_id));
                cmd.Parameters.AddWithValue("@c_topicname", syllabus.c_topicName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@c_classid", Convert.ToInt32(syllabus.c_class_id));
                await cmd.ExecuteNonQueryAsync();
                return 1;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error while creating syllabus:" + ex.Message);
                return 0;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<int> UpdateSyllabus(t_syllabus syllabus)
        {
            try
            {
                await _connection.OpenAsync();
                string qry = "update t_syllabus set c_subject_id=@c_subject_id,c_topicname=@c_topicname,c_classid=@c_classid where c_syllabus_id=@c_syllabus_id";
                var cmd = new NpgsqlCommand(qry, _connection);
                cmd.Parameters.AddWithValue("@c_subject_id", syllabus.c_subject_id);
                cmd.Parameters.AddWithValue("@c_topicname", syllabus.c_topicName);
                cmd.Parameters.AddWithValue("@c_classid", syllabus.c_class_id);
                cmd.Parameters.AddWithValue("@c_syllabus_id", syllabus.c_syllabus_id);
                await cmd.ExecuteNonQueryAsync();
                return 1;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error while updating syllabus:" + ex.Message);
                return 0;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<List<t_syllabus>> DispSyllabus()
        {
            List<t_syllabus> tt = new List<t_syllabus>();
            try
            {
                await _connection.OpenAsync();
                string qry = @"select s.*,sb.c_subjectname,c.c_classname
                            from t_syllabus s
                            inner join t_subjects sb
                            on s.c_subject_id = sb.c_subid
                            inner join t_class c
                            on c.c_classid = s.c_classid";
                var cmd = new NpgsqlCommand(qry, _connection);
                var dr = await cmd.ExecuteReaderAsync();
                if (dr.HasRows)
                {
                    while (await dr.ReadAsync())
                    {
                        // t_syllabus ts = new t_syllabus();
                        tt.Add(new t_syllabus
                        {
                            c_syllabus_id = Convert.ToInt32(dr["c_syllabus_id"]),
                            c_subject_id = Convert.ToInt32(dr["c_subject_id"]),
                            c_topicName = dr["c_topicname"].ToString(),
                            c_class_id = Convert.ToInt32(dr["c_classid"]),
                            subjects1 = new t_subject()
                            {
                                c_subjectname = dr["c_subjectname"].ToString()
                            },
                            class1 = new t_Class()
                            {
                                c_className = dr["c_classname"].ToString()
                            }
                        });

                        // tt.Add(ts);
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error while Displying syllabus:" + ex.Message);
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return tt;
        }

        public async Task<int> DeleteSyllabus(int id)
        {
            try
            {
                await _connection.OpenAsync();
                string qry = "delete from t_syllabus where c_syllabus_id=@c_syllabus_id";
                var cmd = new NpgsqlCommand(qry, _connection);
                cmd.Parameters.AddWithValue("@c_syllabus_id", id);
                await cmd.ExecuteNonQueryAsync();
                return 1;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error while deleting syllabus:" + ex.Message);
                return 0;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<int> EntryCompletedTopics(t_syllabuscomplete completeDetails)
        {
            try
            {
                await _connection.OpenAsync();
                string qry = "insert into t_syllabuscompletedetails (c_classid,c_subjectid,c_teacherid,c_topicnames,c_lecturedate) values (@c_classid,@c_subjectid,@c_teacherid,@c_topicnames,@c_lecturedate)";
                var cmd = new NpgsqlCommand(qry, _connection);
                cmd.Parameters.AddWithValue("@c_classid", completeDetails.c_class_id);
                cmd.Parameters.AddWithValue("@c_subjectid", completeDetails.c_subject_id);
                cmd.Parameters.AddWithValue("@c_teacherid", completeDetails.c_teacher_id);
                cmd.Parameters.AddWithValue("@c_topicnames", completeDetails.c_topicsName);
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
                await _connection.CloseAsync();
            }
        }

        public async Task<List<t_syllabuscomplete>> DispSyllabusProgress()
        {
            List<t_syllabuscomplete> tt = new List<t_syllabuscomplete>();
            try
            {
                await _connection.OpenAsync();
                string qry = @"select sc.*,c.c_classname,s.c_subjectname,t.c_teachername
                            from t_syllabuscompletedetails sc
                            inner join t_class c
                            on sc.c_classid = c.c_classid
                            inner join t_subjects s
                            on sc.c_subjectid = s.c_subid
                            inner join t_teachers t
                            on sc.c_teacherid = t.c_tid";
                var cmd = new NpgsqlCommand(qry, _connection);
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
                                c_className = dr["c_classname"].ToString(),
                            },
                            subjects1 = new t_subject
                            {
                                c_subjectname = dr["c_subjectname"].ToString()
                            },
                            t_TeacherGet1 = new t_teacherGet
                            {
                                c_tName = dr["c_teachername"].ToString(),
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
                await _connection.CloseAsync();
            }
            return tt;
        }

        public async Task<int> UpdateSyllabusProgress(t_syllabuscomplete completeDetails)
        {
            try
            {
                await _connection.OpenAsync();
                string qry = "update t_syllabuscompletedetails set c_classid=@c_classid,c_subjectid=@c_subjectid,c_teacherid=@c_teacherid,c_topicnames=@c_topicnames,c_lecturedate=@c_lecturedate where c_lectureid=@c_lectureid";
                var cmd = new NpgsqlCommand(qry, _connection);
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
                await _connection.CloseAsync();
            }
        }

        public async Task<List<t_topic>> GetTopicNames()
        {
            List<t_topic> tt = new List<t_topic>();
            try
            {
                await _connection.OpenAsync();
                string qry = "select * from t_topics";
                var cmd = new NpgsqlCommand(qry, _connection);
                var dr = await cmd.ExecuteReaderAsync();
                if (dr.HasRows)
                {
                    while (await dr.ReadAsync())
                    {
                        tt.Add(new t_topic
                        {
                            c_tid = Convert.ToInt32(dr["c_topic_id"]),
                            c_topicname = dr["c_topicname"].ToString(),
                            c_subject_id = Convert.ToInt32(dr["c_subject_id"]),
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error while fetching topic names:" + ex.Message);
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return tt;
        }

        public async Task<List<t_Student>> GetAllStudents()
        {
            List<t_Student> tt = new List<t_Student>();
            try
            {
                await _connection.OpenAsync();
                string qry = "select * from t_student";
                var cmd = new NpgsqlCommand(qry,_connection);
                var dr = await cmd.ExecuteReaderAsync();
                if(dr.HasRows){
                    while (await dr.ReadAsync())
                    {
                        tt.Add(new t_Student{
                            c_studentId = Convert.ToInt32(dr["c_id"]),
                            c_studentName = dr["c_name"].ToString(),
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error while getting students:" + ex.Message);
            }
            finally{
                await _connection.CloseAsync();
            }
            return tt;
        }

        public List<TeacherTreeViewModel> GetTeachersWithStudents()
    {
        List<TeacherTreeViewModel> teachers = new List<TeacherTreeViewModel>();


        _connection.Open();
        string query = @"
                    SELECT 
                        t.c_tid AS teacher_id, 
                        t.c_teachername AS teacher_name, 
                        c.c_className AS class_name,
                        s.c_id AS student_id, 
                        s.c_name AS student_name
                    FROM t_teachers t
                    JOIN t_class c ON t.c_class_id = c.c_classid
                    LEFT JOIN t_student s ON s.c_classid = c.c_classid
                    ORDER BY t.c_tid, s.c_id;";

        using (var cmd = new NpgsqlCommand(query, _connection))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int teacherId = reader.GetInt32(0);
                    string teacherName = reader.GetString(1);
                    string className = reader.GetString(2);
                    int? studentId = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3);
                    string studentName = reader.IsDBNull(4) ? null : reader.GetString(4);

                    var teacher = teachers.FirstOrDefault(t => t.Id == teacherId);
                    if (teacher == null)
                    {
                        teacher = new TeacherTreeViewModel
                        {
                            Id = teacherId,
                            Text = $"{teacherName} ({className})",
                            Items = new List<TeacherTreeViewModel>()
                        };
                        teachers.Add(teacher);
                    }

                    if (studentId.HasValue)
                    {
                        teacher.Items.Add(new TeacherTreeViewModel { Id = studentId.Value, Text = studentName });
                    }
                }
            }

        }
        return teachers;
    }
    public async Task<List<t_Student>> GetStudentCountPClass()
    {
        List<t_Student> studentCounts = new List<t_Student>();
        try
        {
            await _connection.OpenAsync();
            string query = "SELECT c_classid, COUNT(c_id) FROM t_student GROUP BY c_classid ORDER BY c_classid;";
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, _connection))
            {
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        t_Student t_Student = new t_Student()
                        {
                            c_class = new t_Class
                            {
                                c_classId = Convert.ToInt32(reader["c_classid"]),
                                StudentCount= Convert.ToInt32(reader["count"])
                            }
                        };
                        studentCounts.Add(t_Student);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }
        return studentCounts;
    }
    }

    }
