using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("t_classschedule")]
public class t_Timetable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? c_scheduleid { get; set; }

    public int c_classid { get; set; }

    public TimeSpan c_starttime { get; set; }  
    public TimeSpan c_endtime { get; set; }    

    public string c_weekday { get; set; }
    public int c_subjectid { get; set; }
    public int c_teacherid { get; set; }

    public string? c_SubjectName { get; set; }
    public string? c_TeacherName { get; set; }
}