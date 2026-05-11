using TaskTracker.Domain;

namespace TaskTracker.Application.Tasks;

public record UpdateTaskCommand(
    int Id,
    string Title,
    string? Description,
    DateTime Date,
    Domain.TaskStatus Status,
    int Order
) : IRequest;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
{
    private readonly ITaskRepository _repository;

    public UpdateTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (task == null)
        {
            throw new KeyNotFoundException($"Task with id {request.Id} not found");
        }

        // Если статус или порядок изменились - пересчитываем order для всех задач в этой колонке
        if (task.Status != request.Status || task.Order != request.Order)
        {
            await RecalculateOrders(request.Id, request.Status, request.Date.ToUniversalTime(), request.Order, cancellationToken);
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.Date = request.Date.ToUniversalTime();
        task.Status = request.Status;
        task.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(task, cancellationToken);
    }

    private async Task RecalculateOrders(int taskId, Domain.TaskStatus status, DateTime date, int newOrder, CancellationToken cancellationToken)
    {
        // Получаем все задачи по этой дате
        var allTasks = await _repository.GetByDateAsync(date, cancellationToken);
        var tasksInColumn = allTasks.Where(t => t.Status == status).ToList();

        // Находим перемещаемую задачу
        var movedTask = tasksInColumn.FirstOrDefault(t => t.Id == taskId);

        // Если задача есть в этой колонке - удаляем её для последующей вставки
        if (movedTask != null)
        {
            tasksInColumn.RemoveAll(t => t.Id == taskId);
        }
        else
        {
            // Задача пришла из другой колонки - получаем её
            movedTask = await _repository.GetByIdAsync(taskId, cancellationToken);
            if (movedTask == null) return;
        }

        // Сортируем оставшиеся задачи по order
        tasksInColumn = tasksInColumn.OrderBy(t => t.Order).ToList();

        // Вставляем задачу на новую позицию
        var insertIndex = Math.Min(newOrder, tasksInColumn.Count);
        tasksInColumn.Insert(insertIndex, movedTask);

        // Обновляем order для всех задач
        for (int i = 0; i < tasksInColumn.Count; i++)
        {
            var t = await _repository.GetByIdAsync(tasksInColumn[i].Id, cancellationToken);
            if (t != null)
            {
                t.Order = i;
                t.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(t, cancellationToken);
            }
        }
    }
}
