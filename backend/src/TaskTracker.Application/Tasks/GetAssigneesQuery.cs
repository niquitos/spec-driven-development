using Microsoft.Extensions.Logging;

namespace TaskTracker.Application.Tasks;

public record GetAssigneesQuery(DateTime Date) : IRequest<string[]>;

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
        var assignees = await _repository.GetAssigneesAsync(request.Date, cancellationToken);
        _logger.LogDebug("Found {Count} unique assignees for date {Date}", assignees.Length, request.Date.ToString("yyyy-MM-dd"));
        return assignees;
    }
}
