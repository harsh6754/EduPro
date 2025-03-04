using System.Collections.Generic;

namespace StudentManagementSystem.Models
{
    public class TeacherTreeViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<TeacherTreeViewModel> Items { get; set; }
    }
}

