using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Models
{
    public class t_topic
    {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_tid{get;set;}

    public string c_topicname{get;set;}

    public int c_subject_id{get;set;}
    }
}