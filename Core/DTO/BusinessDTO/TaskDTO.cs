using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TimeSync.Core.DTO.BusinessDTO
{
    public class TaskDTO
    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public required string TaskName { get; set; }

        public required string TaskDescription { get; set; }

        public TimeSpan TaskTime { get; set; } // Represents the time of the task

        public DateOnly TaskDate { get; set; } // Represents the date of the task

        public int Period { get; set; } // Represents the period in minutes/hours

        public bool Notification { get; set; } // Represents if notifications are enabled

        public DateTime? LastUpdated { get; set; }

        public DateTime? LastNotificationSent { get; set; }
    }
}
