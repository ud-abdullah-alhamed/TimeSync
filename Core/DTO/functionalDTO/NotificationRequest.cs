namespace TimeSync.Core.DTO.functionalDTO
{
    public class NotificationRequest
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string route { get; set; }
        public string ImageUrl { get; set; }  // Optional image URL
        public List<string> ExternalIds { get; set; } // List of external user IDs to target


    }
}
