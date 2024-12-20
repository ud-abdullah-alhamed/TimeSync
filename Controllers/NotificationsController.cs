using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using TimeSync.Core.DTO.BusinessDTO;
using TimeSync.Core.DTO.functionalDTO;
using TimeSync.Services;

namespace TimeSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private readonly string _onesignalAppId;
        private readonly string _onesignalRestApiKey;
        private readonly HttpClient _httpClient;

        public NotificationsController(NotificationService notificationService , IConfiguration configuration, HttpClient httpClient)
        {
            _notificationService = notificationService;
            _onesignalAppId = configuration["OneSignal:AppId"];
            _onesignalRestApiKey = configuration["OneSignal:RestApiKey"];
            _httpClient = httpClient;
        }


        [HttpPost("save-notification")]
        public async Task<IActionResult> SaveNotification([FromBody] NotificationDTO notificationDto)
        {
            try
            {
                await _notificationService.SaveNotificationAsync(notificationDto.UserId, notificationDto.TaskId, notificationDto.IsFav, notificationDto.message, notificationDto.response);
                return Ok(new { message = "Notification saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error saving notification: {ex.Message}" });
            }
        }


        [HttpGet("GetByTaskId/{taskId}")]
        public async Task<IActionResult> GetNotificationsByTaskId(int taskId)
        {
            var notifications = await _notificationService.GetNotificationsByTaskIdAsync(taskId);
            return notifications is string message ? NotFound(message) : Ok(notifications);
        }


        [HttpGet("GetTaskAndNotficatioByUserId/{userId}")]
        public async Task<IActionResult> GetTaskAndNotficatioByUserId(int userId)
        {
            var notifications = await _notificationService.GetNotificationsAndTaskByUserIdAsync(userId);
            return notifications is string message ? NotFound(message) : Ok(notifications);
        }


        [HttpGet("GetByUserId/{userId}")]
        public async Task<IActionResult> GetNotificationsByUserId(int userId)
        {
            var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
            return notifications is string message ? NotFound(message) : Ok(notifications);
        }

        [HttpPost("send-to-all")]
        public async Task<IActionResult> SendToAllUsers()
        {
            var notification = new
            {
                app_id = _onesignalAppId,
                included_segments = new[] { "All" },  // Send to all users
                contents = new { en = "Hi Users" },
                headings = new { en = "Greetings" },
                big_picture = "https://gratisography.com/wp-content/uploads/2024/01/gratisography-cyber-kitty-800x525.jpg" // Image URL here

            };

            var json = JsonConvert.SerializeObject(notification);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications")
            {
                Headers =
                {
                    { "Authorization", $"Basic {_onesignalRestApiKey}" }
                },
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { message = "Notification sent to all users successfully." });
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, error);
            }
        }

        // Add the endpoint to update IsFav field
        [HttpPut("update-isfav/")]
        public async Task<IActionResult> UpdateIsFav(UpdateNotFav notification)
        {
            try
            {
                await _notificationService.UpdateIsFavAsync(notification.NotificationId, notification.isFav);
                return Ok(new { message = "Notification favorite status updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating notification: {ex.Message}" });
            }
        }


        // New endpoint to get notifications and task names by User ID where IsFav is true
        [HttpGet("GetNotificationsAndTaskByUserIdWhereisFav/{userId}")]
        public async Task<IActionResult> GetNotificationsAndTaskByUserIdWhereisFav(int userId) 
        { var notifications = await _notificationService.GetNotificationsAndTaskByUserIdWhereisFavAsync(userId);
          return notifications is string message ? NotFound(message) : Ok(notifications); }





        [HttpPost("SendNotificationToAliases")]
        public async Task<IActionResult> SendNotificationToAliases([FromBody] NotificationRequest notificationRequest)
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications");

            // Building the body for OneSignal API request
            var body = new
            {
                app_id = _onesignalAppId, // Your OneSignal App ID
                include_aliases = new
                {
                    external_id = notificationRequest.ExternalIds // List of external_ids (aliases) to target
                },
                data = new
                {
                    notification_type = "custom", // Custom notification type
                    route = notificationRequest.route // Additional data, indicating that the route is "cart"
                },
                contents = new { en = notificationRequest.Message }, // Message content
                headings = new { en = notificationRequest.Title }, // Notification title
                //data = new { notification_type = "custom" }, // Optional custom data
                idempotency_key = Guid.NewGuid().ToString(), // Unique key to prevent duplicate deliveries
                target_channel = "push", // Specify the channel as "push" for push notifications
                big_picture = notificationRequest.ImageUrl, // Include the image URL in the notification
                                                            // Include the image URL in the notification
                small_icon = "ic_notification", // Specify the name of the small icon (must be included in your app’s drawable resources)
                large_icon = "https://firebasestorage.googleapis.com/v0/b/gens-3bb19.appspot.com/o/uploads%2Flogo.png?alt=media&token=78639472-ef32-4a10-b57f-80ea5ae8d656" // URL of the large icon to be displayed in the notification

            };

            // Adding OneSignal authorization headers
            request.Headers.Add("Authorization", $"Basic {_onesignalRestApiKey}");
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            // Sending the notification request
            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return Ok("Notification sent successfully to specified external IDs.");
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return BadRequest($"Failed to send notification. Error: {responseContent}");
            }
        }
    }
}
