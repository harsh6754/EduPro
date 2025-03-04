using System;
using Repositories.Models;

namespace EduProj.Models
{
    public class vm_Material
    {
        public int c_material_id { get; set; } // Primary key
        public string c_fileName { get; set; } // Name of the file
        public string c_fileType { get; set; }

        public string c_fileData { get; set; } // Type of the file (e.g., PDF, DOCX)
        public DateTime c_uploadDate { get; set; } // Date when the file was uploaded
        public int? c_subject_id { get; set; } // Foreign key to the subject
        public int? c_teacher_id { get; set; } // Foreign key to the teacher

        // Optional: Add navigation properties if needed
        public t_subject c_subject { get; set; } // Navigation property for subject
        public t_Teacher c_teacher { get; set; } // Navigation property for teacher
    }
}