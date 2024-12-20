using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TimeSync.Core.model
{
    public class Tasks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Users")]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public required string TaskName { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public required string TaskDescription { get; set; }

        [Required]
        public TimeSpan TaskTime { get; set; } // Represents the time of the task

        [Required]
        public DateOnly TaskDate { get; set; } // Represents the date of the task

        [Required]
        public int Period { get; set; } // Represents the period in minutes/hours

        [Required]
        public bool Notification { get; set; } // Represents if notifications are enabled

        public DateTime? LastUpdated { get; set; } 

        public DateTime? LastNotificationSent { get; set; }


    }
}
