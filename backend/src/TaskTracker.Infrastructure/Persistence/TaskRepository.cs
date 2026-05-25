using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Tasks;
using TaskTracker.Domain;

namespace TaskTracker.Infrastructure.Persistence;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskEntity>> GetByDateAsync(DateTime date, string[]? assignees, string[]? swimlanes, CancellationToken cancellationToken)
    {
        var utcDate = date.Kind == DateTimeKind.Unspecified
       ? DateTime.SpecifyKind(date, DateTimeKind.Utc)
       : date.ToUniversalTime();

        var query = _context.Tasks
            .Where(t => t.Date == utcDate.Date);

        if (assignees is { Length: > 0 })
        {
            query = query.Where(t => t.Assignee != null && assignees.Contains(t.Assignee));
        }

        if (swimlanes is { Length: > 0 })
        {
            var normalizedSwimlanes = swimlanes.Select(s => s.ToLower()).ToArray();
            var hasUncategorized = normalizedSwimlanes.Contains("без категории");

            query = query.Where(t =>
                (t.Swimlane != null && normalizedSwimlanes.Contains(t.Swimlane.ToLower()))
                || (t.Swimlane == null && hasUncategorized));
        }

        return await query
            .OrderBy(t => t.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskEntity?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Tasks.FindAsync([id], cancellationToken);
    }

    public async Task<TaskEntity> CreateAsync(TaskEntity task, CancellationToken cancellationToken)
    {
        if (task.Date.Kind != DateTimeKind.Utc)
        {
            task.Date = DateTime.SpecifyKind(task.Date, DateTimeKind.Utc);
        }
        if (task.CreatedAt.Kind != DateTimeKind.Utc)
        {
            task.CreatedAt = DateTime.SpecifyKind(task.CreatedAt, DateTimeKind.Utc);
        }

        if (task.UpdatedAt.Kind != DateTimeKind.Utc)
        {
            task.UpdatedAt = DateTime.SpecifyKind(task.UpdatedAt, DateTimeKind.Utc);
        }

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);
        return task;
    }

    public async Task<TaskEntity> UpdateAsync(TaskEntity task, CancellationToken cancellationToken)
    {
        if (task.Date.Kind != DateTimeKind.Utc)
        {
            task.Date = DateTime.SpecifyKind(task.Date, DateTimeKind.Utc);
        }
        if (task.CreatedAt.Kind != DateTimeKind.Utc)
        {
            task.CreatedAt = DateTime.SpecifyKind(task.CreatedAt, DateTimeKind.Utc);
        }

        if (task.UpdatedAt.Kind != DateTimeKind.Utc)
        {
            task.UpdatedAt = DateTime.SpecifyKind(task.UpdatedAt, DateTimeKind.Utc);
        }

        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(cancellationToken);
        return task;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var task = await GetByIdAsync(id, cancellationToken);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<TaskEntity>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        return await _context.Tasks
            .Where(t => ids.Contains(t.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<string[]> GetAssigneesAsync(CancellationToken ct)
    {
        return await _context.Tasks
            .Where(t => t.Assignee != null)
            .Select(t => t.Assignee!)
            .Distinct()
            .OrderBy(a => a)
            .ToArrayAsync(ct);
    }

    public async Task<string[]> GetSwimlanesAsync(DateTime date, CancellationToken ct)
    {
        var utcDate = date.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(date, DateTimeKind.Utc)
            : date.ToUniversalTime();

        return await _context.Tasks
            .Where(t => t.Date == utcDate.Date && t.Swimlane != null)
            .GroupBy(t => t.Swimlane!.ToLower())
            .Select(g => g.First().Swimlane!)
            .OrderBy(s => s.ToLower() == "без категории" ? "" : s.ToLower())
            .ToArrayAsync(ct);
    }

    public async Task<int> MoveIncompleteToTomorrowAsync(DateTime tomorrow, CancellationToken cancellationToken)
    {
        var utcTomorrow = tomorrow.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(tomorrow, DateTimeKind.Utc)
            : tomorrow.ToUniversalTime();

        var result = await _context.Tasks
            .Where(t => t.Status != Domain.TaskStatus.Done)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.Date, utcTomorrow.Date)
                .SetProperty(t => t.UpdatedAt, DateTime.UtcNow),
                cancellationToken);

        return result;
    }
}
