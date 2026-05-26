using Microsoft.Extensions.Logging;
using TaskTracker.Domain;

namespace TaskTracker.Application.Tasks;

public record CreateTaskCommand(
    string Title,
    string? Description,
    DateTime Date,
    Domain.TaskStatus Status,
    int Order,
    string? Assignee = null,
    string? Swimlane = null
) : IRequest<TaskEntity>;

public class CreateTaskCommandValidator : IValidator<CreateTaskCommand>
{
    public async Task<IEnumerable<string>> Validate(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            errors.Add("Title is required");
        }
        else if (request.Title.Length > 200)
        {
            errors.Add("Title must not exceed 200 characters");
        }

        if (request.Description != null && request.Description.Length > 2000)
        {
            errors.Add("Description must not exceed 2000 characters");
        }

        if (request.Assignee?.Length > 100)
        {
            errors.Add("Assignee must not exceed 100 characters");
        }

        if (request.Swimlane?.Length > 100)
        {
            errors.Add("Swimlane must be at most 100 characters long.");
        }

        return errors;
    }
}

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskEntity>
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<CreateTaskCommandHandler> _logger;

    public CreateTaskCommandHandler(ITaskRepository repository, ILogger<CreateTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TaskEntity> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new TaskEntity
        {
            Title = request.Title,
            Description = request.Description,
            Date = request.Date.Date,
            Status = request.Status,
            Order = request.Order,
            Assignee = string.IsNullOrWhiteSpace(request.Assignee) ? null : request.Assignee,
            Swimlane = string.IsNullOrWhiteSpace(request.Swimlane) ? null : request.Swimlane,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _repository.CreateAsync(task, cancellationToken);

        if (result.Assignee != null)
        {
            _logger.LogInformation("Task created with Id: {TaskId}, Assignee: {Assignee}, Title: {Title}",
                result.Id, result.Assignee, result.Title);
        }

        return result;
    }
}
