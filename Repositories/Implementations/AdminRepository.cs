using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Interfaces;
using StudentManagementSystem.Models;
using Npgsql;
using Microsoft.Win32.SafeHandles;

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
            return 1;
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

        public async Task<List<t_subjects>> GetAllSubjects()
        {
            List<t_subjects> ts = new List<t_subjects>();
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
                        t_subjects t = new t_subjects();
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
    }
}