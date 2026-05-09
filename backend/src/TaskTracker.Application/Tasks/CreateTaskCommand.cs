using TaskTracker.Domain;

namespace TaskTracker.Application.Tasks;

public record CreateTaskCommand(
    string Title,
    string? Description,
    DateTime Date,
    Domain.TaskStatus Status,
    int Order
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

        return errors;
    }
}

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskEntity>
{
    private readonly ITaskRepository _repository;

    public CreateTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskEntity> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new TaskEntity
        {
            Title = request.Title,
            Description = request.Description,
            Date = request.Date,
            Status = (Domain.TaskStatus)request.Status,
            Order = request.Order,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _repository.CreateAsync(task, cancellationToken);
    }
}
