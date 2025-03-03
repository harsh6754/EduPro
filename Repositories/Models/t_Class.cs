using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagementSystem.Models
{
    public class t_Class
    {
        public int c_classId { get; set; }
        public string? c_className { get; set; }

        public t_Section? c_section { get; set; }
    }
}