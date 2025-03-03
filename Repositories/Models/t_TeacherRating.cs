using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    
    public class t_TeacherRating
    {

        [Required]
        [Column("c_stud_id")]
        public int c_stud_id { get; set; }

        [Required]
        [Column("c_teacher_id")]
        public int c_teacher_id { get; set; }

        [Required]
        [Column("c_rating")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int c_rating { get; set; }



    }
}