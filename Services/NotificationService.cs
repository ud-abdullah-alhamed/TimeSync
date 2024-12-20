using Microsoft.EntityFrameworkCore;
using System;
using TimeSync.Core.model;
using TimeSync.Infrastructure;

namespace TimeSync.Services
{
    public class NotificationService
    {

        private readonly NotificationContext _context;
        private readonly TaskContext _taskContext;

        public NotificationService(NotificationContext context, TaskContext taskContext)
        {
            _context = context;
            _taskContext = taskContext; 
        }

        // Save the notification to the database
        public async Task SaveNotificationAsync(int userId, int taskId, bool? isFav, string message, string response)
        {
            // Null or invalid value checks
            if (userId == 0)
            {
                throw new ArgumentException("UserId is invalid.");
            }

            if (taskId == 0)
            {
                throw new ArgumentException("TaskId is invalid.");
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message), "Message cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(response))
            {
                throw new ArgumentNullException(nameof(response), "Response cannot be null or empty.");
            }

            var notification = new Notification
            {
                UserId = userId,
                TimeSent = DateTime.UtcNow, // Save in UTC time
                TaskId = taskId,
                IsFav = isFav,
                message = message,
                response = response
            };

            try
            {
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the error and rethrow it
                Console.WriteLine($"Error saving notification: {ex.Message}");
                throw;
            }
        }


        // Get notifications by User ID
        public async Task<object> GetNotificationsByUserIdAsync(int userId)
        {
            // Retrieve notifications associated with the specified user ID
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .ToListAsync();

            // Check if there are no notifications for the given user ID
            if (!notifications.Any())
            {
                return "There are no notifications for this user ID.";
            }

            // Return the list of notifications
            return notifications;
        }

        // Get notifications by Task ID
        public async Task<object> GetNotificationsByTaskIdAsync(int taskId)
        {
            // Retrieve notifications associated with the specified task ID
            var notifications = await _context.Notifications
                .Where(n => n.TaskId == taskId)
                .ToListAsync();

            // Check if there are no notifications for the given task ID
            if (!notifications.Any())
            {
                return "There are no notifications for this task ID.";
            }

            // Return the list of notifications
            return notifications;
        }

        // Update IsFav field
        public async Task UpdateIsFavAsync(int notificationId, bool isFav)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification == null)
            {
                throw new ArgumentException("Notification not found.");
            }

            notification.IsFav = isFav;

            try
            {
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the error and rethrow it
                Console.WriteLine($"Error updating notification: {ex.Message}");
                throw;
            }
        }


        public async Task<object> GetNotificationsAndTaskByUserIdAsync(int userId)
        {
            // Retrieve notifications from the Notifications context
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .ToListAsync();

            if (!notifications.Any())
            {
                return "There are no notifications for this user ID.";
            }

            // Retrieve task details from the Task context
            var taskIds = notifications.Select(n => n.TaskId).ToList();
            var tasks = await _taskContext.Tasks
                .Where(t => taskIds.Contains(t.Id))
                .ToListAsync();

            // Combine notifications with task details
            var notificationDetails = notifications.Select(notification => new
            {
                notification.Id,
                notification.UserId,
                notification.TaskId,
                notification.IsFav,
                notification.TimeSent,
                notification.message,
                notification.response,
                TaskName = tasks.FirstOrDefault(task => task.Id == notification.TaskId)?.TaskName
            }).ToList();

            // Return the enriched list of notifications
            return notificationDetails;
        }


        public async Task<object> GetNotificationsAndTaskByUserIdWhereisFavAsync(int userId)
        {
            // Retrieve notifications from the Notifications context
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && n.IsFav == true)
                .ToListAsync();

            if (!notifications.Any())
            {
                return "There are no notifications for this user ID.";
            }

            // Retrieve task details from the Task context
            var taskIds = notifications.Select(n => n.TaskId).ToList();
            var tasks = await _taskContext.Tasks
                .Where(t => taskIds.Contains(t.Id))
                .ToListAsync();

            // Combine notifications with task details
            var notificationDetails = notifications.Select(notification => new
            {
                notification.Id,
                notification.UserId,
                notification.TaskId,
                notification.IsFav,
                notification.TimeSent,
                notification.message,
                notification.response,
                TaskName = tasks.FirstOrDefault(task => task.Id == notification.TaskId)?.TaskName
            }).ToList();

            // Return the enriched list of notifications
            return notificationDetails;
        }



    }
}
