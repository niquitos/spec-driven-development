using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application;
using TaskTracker.Application.Tasks;
using TaskTracker.Domain;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IRequestHandler<GetTasksQuery, IEnumerable<TaskEntity>> _getTasksHandler;
    private readonly IRequestHandler<CreateTaskCommand, TaskEntity> _createHandler;
    private readonly IRequestHandler<UpdateTaskCommand, TaskEntity> _updateHandler;
    private readonly IRequestHandler<DeleteTaskCommand> _deleteHandler;
    private readonly IRequestHandler<MoveTaskCommand, TaskEntity> _moveHandler;
    private readonly IValidator<CreateTaskCommand> _validator;

    public TasksController(
        IRequestHandler<GetTasksQuery, IEnumerable<TaskEntity>> getTasksHandler,
        IRequestHandler<CreateTaskCommand, TaskEntity> createHandler,
        IRequestHandler<UpdateTaskCommand, TaskEntity> updateHandler,
        IRequestHandler<DeleteTaskCommand> deleteHandler,
        IRequestHandler<MoveTaskCommand, TaskEntity> moveHandler,
        IValidator<CreateTaskCommand> validator)
    {
        _getTasksHandler = getTasksHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _moveHandler = moveHandler;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskEntity>>> GetTasks([FromQuery] DateTime date)
    {
        var tasks = await _getTasksHandler.Handle(new GetTasksQuery(date), CancellationToken.None);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<ActionResult<TaskEntity>> CreateTask([FromBody] CreateTaskRequest request)
    {
        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.Date,
            request.Status ?? Domain.TaskStatus.New,
            request.Order ?? 0
        );

        var validationErrors = await _validator.Validate(command, CancellationToken.None);
        if (validationErrors.Any())
        {
            return BadRequest(validationErrors);
        }

        var task = await _createHandler.Handle(command, CancellationToken.None);
        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskEntity>> GetTaskById(int id)
    {
        var task = await _getTasksHandler.Handle(new GetTasksQuery(DateTime.Today), CancellationToken.None);
        var foundTask = task.FirstOrDefault(t => t.Id == id);
        if (foundTask == null)
        {
            return NotFound();
        }
        return Ok(foundTask);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskEntity>> UpdateTask(int id, [FromBody] UpdateTaskRequest request)
    {
        var command = new UpdateTaskCommand(
            id,
            request.Title,
            request.Description,
            request.Date,
            request.Status
        );

        var task = await _updateHandler.Handle(command, CancellationToken.None);
        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        await _deleteHandler.Handle(new DeleteTaskCommand(id), CancellationToken.None);
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<TaskEntity>> MoveTask(int id, [FromBody] MoveTaskRequest request)
    {
        var command = new MoveTaskCommand(id, request.Status, request.Order);
        var task = await _moveHandler.Handle(command, CancellationToken.None);
        return Ok(task);
    }
}

public record CreateTaskRequest(
    string Title,
    string? Description,
    DateTime Date,
    Domain.TaskStatus? Status,
    int? Order
);

public record UpdateTaskRequest(
    string? Title,
    string? Description,
    DateTime? Date,
    Domain.TaskStatus? Status
);

public record MoveTaskRequest(
    Domain.TaskStatus Status,
    int Order
);
