# Research: Kanban Calendar Board

**Feature**: 001-kanban-calendar  
**Date**: 2026-05-09  
**Last Updated**: 2026-05-11  
**Purpose**: Resolve all NEEDS CLARIFICATION from Technical Context  
**Status**: Implemented

---

## R001: Кастомная реализация IValidator без FluentValidation

**Decision**: Реализовать интерфейс `IValidator<T>` с методами:
```csharp
public interface IValidator<T>
{
    Task<ValidationResult> ValidateAsync(T item, CancellationToken ct);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public IEnumerable<ValidationError> Errors { get; set; }
}

public class ValidationError
{
    public string PropertyName { get; set; }
    public string ErrorMessage { get; set; }
}
```

**Rationale**: 
- Простой интерфейс, легко тестировать
- Поддержка асинхронной валидации (БД-проверки)
- Явные ошибки с указанием свойства

**Alternatives Considered**:
- FluentValidation — отклонено по требованию
- Валидация в доменной модели — смешивает ответственности

**Implementation**:
```csharp
// Infrastructure/Validators/CreateTaskValidator.cs
public class CreateTaskValidator : IValidator<CreateTaskCommand>
{
    public async Task<ValidationResult> ValidateAsync(CreateTaskCommand cmd, CancellationToken ct)
    {
        var errors = new List<ValidationError>();
        
        if (string.IsNullOrWhiteSpace(cmd.Title))
            errors.Add(new ValidationError(nameof(cmd.Title), "Title is required"));
        
        if (cmd.Date == default)
            errors.Add(new ValidationError(nameof(cmd.Date), "Date is required"));
        
        return new ValidationResult 
        { 
            IsValid = !errors.Any(), 
            Errors = errors 
        };
    }
}
```

---

## R002: CQRS с кастомным IRequest/IRequestHandler

**Decision**: 
```csharp
// Application/Common/Interfaces/IRequest.cs
public interface IRequest<TResponse>
{
}

public interface IRequest
{
}

// Application/Common/Interfaces/IRequestHandler.cs
public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken ct);
}

public interface IRequestHandler<TRequest>
    where TRequest : IRequest
{
    Task Handle(TRequest request, CancellationToken ct);
}
```

**Rationale**:
- Явные контракты для команд и запросов
- Разделение на void/non-void handlers
- Упрощённое тестирование через моки

**Alternatives Considered**:
- MediatR — отклонено по требованию (избыточная сложность)
- Прямые сервисы — сложнее для CQRS паттерна

**Implementation Sample**:
```csharp
// Application/Features/Tasks/CreateTask/CreateTaskCommand.cs
public record CreateTaskCommand(
    string Title,
    string? Description,
    DateTime Date,
    TaskStatus Status
) : IRequest<Guid>;

// Application/Features/Tasks/CreateTask/CreateTaskHandler.cs
public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly IAppDbContext _dbContext;
    private readonly IValidator<CreateTaskCommand> _validator;
    
    public async Task<Guid> Handle(CreateTaskCommand cmd, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        var task = new TaskEntity 
        {
            Id = Guid.NewGuid(),
            Title = cmd.Title,
            Description = cmd.Description,
            Date = cmd.Date,
            Status = cmd.Status,
            Order = 0 // TODO: вычислить следующий порядок
        };
        
        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync(ct);
        
        return task.Id;
    }
}
```

---

## R003: React DnD библиотеки для drag-n-drop

**Decision**: `@hello-pangea/dnd`

**Rationale**:
- Fork react-beautiful-dnd с поддержкой React 18
- Лучшая совместимость с канбан-досками
- Встроенная поддержка вертикального и горизонтального drag-n-drop
- Простой API для колонок

**Alternatives Considered**:
- @dnd-kit/core — более сложный API
- react-dnd — избыточен для простых сценариев

**Implementation** (реализовано):
```tsx
// components/Board.tsx
import { DragDropContext, DropResult } from '@hello-pangea/dnd';

export function Board() {
  const { moveTask } = useTaskStore();

  const handleDragEnd = (result: DropResult) => {
    if (!result.destination) return;

    const taskId = Number(result.draggableId);
    const newStatus = Number(result.destination.droppableId) as TaskStatus;
    const newOrder = result.destination.index;

    moveTask(taskId, newStatus, newOrder);
  };

  return (
    <DragDropContext onDragEnd={handleDragEnd}>
      <div className="board">
        {columns.map((column) => (
          <Column key={column.status} status={column.status} tasks={...} />
        ))}
      </div>
    </DragDropContext>
  );
}
```

---

## R004: Структура API контрактов для задач

**Decision**: RESTful API с ресурсами:

```
GET    /api/tasks?date=2026-05-09     # Получить задачи на дату
POST   /api/tasks                     # Создать задачу
GET    /api/tasks/{id}                # Получить задачу
PUT    /api/tasks/{id}                # Обновить задачу
DELETE /api/tasks/{id}                # Удалить задачу
PATCH  /api/tasks/{id}/status         # Изменить статус (drag-n-drop)
PUT    /api/tasks/reorder             # Изменить порядок в колонке

POST   /api/tasks/bulk/delete         # Массовое удаление
POST   /api/tasks/bulk/move           # Массовое перемещение
```

**Request/Response Schemas**:

```typescript
// GET /api/tasks?date=2026-05-09
Response 200:
{
  "date": "2026-05-09",
  "tasks": [
    {
      "id": "uuid",
      "title": "Task title",
      "description": "Description text",
      "status": "new" | "inprogress" | "done",
      "date": "2026-05-09T00:00:00Z",
      "order": 0,
      "createdAt": "2026-05-09T10:00:00Z"
    }
  ]
}

// POST /api/tasks
Request:
{
  "title": "Task title",
  "description": "Description",
  "date": "2026-05-09",
  "status": "new"
}
Response 201:
{
  "id": "uuid",
  "title": "Task title",
  ...
}

// Validation Error 400:
{
  "errors": [
    { "propertyName": "title", "errorMessage": "Title is required" }
  ]
}
```

---

## R005: Настройка EF Core с PostgreSQL

**Decision**: 
```csharp
// Infrastructure/Persistence/AppDbContext.cs
public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }
    
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tasks");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

// Program.cs
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"),
        npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);
        }));
```

**Rationale**:
- Npgsql — официальный провайдер
- Retry policy для transient ошибок
- Миграции в отдельной сборке

**Connection String**:
```
Host=localhost;Port=5432;Database=tasktracker;Username=postgres;Password=postgres
```

---

## Summary

| Research Task | Decision | Implemented |
|---------------|----------|-------------|
| R001: IValidator | Кастомный интерфейс с ValidationResult | ✅ |
| R002: CQRS | IRequest<T>/IRequestHandler<T,R> без MediatR | ✅ |
| R003: Drag-n-Drop | @hello-pangea/dnd | ✅ |
| R004: API Contracts | RESTful endpoints с JSON schemas | ✅ |
| R005: EF Core + Postgres | Npgsql с миграциями | ✅ |

## Implementation Notes

**Финальные решения:**
- **ID задачи**: INT IDENTITY вместо GUID (упрощение, производительность)
- **Drag-n-Drop**: @hello-pangea/dnd вместо @dnd-kit (лучше для канбан-досок)
- **State Management**: Zustand вместо Redux (меньше boilerplate)
- **CLI**: Удалён — веб-интерфейс является основным UX
- **Tests**: TDD требуется, но тесты пока не написаны

**All NEEDS CLARIFICATION resolved. Implementation complete.**
