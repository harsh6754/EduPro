using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagementSystem.Models
{
    public class t_classschedule
    {
   [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
        
        [Column("c_scheduleid")]
        public int ScheduleId { get; set; }

        // [ForeignKey("Class")]
        [Column("c_classid")]
        public int? ClassId { get; set; }

        [Column("c_starttime")]
        public TimeSpan? StartTime { get; set; }

        [Column("c_endtime")]
        public TimeSpan? EndTime { get; set; }

        [Column("c_weekday")]
        [StringLength(10)]
        public string Weekday { get; set; }

        // [ForeignKey("Subject")]
        [Column("c_subjectid")]
        public int? SubjectId { get; set; }

        // [ForeignKey("Teacher")]
        [Column("c_teacherid")]
        public int? TeacherId { get; set; }

        
}
}

    