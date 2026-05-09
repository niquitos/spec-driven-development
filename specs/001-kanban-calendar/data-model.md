# Data Model: Kanban Calendar Board

**Feature**: 001-kanban-calendar  
**Date**: 2026-05-09  
**Source**: [spec.md](spec.md), [research.md](research.md)

---

## Core Entities

### TaskEntity

Представляет задачу на канбан-доске.

```csharp
public class TaskEntity
{
    public Guid Id { get; set; }           // Primary key
    public string Title { get; set; }      // NVARCHAR(200), NOT NULL
    public string? Description { get; set; } // NVARCHAR(MAX), NULL
    public TaskStatus Status { get; set; } // INT, NOT NULL (0=new, 1=inprogress, 2=done)
    public DateTime Date { get; set; }     // DATE, NOT NULL
    public int Order { get; set; }         // INT, NOT NULL (порядок в колонке)
    public DateTime CreatedAt { get; set; } // DATETIME2, NOT NULL
    public DateTime? UpdatedAt { get; set; } // DATETIME2, NULL
}
```

**Fields**:
- `Id`: Уникальный идентификатор (GUID)
- `Title`: Название задачи (1-200 символов)
- `Description`: Описание (опционально)
- `Status`: Статус задачи
- `Date`: Дата задачи (для какой колонки отображается)
- `Order`: Позиция в колонке (для сортировки)
- `CreatedAt`: Дата создания
- `UpdatedAt`: Дата последнего изменения

### TaskStatus (Enum)

```csharp
public enum TaskStatus
{
    New = 0,        // "Новые"
    InProgress = 1, // "В процессе"
    Done = 2        // "Сделаны"
}
```

---

## Value Objects

### DateRange

Для навигации по датам.

```csharp
public readonly record struct DateRange
{
    public DateTime Start { get; }
    public DateTime End { get; }
    
    public DateRange(DateTime date) 
    {
        Start = date.Date;
        End = date.Date.AddDays(1).AddTicks(-1);
    }
    
    public DateRange Previous() => new(Start.AddDays(-1));
    public DateRange Next() => new(Start.AddDays(1));
}
```

---

## Database Schema

```sql
-- Schema: tasks
CREATE SCHEMA tasks;

-- Table: Tasks
CREATE TABLE tasks.Tasks (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Status INT NOT NULL CONSTRAINT DF_Tasks_Status DEFAULT 0,
    [Date] DATE NOT NULL,
    [Order] INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Tasks_CreatedAt DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,
    
    INDEX IX_Tasks_Date_Status_Order ([Date], Status, [Order])
);
```

**Indexes**:
- `IX_Tasks_Date_Status_Order`: Composite index для быстрого поиска задач на дату с сортировкой

---

## EF Core Configuration

```csharp
// Infrastructure/Persistence/Configurations/TaskEntityConfiguration.cs
public class TaskEntityConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.ToTable("Tasks", "tasks");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(t => t.Description)
            .HasMaxLength(4000);
        
        builder.Property(t => t.Status)
            .HasConversion<int>()
            .IsRequired();
        
        builder.Property(t => t.Date)
            .HasColumnType("date")
            .IsRequired();
        
        builder.Property(t => t.Order)
            .IsRequired();
        
        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");
        
        builder.HasIndex(t => new { t.Date, t.Status, t.Order });
    }
}
```

---

## State Transitions

```
Task Lifecycle:

[Created] → New (Status = 0)
    ↓
[Drag to "В процессе"] → InProgress (Status = 1)
    ↓
[Drag to "Сделаны"] → Done (Status = 2)

[Any Status] → [Deleted] (Hard delete)
```

**Invariants**:
- Задача всегда имеет валидный статус (0, 1, или 2)
- Order уникален в пределах (Date, Status)
- Title не может быть пустым
- Date не может быть default(DateTime)

---

## Query Models

### TasksByDateResult

```csharp
public record TasksByDateResult(
    DateTime Date,
    IReadOnlyList<TaskDto> Tasks
);

public record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    TaskStatus Status,
    DateTime Date,
    int Order,
    DateTime CreatedAt
);
```

---

## Validation Rules

| Field | Rule | Error Message |
|-------|------|---------------|
| Title | Required, 1-200 chars | "Title is required" / "Title must be 1-200 characters" |
| Description | Optional, max 4000 chars | "Description too long" |
| Date | Required, valid date | "Date is required" |
| Status | Required, 0/1/2 | "Invalid status" |
| Order | Auto-generated on create | (internal) |

---

## Migrations

**Initial Migration**: `001_Initial_CreateTasks`

```bash
dotnet ef migrations add 001_Initial_CreateTasks --project TaskTracker.Infrastructure
```

**Schema**:
- Создание схемы `tasks`
- Создание таблицы `Tasks`
- Создание индекса `IX_Tasks_Date_Status_Order`
