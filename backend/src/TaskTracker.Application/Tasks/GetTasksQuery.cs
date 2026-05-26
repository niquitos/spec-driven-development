using TaskTracker.Domain;

namespace TaskTracker.Application.Tasks;

public record GetTasksQuery(DateTime Date, string[]? Assignees = null, string[]? Swimlanes = null) : IRequest<IEnumerable<TaskEntity>>;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, IEnumerable<TaskEntity>>
{
    private readonly ITaskRepository _repository;

    public GetTasksQueryHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TaskEntity>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByDateAsync(request.Date, request.Assignees, request.Swimlanes, cancellationToken);
    }
}
