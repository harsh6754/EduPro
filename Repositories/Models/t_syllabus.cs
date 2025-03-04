using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Models;
using StudentManagementSystem.Models;

public class t_syllabus
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_syllabus_id{get;set;}

    public t_subject? subjects1{get;set;}
    public int c_subject_id{get;set;}
    public string c_topicName{get;set;}

    public t_Class? class1{get;set;}
    public int c_class_id{get;set;}
}