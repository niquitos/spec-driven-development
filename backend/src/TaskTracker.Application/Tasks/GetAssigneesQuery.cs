using Microsoft.Extensions.Logging;

namespace TaskTracker.Application.Tasks;

public record GetAssigneesQuery() : IRequest<string[]>;

public class GetAssigneesQueryHandler : IRequestHandler<GetAssigneesQuery, string[]>
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<GetAssigneesQueryHandler> _logger;

    public GetAssigneesQueryHandler(ITaskRepository repository, ILogger<GetAssigneesQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<string[]> Handle(GetAssigneesQuery request, CancellationToken cancellationToken)
    {
        var assignees = await _repository.GetAssigneesAsync(cancellationToken);
        _logger.LogDebug("Found {Count} unique assignees", assignees.Length);
        return assignees;
    }
}
