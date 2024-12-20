using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeSync.Core.DTO.BusinessDTO;
using TimeSync.Services;

namespace TimeSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _taskService;

        public TasksController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskDTO taskDto)
        {
            var result = await _taskService.CreateTaskAsync(taskDto);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                await _taskService.DeleteTaskAsync(id);
                return Ok("Task deleted successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Task not found.");
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateTask([FromBody] TaskDTO taskDto)
        {
            if (taskDto == null || taskDto.Id == 0) return BadRequest("Invalid task data.");
            try
            {
                var updatedTask = await _taskService.UpdateTaskAsync(taskDto);
                return Ok(updatedTask);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Task not found.");
            }
        }


        // Get Task by ID
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null) return NotFound("Task not found.");
            return Ok(task);
        }

        [HttpGet("GetByUserId/{id}")]
        public async Task<IActionResult> GetTaskByUserId(int id)
        {
            var task = await _taskService.GetTasksByUserIdAsync(id);
            if (task == null) return NotFound("Task not found.");
            return Ok(task);
        }
    }
}
