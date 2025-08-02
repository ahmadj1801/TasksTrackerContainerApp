using Microsoft.AspNetCore.Mvc;
using TasksTracker.TasksManager.Backend.Api.Models;
using TasksTracker.TasksManager.Backend.Api.Services;

namespace TasksTracker.TasksManager.Backend.Api.Controllers
{
    [Route("api/task")]
    [ApiController]
    public class TaskController: ControllerBase
    {
        private readonly ILogger<TaskController> _logger;
        private readonly ITaskManager _taskManager;

        public TaskController(ILogger<TaskController> logger, ITaskManager tasksManager)
        {
            _logger = logger;
            _taskManager = tasksManager;
        }

        [HttpGet]
        public async Task<IEnumerable<TaskModel>> Get(string createdBy)
        {
            return await _taskManager.GetTasksByCreator(createdBy);
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTask(Guid taskId)
        {
            var task = await _taskManager.GetTaskById(taskId);

            return (task != null) ? Ok(task) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TaskAddModel taskAddModel)
        {
            var taskId = await _taskManager.CreateNewTask(
                taskAddModel.TaskName,
                taskAddModel.TaskCreatedBy,
                taskAddModel.TaskAssignedTo,
                taskAddModel.TaskDueDate
            );

            return Created($"/api/tasks/{taskId}", null);

        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> Put(Guid taskId, [FromBody] TaskUpdateModel taskUpdateModel)
        {
            var updated = await _taskManager.UpdateTask(
                taskId,
                taskUpdateModel.TaskName,
                taskUpdateModel.TaskAssignedTo,
                taskUpdateModel.TaskDueDate
            );

            return updated ? Ok() : BadRequest();
        }

        [HttpPut("{taskId}/markcomplete")]
        public async Task<IActionResult> MarkComplete(Guid taskId)
        {
            var updated = await _taskManager.MarkTaskCompleted(taskId);

            return updated ? Ok() : BadRequest();
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> Delete(Guid taskId)
        {
            var deleted = await _taskManager.DeleteTask(taskId);

            return deleted ? Ok() : NotFound();
        }
    }
}
