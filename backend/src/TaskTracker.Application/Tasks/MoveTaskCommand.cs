using TaskTracker.Domain;

namespace TaskTracker.Application.Tasks;

public record MoveTaskCommand(int Id, Domain.TaskStatus Status, int Order) : IRequest<TaskEntity>;

public class MoveTaskCommandHandler : IRequestHandler<MoveTaskCommand, TaskEntity>
{
    private readonly ITaskRepository _repository;

    public MoveTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskEntity> Handle(MoveTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (task == null)
        {
            throw new KeyNotFoundException($"Task with id {request.Id} not found");
        }

        task.Status = (Domain.TaskStatus)request.Status;
        task.Order = request.Order;
        task.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateAsync(task, cancellationToken);
    }
}
