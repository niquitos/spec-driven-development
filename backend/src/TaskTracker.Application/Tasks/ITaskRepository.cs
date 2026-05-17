using TaskTracker.Domain;

namespace TaskTracker.Application.Tasks;

public interface ITaskRepository
{
    Task<IEnumerable<TaskEntity>> GetByDateAsync(DateTime date, string[]? assignees, CancellationToken cancellationToken);
    Task<TaskEntity?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<TaskEntity> CreateAsync(TaskEntity task, CancellationToken cancellationToken);
    Task<TaskEntity> UpdateAsync(TaskEntity task, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<TaskEntity>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
    Task<string[]> GetAssigneesAsync(CancellationToken ct);
}
