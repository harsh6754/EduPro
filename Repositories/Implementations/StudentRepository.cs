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
            // cm.Parameters.AddWithValue("@c_userid", data.c_studentId);
            cm.Parameters.AddWithValue("@c_name", data.c_studentName);
            cm.Parameters.AddWithValue("@c_email", data.c_studentEmail);
            cm.Parameters.AddWithValue("@c_dob", data.c_studentDOB);
            cm.Parameters.AddWithValue("@c_mobile_no", data.c_studentPhone);
            cm.Parameters.AddWithValue("@c_gender", data.c_studentGender);
            cm.Parameters.AddWithValue("@c_password", data.c_password);
            cm.Parameters.AddWithValue("@c_classid", data.c_class);
            cm.Parameters.AddWithValue("@c_sectionid", data.c_sectionid);
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
            return 0;
        }
    }

    public async Task<int> Delete(string studentid)
    {
        try
        {
            NpgsqlCommand cm = new NpgsqlCommand(@"DELETE FROM t_student where c_id=@c_id", _conn);
            cm.Parameters.AddWithValue("@c_id", int.Parse(studentid));
            _conn.Close();
            _conn.Open();
            cm.ExecuteNonQuery();
            _conn.Close();
            return 1;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<List<t_Student>> GetAll()
    {
        DataTable dt = new DataTable();
        NpgsqlCommand cm = new NpgsqlCommand("select * from t_student", _conn);
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
                           c_classid = Convert.ToInt32(r["c_classid"].ToString()),
                           c_sectionid = Convert.ToInt32(r["c_sectionid"].ToString()),
                           c_studentGuardianDetails = r["c_guardian_name"].ToString(),
                           c_studentEnrollDate = Convert.ToDateTime(r["c_enroll_date"].ToString()),
                           c_studentStatus = r["c_profile_pic"].ToString(),
                           c_studentProfile = r["c_status"].ToString()
                       }).ToList();
        _conn.Close();
        return studentList;
    }

    public async Task<List<t_Student>> GetAllByUser(string userid)
    {
        DataTable dt = new DataTable();
        List<t_Student> studentList = new List<t_Student>();
        _conn.Open();
        using (NpgsqlCommand cm = new NpgsqlCommand("select * from t_student where c_id = @c_id", _conn))
        {
            cm.Parameters.AddWithValue("@c_id", int.Parse(userid));
            // Execute the reader
            using (NpgsqlDataReader dr = await cm.ExecuteReaderAsync())
            {
                if (dr.HasRows)
                {
                    dt.Load(dr);
                }
                studentList = (from DataRow r in dt.Rows
                               where r["c_userid"].ToString() == userid
                               select new t_Student()
                               {
                                   c_studentId = Convert.ToInt32(r["c_id"]),
                                   c_studentName = r["c_name"].ToString(),
                                   c_studentEmail = r["c_email"].ToString(),
                                   c_studentDOB = Convert.ToDateTime(r["c_dob"].ToString()),
                                   c_studentPhone = r["c_mobile_no"].ToString(),
                                   c_studentGender = r["c_gender"].ToString(),
                                   c_password = r["c_password"].ToString(),
                                   c_classid = Convert.ToInt32(r["c_classid"].ToString()),
                                   c_sectionid = Convert.ToInt32(r["c_sectionid"].ToString()),
                                   c_studentGuardianDetails = r["c_guardian_name"].ToString(),
                                   c_studentEnrollDate = Convert.ToDateTime(r["c_enroll_date"].ToString()),
                                   c_studentStatus = r["c_profile_pic"].ToString(),
                                   c_studentProfile = r["c_status"].ToString()
                               }).ToList();
            }
        }
        _conn.Close();
        return studentList;
    }

    public async Task<t_Student> GetOne(string studentid)
    {
        t_Student student = null;

        // Open the connection
        _conn.Open();

        using (NpgsqlCommand cm = new NpgsqlCommand("select * from t_student where c_id=@c_id", _conn))
        {
            cm.Parameters.AddWithValue("@c_id", int.Parse(studentid));

            // Execute the reader
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
                        c_classid = Convert.ToInt32(r["c_classid"].ToString()),
                        c_sectionid = Convert.ToInt32(r["c_sectionid"].ToString()),
                        c_studentGuardianDetails = r["c_guardian_name"].ToString(),
                        c_studentEnrollDate = Convert.ToDateTime(r["c_enroll_date"].ToString()),
                        c_studentStatus = r["c_profile_pic"].ToString(),
                        c_studentProfile = r["c_status"].ToString()
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
            using (
            NpgsqlCommand cm = new NpgsqlCommand(@"UPDATE t_student set c_id = @c_id, c_name = @c_name,c_email=@c_email, c_dob=@c_dob, c_mobile_no=@c_mobile_no, c_gender=@c_gender, c_password = @c_password, c_classid = @c_classid, c_sectionid = @c_sectionid, c_guardian_name = @c_guardian_name, c_enroll_date = @c_enroll_date, c_profile_pic = @c_profile_pic, c_status = @c_status", _conn))
            {
                cm.Parameters.AddWithValue("@c_id", data.c_studentId);
                cm.Parameters.AddWithValue("@c_name", data.c_studentName);
                cm.Parameters.AddWithValue("@c_email", data.c_studentEmail);
                cm.Parameters.AddWithValue("@c_dob", data.c_studentDOB);
                cm.Parameters.AddWithValue("@c_mobile_no", data.c_studentPhone);
                cm.Parameters.AddWithValue("@c_gender", data.c_studentGender);
                cm.Parameters.AddWithValue("@c_password", data.c_password);
                cm.Parameters.AddWithValue("@c_classid", data.c_class);
                cm.Parameters.AddWithValue("@c_sectionid", data.c_sectionid);
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
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("fuck error in contactRepository --> Update" + ex);
            return 0;
        }
    }

}

