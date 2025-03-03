using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagementSystem.Models
{
    public class t_student_view
    {
         [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Please enter student name")]
        [Display(Name = "Student Name")]
        [StringLength(100)]
        public string c_studentName { get; set; }

          
          public int c_classId { get; set; }



    }

}