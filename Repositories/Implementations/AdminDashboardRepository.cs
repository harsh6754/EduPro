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
    }
}