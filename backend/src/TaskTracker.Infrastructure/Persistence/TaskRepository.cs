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

    public async Task<IEnumerable<TaskEntity>> GetByDateAsync(DateTime date, CancellationToken cancellationToken)
    {
        var dateOnly = DateOnly.FromDateTime(date);
        return await _context.Tasks
            .Where(t => t.Date.Date == date)
            .OrderBy(t => t.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskEntity?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Tasks.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<TaskEntity> CreateAsync(TaskEntity task, CancellationToken cancellationToken)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);
        return task;
    }

    public async Task<TaskEntity> UpdateAsync(TaskEntity task, CancellationToken cancellationToken)
    {
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
}
