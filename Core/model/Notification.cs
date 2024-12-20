using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TimeSync.Core.model
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Unique identifier for the notification

        [Required]
        [ForeignKey("Users")]
        public int UserId { get; set; } // ID of the user the notification is for
        public DateTime TimeSent { get; set; } // Time when the notification was sent

        [Required]
        [Column(TypeName = "varchar(100)")]
        public required string message { get; set; }
        [Required]
        [Column(TypeName = "varchar(100)")]
        public required string response { get; set; }

        public int TaskId { get; set; } // Task ID associated with the notification
        public bool? IsFav { get; set; } // Whether the notification is marked as favorite (nullable)

    }
}
