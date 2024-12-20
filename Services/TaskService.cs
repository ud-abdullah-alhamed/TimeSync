using Microsoft.EntityFrameworkCore;
using TimeSync.Core.DTO.BusinessDTO;
using TimeSync.Core.model;
using TimeSync.Infrastructure;

namespace TimeSync.Services
{
    public class TaskService
    {
        private readonly TaskContext _taskContext;

        public TaskService(TaskContext taskContext)
        {
            _taskContext = taskContext;
        }

        // Create Task
        public async Task<TaskDTO> CreateTaskAsync(TaskDTO taskDto)
        {
            var task = new Tasks
            {
                TaskName = taskDto.TaskName,
                TaskDescription = taskDto.TaskDescription,
                TaskTime = taskDto.TaskTime,
                TaskDate = taskDto.TaskDate,
                Period = taskDto.Period,
                Notification = taskDto.Notification ? true : false ,
                UserId = taskDto.UserId
            };

            _taskContext.Tasks.Add(task);
            await _taskContext.SaveChangesAsync();

            taskDto.Id = task.Id;
            return taskDto;
        }

        // Get Task by ID
        public async Task<TaskDTO> GetTaskByIdAsync(int id)
        {
            var task = await _taskContext.Tasks.FindAsync(id);

            if (task == null)
            {
                throw new KeyNotFoundException("Task not found.");
            }

            return new TaskDTO
            {
                Id = task.Id,
                TaskName = task.TaskName,
                TaskDescription = task.TaskDescription,
                TaskTime = task.TaskTime,
                TaskDate = task.TaskDate,
                Period = task.Period,
                Notification = task.Notification,
                UserId = task.UserId
            };
        }

        // Update Task
        public async Task<TaskDTO> UpdateTaskAsync(TaskDTO taskDto)
        {
            var task = await _taskContext.Tasks.FindAsync(taskDto.Id);

            if (task == null)
            {
                throw new KeyNotFoundException("Task not found.");
            }

            task.TaskName = taskDto.TaskName;
            task.TaskDescription = taskDto.TaskDescription;
            task.TaskTime = taskDto.TaskTime;
            task.TaskDate = taskDto.TaskDate;
            task.Period = taskDto.Period;
            task.Notification = taskDto.Notification;
            task.UserId = taskDto.UserId;

            _taskContext.Tasks.Update(task);
            await _taskContext.SaveChangesAsync();

            return taskDto;
        }


        // Delete Task
        public async Task DeleteTaskAsync(int id)
        {
            var task = await _taskContext.Tasks.FindAsync(id);

            if (task == null)
            {
                throw new KeyNotFoundException("Task not found.");
            }

            _taskContext.Tasks.Remove(task);
            await _taskContext.SaveChangesAsync();
        }

        public async Task<object> GetTasksByUserIdAsync(int userId)
        {
            // Retrieve tasks associated with the specified user ID
            var tasks = await _taskContext.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();

            // Check if there are no tasks for the given user ID
            if (!tasks.Any())
            {
                return "There are no tasks for this user ID.";
            }

            // Map tasks to TaskDTO and return the list
            var taskDTOs = tasks.Select(task => new TaskDTO
            {
                Id = task.Id,
                TaskName = task.TaskName,
                TaskDescription = task.TaskDescription,
                TaskTime = task.TaskTime,
                TaskDate = task.TaskDate,
                Period = task.Period,
                Notification = task.Notification,
                UserId = task.UserId
            }).ToList();

            return taskDTOs;
        }

    }
}
