using Microsoft.Extensions.Logging;
using TaskTracker.Domain;

namespace TaskTracker.Application.Tasks;

public record GetSwimlanesQuery(DateTime Date) : IRequest<string[]>;

public class GetSwimlanesQueryValidator : IValidator<GetSwimlanesQuery>
{
    public async Task<IEnumerable<string>> Validate(GetSwimlanesQuery request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        if (request.Date == default)
            errors.Add("Date is required");
        return errors;
    }
}

public class GetSwimlanesQueryHandler : IRequestHandler<GetSwimlanesQuery, string[]>
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<GetSwimlanesQueryHandler> _logger;

    public GetSwimlanesQueryHandler(ITaskRepository repository, ILogger<GetSwimlanesQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<string[]> Handle(GetSwimlanesQuery request, CancellationToken cancellationToken)
    {
        var swimlanes = await _repository.GetSwimlanesAsync(request.Date, cancellationToken);
        _logger.LogDebug("Found {Count} unique swimlanes for date {Date:yyyy-MM-dd}", swimlanes.Length, request.Date);
        return swimlanes;
    }
}