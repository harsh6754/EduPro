using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace StudentManagementSystem.Models
{
    public class t_Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int c_studentId { get; set; }

        [Required(ErrorMessage = "Please enter student name")]
        [Display(Name = "Student Name")]
        [StringLength(100)]
        public string c_studentName { get; set; }

        [Required(ErrorMessage = "Please enter student email")]
        [Display(Name = "Student Email")]
        [StringLength(100)]
        public string c_studentEmail { get; set; }

        [Required(ErrorMessage = "Please Enter Student Phone Number")]
        [Display(Name = "Student Phone Number")]
        [StringLength(10)]
        public string c_studentPhone { get; set; }

        public string c_password { get; set; }

        [Required(ErrorMessage = "Please enter Student DOB")]
        [Display(Name = "Student DOB")]
        [DataType(DataType.Date)]
        public DateTime c_studentDOB { get; set; }

        [Required(ErrorMessage = "Please enter Gender")]
        [Display(Name = "Gender")]
        public string c_studentGender { get; set; }

        public t_Class c_class { get; set; }
        public int c_sectionid { get; set; }   
        public int c_classid { get; set; }   


        [Required(ErrorMessage = "Please enter Student Guardian Details")]
        [Display(Name = "Student Guardian Details")]
        public string c_studentGuardianDetails { get; set; }

        [Required(ErrorMessage = "Please enter Student Enroll Date")]
        [Display(Name = "Student Enroll Date")]
        [DataType(DataType.Date)]
        public DateTime c_studentEnrollDate { get; set; }

        [Required(ErrorMessage = "Please Add Student Profile")]
        [Display(Name = "Student Profile")]
        public string c_studentProfile { get; set; }

        public IFormFile? StudentPic { get; set; } // ✅ Nullable (optional)

        [Required(ErrorMessage = "Student Status")]
        [Display(Name = "Student Status")]
        public string c_studentStatus { get; set; }

    }
}