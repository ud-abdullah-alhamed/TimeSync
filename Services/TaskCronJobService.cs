using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System.Text;
using System.Text.Json;
using TimeSync.Core.model;
using TimeSync.Infrastructure;
using System.Text.Json;
using Microsoft.AspNetCore.Routing;
using Azure;
using MySqlConnector;
using Dapper;
namespace TimeSync.Services
{
    public class TaskCronJobService
    {
        private readonly TaskContext _taskContext;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly string _onesignalAppId;
        private readonly string _onesignalRestApiKey;
        private readonly string _apiKey;
        private readonly NotificationService _notificationService;

        public TaskCronJobService(
           TaskContext taskContext,
           IBackgroundJobClient backgroundJobClient,
           IConfiguration configuration)
        {
            _taskContext = taskContext;
            _backgroundJobClient = backgroundJobClient;
            _onesignalAppId = configuration["OneSignal:AppId"];
            _onesignalRestApiKey = configuration["OneSignal:RestApiKey"];
            _apiKey = configuration["OpenAI:ApiKey"];
        }

        public void ScheduleTaskNotifications()
        {
            // Cron job scheduled to run every hour to check for tasks
            RecurringJob.AddOrUpdate(
                "check-tasks-for-notifications",
                () => CheckTasksForNotificationAsync(),
                Cron.Hourly); // You can adjust the frequency to your needs (e.g., Cron.Daily)
        }

        public async Task CheckTasksForNotificationAsync()
        {
            // Query tasks where notifications are enabled
            var tasks = await _taskContext.Tasks
                .Where(t => t.Notification == true)
                .ToListAsync();

            foreach (var task in tasks)
            {
                // Check if the task period has passed
                var hoursSinceLastUpdate = task.LastUpdated.HasValue
    ? (DateTime.Now - task.LastUpdated.Value).TotalHours
    : 0; // Default value when LastUpdated is null

                // If the task period has passed based on the notification period, schedule a job
                if (hoursSinceLastUpdate  <= task.Period)
                {
                    // Create a background job for sending notification
                    _backgroundJobClient.Enqueue(() => SendTaskNotificationAsync(task));
                    Console.WriteLine("Process successfully!");
                    // Optionally, you could update the task to reset the period or mark it as notified
                    task.LastUpdated = DateTime.Now;
                    _taskContext.Update(task);
                    Console.WriteLine("Process successfullyyyyyyyyyyyy!");
                    await _taskContext.SaveChangesAsync();
                }
            }
        }

        // This method is called when the cron job triggers the notification job
      public async Task SendTaskNotificationAsync(Tasks tasks)
{
    var message = $"For a notification, give me information to study the task '{tasks.TaskName}' in one line.";

    try
    {
        var chatGptResponse = await GetChatGPTResponse(message);

        var notificationContent = new
        {
            app_id = _onesignalAppId,
            contents = new { en = $"{tasks.TaskName}: {chatGptResponse}" },
            include_aliases = new
            {
                external_id = new[] { tasks.UserId.ToString() } // Ensure this is an array of strings
            },
            data = new { notification_type = "custom" },
            target_channel = "push"
        };

        var json = JsonConvert.SerializeObject(notificationContent);
        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };
        client.DefaultRequestHeaders.Add("Authorization", "Basic " + _onesignalRestApiKey);

        var response = await client.PostAsync("https://onesignal.com/api/v1/notifications", new StringContent(json, Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Notification sent successfully!");
                    Console.WriteLine(tasks.UserId );
                    Console.WriteLine(tasks.Id);
                    Console.WriteLine(message);
                    Console.WriteLine(chatGptResponse);
                    Console.WriteLine(false);

                    var insertQuery = @"
            INSERT INTO TimeSync.Notifications
            (
                UserId,
                TimeSent,
                message,
                response,
                TaskId,
                IsFav
            )
            VALUES
            (
                @UserId,
                @TimeSent,
                @Message,
                @Response,
                @TaskId,
                @IsFav
            );";

                    using (var connection = new MySqlConnection("Server=166.1.227.102;Database=TimeSync;User=dev_user;Password=dev_password;Allow User Variables=true;Connection Timeout=30;"))
                    {
                        var parameters = new
                        {
                            UserId = tasks.UserId,
                            TimeSent = DateTime.UtcNow,
                            Message = message ?? string.Empty,
                            Response = chatGptResponse ?? string.Empty,
                            TaskId = tasks.Id,
                            IsFav = false // or your desired value
                        };

                        await connection.ExecuteAsync(insertQuery, parameters);
                    }

                   // await _notificationService.SaveNotificationAsync(tasks.UserId, tasks.Id, null, message ?? "", chatGptResponse ?? "");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Failed to send notification. Error: {error}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending notification: {ex.Message}");
    }
}


        public async Task<string> GetChatGPTResponse(string message)
        {
            var client = new RestClient("https://api.openai.com/v1/chat/completions");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Authorization", $"Bearer {_apiKey}");
            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = message }
                },
                max_tokens = 100,
                temperature = 0.7
            };

            request.AddJsonBody(body);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var responseData = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(response.Content);
               Console.WriteLine(responseData.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString());
                return responseData.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

           
            }

            throw new Exception("Failed to get response from ChatGPT: " + response.ErrorMessage);
        }


        public class NoAuthorizationFilter : Hangfire.Dashboard.IDashboardAuthorizationFilter
        {
            public bool Authorize(Hangfire.Dashboard.DashboardContext context)
            {
                return true; // Always return true to allow access without authorization
            }
        }

    }
}
