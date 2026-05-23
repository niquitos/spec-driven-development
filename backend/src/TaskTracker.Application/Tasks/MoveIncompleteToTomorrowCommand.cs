using Microsoft.Extensions.Logging;

namespace TaskTracker.Application.Tasks;

public record MoveIncompleteToTomorrowCommand : IRequest<MoveIncompleteToTomorrowResponse>;

public record MoveIncompleteToTomorrowResponse(int Moved, DateTime TargetDate);

public class MoveIncompleteToTomorrowCommandHandler : IRequestHandler<MoveIncompleteToTomorrowCommand, MoveIncompleteToTomorrowResponse>
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<MoveIncompleteToTomorrowCommandHandler> _logger;

    public MoveIncompleteToTomorrowCommandHandler(
        ITaskRepository repository,
        ILogger<MoveIncompleteToTomorrowCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<MoveIncompleteToTomorrowResponse> Handle(MoveIncompleteToTomorrowCommand request, CancellationToken cancellationToken)
    {
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var moved = await _repository.MoveIncompleteToTomorrowAsync(tomorrow, cancellationToken);

        _logger.LogInformation("Moved {Count} incomplete tasks to {TargetDate}", moved, tomorrow);

        return new MoveIncompleteToTomorrowResponse(moved, tomorrow);
    }
}