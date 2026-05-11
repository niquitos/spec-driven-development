# Data Model: Kanban Calendar Board

**Feature**: 001-kanban-calendar  
**Date**: 2026-05-09  
**Last Updated**: 2026-05-11  
**Source**: [spec.md](spec.md), [research.md](research.md)
**Status**: Implemented

---

## Core Entities

### TaskEntity

Представляет задачу на канбан-доске.

```csharp
public class TaskEntity
{
    public int Id { get; set; }            // Primary key (IDENTITY)
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
- `Id`: Уникальный идентификатор (INT IDENTITY — автоинкремент)
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
-- Table: Tasks (EF Core использует dbo по умолчанию)
CREATE TABLE Tasks (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Status INT NOT NULL CONSTRAINT DF_Tasks_Status DEFAULT 0,
    [Date] DATE NOT NULL,
    [Order] INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Tasks_CreatedAt DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    
    INDEX IX_Tasks_Date_Status_Order ([Date], Status, [Order])
);
```

**Indexes**:
- `IX_Tasks_Date_Status_Order`: Composite index для быстрого поиска задач на дату с сортировкой

**Implementation Note**: В реализованной версии используется `INT IDENTITY` вместо `GUID` для упрощения и лучшей производительности при индексации.

---

## EF Core Configuration

**Implementation Note**: В реализованной версии конфигурация упрощена — используется конвенция по умолчанию без явной конфигурации через `IEntityTypeConfiguration`.

```csharp
// Infrastructure/Persistence/AppDbContext.cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<TaskEntity>(builder =>
    {
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
        
        builder.HasIndex(t => new { t.Date, t.Status, t.Order });
    });
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

В реализованной версии используется прямое возвращение `TaskEntity` без дополнительных DTO.

### API Response Model

```csharp
// Frontend type (TypeScript)
export interface Task {
  id: number;              // INT из БД
  title: string;
  description: string | null;
  status: TaskStatus;      // Enum: 0=new, 1=inprogress, 2=done
  date: string;            // ISO date string
  order: number;
  createdAt: string;       // ISO datetime string
  updatedAt: string;       // ISO datetime string
}
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
