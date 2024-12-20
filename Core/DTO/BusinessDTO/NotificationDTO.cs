namespace TimeSync.Core.DTO.BusinessDTO
{
    public class NotificationDTO
    {
        public int Id { get; set; } // Unique identifier for the notification
        public int UserId { get; set; } // ID of the user the notification is for
        public string message { get; set; }

        public string response { get; set; }

        public int TaskId { get; set; } // Task ID associated with the notification
        public bool? IsFav { get; set; } // Whether the notification is marked as favorite (nullable)

    }
}
