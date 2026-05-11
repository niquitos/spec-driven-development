using TaskTracker.Domain;

namespace TaskTracker.Application.Tasks;

public record UpdateTaskCommand(
    int Id,
    string Title,
    string? Description,
    DateTime Date,
    Domain.TaskStatus Status,
    int Order
) : IRequest<TaskEntity>;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskEntity>
{
    private readonly ITaskRepository _repository;

    public UpdateTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskEntity> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (task == null)
        {
            throw new KeyNotFoundException($"Task with id {request.Id} not found");
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.Date = request.Date.ToUniversalTime();
        task.Status = request.Status;
        task.Order = request.Order;
        task.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateAsync(task, cancellationToken);
    }
}
