namespace TaskTracker.Application.Tasks;

public record BulkDeleteCommand(IList<int> TaskIds) : IRequest<BulkDeleteResponse>;

public record BulkDeleteResponse(int Deleted);

public class BulkDeleteCommandHandler : IRequestHandler<BulkDeleteCommand, BulkDeleteResponse>
{
    private readonly ITaskRepository _repository;

    public BulkDeleteCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<BulkDeleteResponse> Handle(BulkDeleteCommand request, CancellationToken cancellationToken)
    {
        if (request.TaskIds == null || request.TaskIds.Count == 0)
        {
            return new BulkDeleteResponse(0);
        }

        var deletedCount = 0;
        foreach (var taskId in request.TaskIds)
        {
            var task = await _repository.GetByIdAsync(taskId, cancellationToken);
            if (task != null)
            {
                await _repository.DeleteAsync(taskId, cancellationToken);
                deletedCount++;
            }
        }

        return new BulkDeleteResponse(deletedCount);
    }
}
