namespace TaskTracker.Application.Tasks;

public record BulkMoveCommand(IList<int> TaskIds, DateTime TargetDate) : IRequest<BulkMoveResponse>;

public record BulkMoveResponse(int Moved, DateTime TargetDate);

public class BulkMoveCommandHandler : IRequestHandler<BulkMoveCommand, BulkMoveResponse>
{
    private readonly ITaskRepository _repository;

    public BulkMoveCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<BulkMoveResponse> Handle(BulkMoveCommand request, CancellationToken cancellationToken)
    {
        if (request.TaskIds == null || request.TaskIds.Count == 0)
        {
            return new BulkMoveResponse(0, request.TargetDate);
        }

        var movedCount = 0;
        foreach (var taskId in request.TaskIds)
        {
            var task = await _repository.GetByIdAsync(taskId, cancellationToken);
            if (task != null)
            {
                task.Date = request.TargetDate;
                task.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(task, cancellationToken);
                movedCount++;
            }
        }

        return new BulkMoveResponse(movedCount, request.TargetDate);
    }
}
