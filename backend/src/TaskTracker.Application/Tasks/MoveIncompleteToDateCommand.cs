using Microsoft.Extensions.Logging;

namespace TaskTracker.Application.Tasks;

public record MoveIncompleteToDateCommand(DateTime TargetDate) : IRequest<MoveIncompleteToDateResponse>;

public record MoveIncompleteToDateResponse(int Moved, DateTime TargetDate);

public class MoveIncompleteToDateCommandHandler : IRequestHandler<MoveIncompleteToDateCommand, MoveIncompleteToDateResponse>
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<MoveIncompleteToDateCommandHandler> _logger;

    public MoveIncompleteToDateCommandHandler(
        ITaskRepository repository,
        ILogger<MoveIncompleteToDateCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<MoveIncompleteToDateResponse> Handle(MoveIncompleteToDateCommand request, CancellationToken cancellationToken)
    {
        var moved = await _repository.MoveIncompleteToDateAsync(request.TargetDate, cancellationToken);

        _logger.LogInformation("Moved {Count} incomplete tasks to {TargetDate}", moved, request.TargetDate);

        return new MoveIncompleteToDateResponse(moved, request.TargetDate);
    }
}
