using TaskTracker.Domain;

namespace TaskTracker.Application.Tasks;

public record UpdateTaskCommand(
    int Id,
    string? Title,
    string? Description,
    DateTime? Date,
    Domain.TaskStatus? Status
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

        if (request.Title != null) task.Title = request.Title;
        if (request.Description != null) task.Description = request.Description;
        if (request.Date.HasValue) task.Date = request.Date.Value;
        if (request.Status.HasValue) task.Status = (Domain.TaskStatus)request.Status.Value;

        task.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateAsync(task, cancellationToken);
    }
}
