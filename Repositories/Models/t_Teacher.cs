using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Models
{
    public class t_Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [Column("c_tid")]
        public int TeacherId { get; set; }

        [Required]
        [Column("c_TeacherName")]
        [StringLength(50)]
        public string T_Name { get; set; }

        [Required]
        [Column("c_temail")]
        [StringLength(50)]
        [EmailAddress]
        public string T_Email { get; set; }

        [Required]
        [Column("c_tpassword")]
        [StringLength(100)]
        public string T_PasswordHash { get; set; } // Store as a hashed password

        [Required]
        [Column("c_tmobno")]
        public long T_MobileNumber { get; set; } // Stored as BIGINT

        [Required]
        [Column("c_tdob")]
        public DateTime T_DateOfBirth { get; set; }

        [Required]
        [Column("c_tQualification")]
        [StringLength(200)]
        public string T_Qualification { get; set; }

        [Required]
        [Column("c_experience")]
        public int T_Experience { get; set; }

        [Required]
        [Column("c_expert_subject")]
        [StringLength(200)]
        public string T_ExpertSubject { get; set; }

        [Column("c_class_id")]
        
        public int? T_Class_Id { get; set; }

        [Column("c_subjectId")]
        public int? T_SubjectId { get; set; } // Nullable Foreign Key

        public static implicit operator t_Teacher(t_teacherGet v)
        {
            throw new NotImplementedException();
        }

        // [ForeignKey("SubjectId")]
        // public virtual Subject? Subject { get; set; } // Navigation Property 

        public class t_teacherGet
{
    [Key]
    public int c_tid{get;set;}

    public string c_tName{get;set;}    
}

public class t_teacher_Assign
{
    [Key]
    public int c_tid{get;set;}

    public int c_subject_id{get;set;}
    public int c_class_id{get;set;}
}
}
}