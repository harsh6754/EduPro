using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudentManagementSystem.Models;


namespace Repositories.Models
{
    public class t_syllabuscomplete
    {
        [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_lecture_id{get;set;}

    public t_Class? class1{get;set;}
    public int c_class_id{get;set;}

    public t_subject? subjects1{get;set;}
    public int c_subject_id{get;set;}

    public t_Teacher? t_TeacherGet1{get;set;}
    public int c_teacher_id{get;set;}
    public string? c_topicsName{get;set;}
    public string c_lectureDate{get;set;}
    }
}