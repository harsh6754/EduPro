using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Models;
using Microsoft.AspNetCore.Http;
using StudentManagementSystem.Models;


namespace Repositories.Models
{
    public class t_Exam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CEid { get; set; }
        public int CClassId { get; set; }
        public t_Class? Class{get;set;}
        public string? CImage { get; set; }
        public IFormFile? Image { get; set; }
    }
}