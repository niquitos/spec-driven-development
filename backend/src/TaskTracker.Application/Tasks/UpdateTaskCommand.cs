using Microsoft.Extensions.Logging;
using TaskTracker.Domain;

namespace TaskTracker.Application.Tasks;

public record UpdateTaskCommand(
    int Id,
    string Title,
    string? Description,
    DateTime Date,
    Domain.TaskStatus Status,
    int Order,
    string? Assignee = null
) : IRequest;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<UpdateTaskCommandHandler> _logger;

    public UpdateTaskCommandHandler(ITaskRepository repository, ILogger<UpdateTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (task == null)
        {
            throw new KeyNotFoundException($"Task with id {request.Id} not found");
        }

        var oldAssignee = task.Assignee;
        var newAssignee = string.IsNullOrWhiteSpace(request.Assignee) ? null : request.Assignee;

        // Если статус или порядок изменились - пересчитываем order для всех задач в этой колонке
        if (task.Status != request.Status || task.Order != request.Order)
        {
            await RecalculateOrders(request.Id, request.Status, request.Date.Date, request.Order, cancellationToken);
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.Date = request.Date.Date;
        task.Status = request.Status;
        task.Assignee = newAssignee;
        task.UpdatedAt = DateTime.UtcNow;

        if (oldAssignee != newAssignee)
        {
            _logger.LogInformation(
                "Task {TaskId} assignee changed: \"{OldAssignee}\" -> \"{NewAssignee}\"",
                task.Id, oldAssignee ?? "(none)", newAssignee ?? "(none)");
        }

        await _repository.UpdateAsync(task, cancellationToken);
    }

    private async Task RecalculateOrders(int taskId, Domain.TaskStatus status, DateTime date, int newOrder, CancellationToken cancellationToken)
    {
        // Получаем все задачи по этой дате
        var allTasks = await _repository.GetByDateAsync(date, null, cancellationToken);
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
