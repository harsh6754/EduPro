using System.Data;
using Npgsql;
using Repositories.Interfaces;
using StudentManagementSystem.Models;

public class StudentRepository : IStudentInterface
{

    private readonly NpgsqlConnection _conn;
    public StudentRepository(NpgsqlConnection connection)
    {
        _conn = connection;
    }

    public async Task<int> Add(t_Student data)
    {
        try
        {
            NpgsqlCommand cm = new NpgsqlCommand(@"INSERT INTO t_student (c_name,c_email,c_dob,c_mobile_no,c_gender,c_password,c_classid,c_sectionid,c_guardian_name,c_enroll_date,c_profile_pic,c_status) values ( @c_name, @c_email, @c_dob, @c_mobile_no, @c_gender,@c_password, @c_classid, @c_sectionid, @c_guardian_name, @c_enroll_date, @c_profile_pic,@c_status)", _conn);
            cm.Parameters.AddWithValue("@c_name", data.c_studentName);
            cm.Parameters.AddWithValue("@c_email", data.c_studentEmail);
            cm.Parameters.AddWithValue("@c_dob", data.c_studentDOB);
            cm.Parameters.AddWithValue("@c_mobile_no", data.c_studentPhone);
            cm.Parameters.AddWithValue("@c_gender", data.c_studentGender);
            cm.Parameters.AddWithValue("@c_password", data.c_password);
            cm.Parameters.AddWithValue("@c_classid", data.c_class.c_classId);
            cm.Parameters.AddWithValue("@c_sectionid", data.c_Section.c_sectionId);
            cm.Parameters.AddWithValue("@c_guardian_name", data.c_studentGuardianDetails);
            cm.Parameters.AddWithValue("@c_enroll_date", data.c_studentEnrollDate);
            cm.Parameters.AddWithValue("@c_profile_pic", data.c_studentProfile == null ? DBNull.Value : data.c_studentProfile);
            cm.Parameters.AddWithValue("@c_status", data.c_studentStatus);
            _conn.Close();
            _conn.Open();
            cm.ExecuteNonQuery();
            _conn.Close();
            return 1;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("Add karvama problem chhe, repository ---> add" + ex);
            return 0;
        }
    }

    public async Task<int> Delete(int studentid)
    {
        try
        {
            NpgsqlCommand cm = new NpgsqlCommand(@"DELETE FROM t_student where c_id=@c_id", _conn);
            cm.Parameters.AddWithValue("@c_id", studentid);
            _conn.Close();
            _conn.Open();
            cm.ExecuteNonQuery();
            _conn.Close();
            return 1;
        }
        catch (Exception e)
        {
            System.Console.WriteLine("error in delete" + e.Message);
            return 0;
        }
    }

    public async Task<List<t_Student>> GetAll()
    {
        DataTable dt = new DataTable();
        NpgsqlCommand cm = new NpgsqlCommand(@"SELECT 
                        s.c_id, s.c_name, s.c_email, s.c_dob, s.c_mobile_no, 
                        s.c_gender, s.c_password, s.c_guardian_name, s.c_enroll_date, 
                        s.c_profile_pic, s.c_status, 
                        c.c_classid, c.c_className, 
                        sec.c_sectionid, sec.c_sectionName 
                    FROM t_student s
                    INNER JOIN t_class c ON s.c_classid = c.c_classid
                    INNER JOIN t_section sec ON s.c_sectionid = sec.c_sectionid", _conn);
        _conn.Close();
        _conn.Open();
        NpgsqlDataReader dr = cm.ExecuteReader();
        if (dr.HasRows)
        {
            dt.Load(dr);
        }
        List<t_Student> studentList = new List<t_Student>();
        studentList = (from DataRow r in dt.Rows
                       select new t_Student()
                       {
                           c_studentId = Convert.ToInt32(r["c_id"]),
                           c_studentName = r["c_name"].ToString(),
                           c_studentEmail = r["c_email"].ToString(),
                           c_studentDOB = Convert.ToDateTime(r["c_dob"].ToString()),
                           c_studentPhone = r["c_mobile_no"].ToString(),
                           c_studentGender = r["c_gender"].ToString(),
                           c_password = r["c_password"].ToString(),
                           c_studentGuardianDetails = r["c_guardian_name"].ToString(),
                           c_studentEnrollDate = Convert.ToDateTime(r["c_enroll_date"].ToString()),
                           c_studentStatus = r["c_profile_pic"].ToString(),
                           c_studentProfile = r["c_status"].ToString(),
                           c_class = new t_Class
                           {
                               c_classId = Convert.ToInt32(r["c_classid"]),
                               c_className = r["c_className"].ToString()
                           },
                           c_Section = new t_Section
                           {
                               c_sectionId = Convert.ToInt32(r["c_sectionid"]),
                               c_sectionName = r["c_sectionName"].ToString(),
                               c_classid = Convert.ToInt32(r["c_classid"].ToString())
                           }

                       }).ToList();
        _conn.Close();
        return studentList;
    }


    public async Task<t_Student> GetOne(string studentid)
    {
        t_Student student = null;
        _conn.Open();

        using (NpgsqlCommand cm = new NpgsqlCommand(@"SELECT 
                        s.c_id, s.c_name, s.c_email, s.c_dob, s.c_mobile_no, 
                        s.c_gender, s.c_password, s.c_guardian_name, s.c_enroll_date, 
                        s.c_profile_pic, s.c_status, 
                        c.c_classid, c.c_className, 
                        sec.c_sectionid, sec.c_sectionName 
                    FROM t_student s
                    INNER JOIN t_class c ON s.c_classid = c.c_classid
                    INNER JOIN t_section sec ON s.c_sectionid = sec.c_sectionid
                    WHERE s.c_id = @c_id", _conn))
        {
            cm.Parameters.AddWithValue("@c_id", int.Parse(studentid));

            using (NpgsqlDataReader r = await cm.ExecuteReaderAsync())
            {
                if (r.Read()) // Check if there is any data
                {
                    student = new t_Student()
                    {
                        c_studentId = Convert.ToInt32(r["c_id"]),
                        c_studentName = r["c_name"].ToString(),
                        c_studentEmail = r["c_email"].ToString(),
                        c_studentDOB = Convert.ToDateTime(r["c_dob"].ToString()),
                        c_studentPhone = r["c_mobile_no"].ToString(),
                        c_studentGender = r["c_gender"].ToString(),
                        c_password = r["c_password"].ToString(),
                        c_studentGuardianDetails = r["c_guardian_name"].ToString(),
                        c_studentEnrollDate = Convert.ToDateTime(r["c_enroll_date"].ToString()),
                        c_studentStatus = r["c_profile_pic"].ToString(),
                        c_studentProfile = r["c_status"].ToString(),
                        c_class = new t_Class
                        {
                            c_classId = Convert.ToInt32(r["c_classid"]),
                            c_className = r["c_className"].ToString()
                        },
                        c_Section = new t_Section
                        {
                            c_sectionId = Convert.ToInt32(r["c_sectionid"]),
                            c_sectionName = r["c_sectionName"].ToString(),
                            c_classid = Convert.ToInt32(r["c_classid"].ToString())
                        }
                    };
                }
            }
        }
        _conn.Close();

        return student;
    }

    public async Task<int> Update(t_Student data)
    {
        try
        {
            using (NpgsqlCommand cm = new NpgsqlCommand(@"UPDATE t_student SET 
                c_name = @c_name, 
                c_email = @c_email, 
                c_dob = @c_dob, 
                c_mobile_no = @c_mobile_no, 
                c_gender = @c_gender, 
                c_password = @c_password, 
                c_classid = @c_classid, 
                c_sectionid = @c_sectionid, 
                c_guardian_name = @c_guardian_name, 
                c_enroll_date = @c_enroll_date, 
                c_profile_pic = @c_profile_pic, 
                c_status = @c_status
                WHERE c_id = @c_id", _conn))
            {
                cm.Parameters.AddWithValue("@c_id", data.c_studentId);
                cm.Parameters.AddWithValue("@c_name", data.c_studentName);
                cm.Parameters.AddWithValue("@c_email", data.c_studentEmail);
                cm.Parameters.AddWithValue("@c_dob", data.c_studentDOB);
                cm.Parameters.AddWithValue("@c_mobile_no", data.c_studentPhone);
                cm.Parameters.AddWithValue("@c_gender", data.c_studentGender);
                cm.Parameters.AddWithValue("@c_password", data.c_password);
                cm.Parameters.AddWithValue("@c_classid", data.c_class.c_classId);
                cm.Parameters.AddWithValue("@c_sectionid", data.c_Section.c_sectionId);
                cm.Parameters.AddWithValue("@c_guardian_name", data.c_studentGuardianDetails);
                cm.Parameters.AddWithValue("@c_enroll_date", data.c_studentEnrollDate);
                cm.Parameters.AddWithValue("@c_profile_pic", data.c_studentProfile ?? (object)DBNull.Value);
                cm.Parameters.AddWithValue("@c_status", data.c_studentStatus);

                _conn.Open();
                int rowsAffected = await cm.ExecuteNonQueryAsync();
                _conn.Close();

                return rowsAffected;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in Update Student in student repository: " + ex.Message);
            return 0;
        }
    }

    public async Task<List<t_Class>> GetClasses()
    {
        List<t_Class> rooms = new List<t_Class>();
        try
        {
            DataTable dt = new DataTable();
            _conn.Close();
            NpgsqlCommand cmd = new NpgsqlCommand(@"select * from t_class", _conn);
            _conn.Close();
            _conn.Open();
            NpgsqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader.HasRows)
            {
                dt.Load(dataReader);
            }

            rooms = (from DataRow dr in dt.Rows
                     select new t_Class()
                     {
                         c_classId = Convert.ToInt32(dr["c_classid"]),
                         c_className = dr["c_className"].ToString()
                     }
            ).ToList();
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("class Getiing error :" + ex);
        }
        _conn.Close();
        return rooms;
    }

    public async Task<List<t_Section>> GetSectionsByClassId(int id)
    {
        List<t_Section> tc = new List<t_Section>();
        try
        {
            DataTable dt = new DataTable();
            _conn.Open();
            string qry = "select * from t_section where c_classid = @c_classid";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_classid", id);
            var dataReader = cmd.ExecuteReader();

            if (dataReader.HasRows)
            {
                dt.Load(dataReader);
            }
            tc = (from DataRow r in dt.Rows
                  select new t_Section()
                  {
                      c_sectionId = Convert.ToInt32(r["c_sectionid"]),
                      c_sectionName = r["c_sectionName"].ToString(),
                      c_classid = Convert.ToInt32(r["c_classid"].ToString())
                  }
            ).ToList();
            _conn.Close();
        }
        catch (System.Exception e)
        {
            System.Console.WriteLine(e.Message);
        }
        return tc;
    }

}

