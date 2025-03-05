using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface ITeacherRating
    {
    // Task<List<TeacherInfo>> GetTeachersByClassIdAsync(int classId);
    Task<t_TeacherRating> InsertTeacherRatingAsync(int c_stud_id, int c_teacher_id, int c_rating);
    Task<t_material> GetLatestUploadedFile();

    Task<List<TeacherInfo>> GetTeachersByStudentId(int classId);
    }
}