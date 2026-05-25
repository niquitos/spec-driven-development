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
    private readonly IRequestHandler<GetTaskByIdQuery, TaskEntity?> _getTaskByIdHandler;
    private readonly IRequestHandler<GetAssigneesQuery, string[]> _getAssigneesHandler;
    private readonly IRequestHandler<GetSwimlanesQuery, string[]> _getSwimlanesHandler;
    private readonly IRequestHandler<CreateTaskCommand, TaskEntity> _createHandler;
    private readonly IRequestHandler<UpdateTaskCommand> _updateHandler;
    private readonly IRequestHandler<DeleteTaskCommand> _deleteHandler;
    private readonly IRequestHandler<BulkDeleteCommand, BulkDeleteResponse> _bulkDeleteHandler;
    private readonly IRequestHandler<BulkMoveCommand, BulkMoveResponse> _bulkMoveHandler;
    private readonly IRequestHandler<MoveIncompleteToTomorrowCommand, MoveIncompleteToTomorrowResponse> _moveIncompleteToTomorrowHandler;
    private readonly IValidator<CreateTaskCommand> _validator;

    public TasksController(
        IRequestHandler<GetTasksQuery, IEnumerable<TaskEntity>> getTasksHandler,
        IRequestHandler<GetTaskByIdQuery, TaskEntity?> getTaskByIdHandler,
        IRequestHandler<GetAssigneesQuery, string[]> getAssigneesHandler,
        IRequestHandler<GetSwimlanesQuery, string[]> getSwimlanesHandler,
        IRequestHandler<CreateTaskCommand, TaskEntity> createHandler,
        IRequestHandler<UpdateTaskCommand> updateHandler,
        IRequestHandler<DeleteTaskCommand> deleteHandler,
        IRequestHandler<BulkDeleteCommand, BulkDeleteResponse> bulkDeleteHandler,
        IRequestHandler<BulkMoveCommand, BulkMoveResponse> bulkMoveHandler,
        IRequestHandler<MoveIncompleteToTomorrowCommand, MoveIncompleteToTomorrowResponse> moveIncompleteToTomorrowHandler,
        IValidator<CreateTaskCommand> validator)
    {
        _getTasksHandler = getTasksHandler;
        _getTaskByIdHandler = getTaskByIdHandler;
        _getAssigneesHandler = getAssigneesHandler;
        _getSwimlanesHandler = getSwimlanesHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _bulkDeleteHandler = bulkDeleteHandler;
        _bulkMoveHandler = bulkMoveHandler;
        _moveIncompleteToTomorrowHandler = moveIncompleteToTomorrowHandler;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskEntity>>> GetTasks(
        [FromQuery] DateTime date,
        [FromQuery] string? assignees = null,
        [FromQuery] string? swimlanes = null)
    {
        var assigneesList = !string.IsNullOrWhiteSpace(assignees)
            ? assignees.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            : null;
        var swimlanesList = !string.IsNullOrWhiteSpace(swimlanes)
            ? swimlanes.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            : null;
        var tasks = await _getTasksHandler.Handle(new GetTasksQuery(date, assigneesList, swimlanesList), CancellationToken.None);
        return Ok(tasks);
    }

    [HttpGet("assignees")]
    public async Task<ActionResult<string[]>> GetAssignees()
    {
        var assignees = await _getAssigneesHandler.Handle(new GetAssigneesQuery(), CancellationToken.None);
        return Ok(assignees);
    }

    [HttpGet("swimlanes")]
    public async Task<ActionResult<string[]>> GetSwimlanes([FromQuery] DateTime date)
    {
        if (date == default)
        {
            return BadRequest("Date parameter is required");
        }
        var swimlanes = await _getSwimlanesHandler.Handle(new GetSwimlanesQuery(date), CancellationToken.None);
        return Ok(swimlanes);
    }

    [HttpPost]
    public async Task<ActionResult<TaskEntity>> CreateTask([FromBody] CreateTaskRequest request)
    {
        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.Date,
            request.Status ?? Domain.TaskStatus.New,
            request.Order ?? 0,
            request.Assignee,
            request.Swimlane
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
        var task = await _getTaskByIdHandler.Handle(new GetTaskByIdQuery(id), CancellationToken.None);
        if (task == null)
        {
            return NotFound();
        }
        return Ok(task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskRequest request)
    {
        var command = new UpdateTaskCommand(
            id,
            request.Title,
            request.Description,
            request.Date,
            request.Status,
            request.Order,
            request.Assignee,
            request.Swimlane
        );

        await _updateHandler.Handle(command, CancellationToken.None);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        await _deleteHandler.Handle(new DeleteTaskCommand(id), CancellationToken.None);
        return NoContent();
    }

    [HttpPost("bulk/delete")]
    public async Task<ActionResult<BulkDeleteResponse>> BulkDelete([FromBody] BulkDeleteRequest request)
    {
        var command = new BulkDeleteCommand(request.TaskIds);
        var result = await _bulkDeleteHandler.Handle(command, CancellationToken.None);
        return Ok(result);
    }

    [HttpPost("bulk/move")]
    public async Task<ActionResult<BulkMoveResponse>> BulkMove([FromBody] BulkMoveRequest request)
    {
        var command = new BulkMoveCommand(request.TaskIds, request.TargetDate);
        var result = await _bulkMoveHandler.Handle(command, CancellationToken.None);
        return Ok(result);
    }

    [HttpPost("bulk/move-incomplete-to-tomorrow")]
    public async Task<ActionResult<MoveIncompleteToTomorrowResponse>> MoveIncompleteToTomorrow()
    {
        var command = new MoveIncompleteToTomorrowCommand();
        var result = await _moveIncompleteToTomorrowHandler.Handle(command, CancellationToken.None);
        return Ok(result);
    }
}

public record CreateTaskRequest(
    string Title,
    string? Description,
    DateTime Date,
    Domain.TaskStatus? Status,
    int? Order,
    string? Assignee,
    string? Swimlane
);

public record UpdateTaskRequest(
    string Title,
    string? Description,
    DateTime Date,
    Domain.TaskStatus Status,
    int Order,
    string? Assignee,
    string? Swimlane
);

public record BulkDeleteRequest(
    IList<int> TaskIds
);

public record BulkMoveRequest(
    IList<int> TaskIds,
    DateTime TargetDate
);