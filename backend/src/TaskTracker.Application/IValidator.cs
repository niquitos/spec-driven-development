namespace TaskTracker.Application;

public interface IValidator<in TRequest>
{
    Task<IEnumerable<string>> Validate(TRequest request, CancellationToken cancellationToken);
}
