using Microsoft.Extensions.Logging;
using TaskTracker.Domain;

namespace TaskTracker.Application.Tasks;

public record PatchTaskCommand(
    int Id,
    string? Title = null,
    string? Description = null,
    Domain.TaskStatus? Status = null,
    DateTime? Date = null,
    int? Order = null,
    string? Assignee = null,
    string? Swimlane = null
) : IRequest;

public class PatchTaskCommandValidator : IValidator<PatchTaskCommand>
{
    public async Task<IEnumerable<string>> Validate(PatchTaskCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (request.Title is not null)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                errors.Add("Title is required");
            }
            else if (request.Title.Length > 200)
            {
                errors.Add("Title must not exceed 200 characters");
            }
        }

        if (request.Description is not null && request.Description.Length > 2000)
        {
            errors.Add("Description must not exceed 2000 characters");
        }

        if (request.Assignee is not null && request.Assignee.Length > 100)
        {
            errors.Add("Assignee must not exceed 100 characters");
        }

        if (request.Swimlane is not null && request.Swimlane.Length > 100)
        {
            errors.Add("Swimlane must not exceed 100 characters");
        }

        if (request.Status is not null && !Enum.IsDefined(typeof(Domain.TaskStatus), request.Status.Value))
        {
            errors.Add("Invalid status value");
        }

        return errors;
    }
}

public class PatchTaskCommandHandler : IRequestHandler<PatchTaskCommand>
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<PatchTaskCommandHandler> _logger;

    public PatchTaskCommandHandler(ITaskRepository repository, ILogger<PatchTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(PatchTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (task == null)
        {
            throw new KeyNotFoundException($"Task with id {request.Id} not found");
        }

        if (request.Title is not null)
        {
            task.Title = request.Title;
        }

        if (request.Description is not null)
        {
            task.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description;
        }

        if (request.Status is not null)
        {
            task.Status = request.Status.Value;
        }

        if (request.Date is not null)
        {
            task.Date = request.Date.Value.Date;
        }

        if (request.Order is not null)
        {
            task.Order = request.Order.Value;
        }

        if (request.Assignee is not null)
        {
            task.Assignee = string.IsNullOrWhiteSpace(request.Assignee) ? null : request.Assignee;
        }

        if (request.Swimlane is not null)
        {
            task.Swimlane = string.IsNullOrWhiteSpace(request.Swimlane) ? null : request.Swimlane;
        }

        task.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(task, cancellationToken);
    }
}