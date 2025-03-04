using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }  // Auto-incrementing Primary Key
        public int ReceiverId { get; set; }  // Student receiving notification
        public string Title { get; set; }  // Notification Title
        public string Message { get; set; }  // Notification Content
        public bool Status { get; set; } = false;  // Default Unread
        public DateTime CreatedAt { get; set; }  // Timestamp of creation
    }

    public class SlotAvailabilityRequest
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int ClassId { get; set; }
        public int TeacherId { get; set; }

        public string weekday { get; set; }
    }
}