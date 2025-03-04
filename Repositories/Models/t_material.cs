using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Repositories.Models
{
    public class t_material
    {
  [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? TeacherId { get; set; } // For teacher-related information, can be filled from session or context.

        [Key]
        public int MaterialId { get; set; } // Unique identifier for the material

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } // The name of the uploaded file

        [Required]
        [StringLength(50)]
        public string FileType { get; set; } // The file's extension (PDF, DOCX, etc.)

        [Required]
        public string? FilePath { get; set; } // The path where the file is saved

        [Required]
        public DateTime UploadDate { get; set; } // Date when the material is uploaded

        [Required]
        public int SubjectId { get; set; } // The subject associated with the material, selected from a dropdown list
        public string? SubjectName { get; set; } // The name of the subject, can be filled from the database

    }
         
}
