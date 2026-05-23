# Модель данных: Массовые операции над задачами

**Функция**: 003-multiple-operations | **Дата**: 2026-05-23

## Существующие сущности

### TaskEntity (без изменений)

| Поле | Тип | Ограничения | Описание |
|------|-----|-------------|----------|
| Id | int | PK, auto-increment | Уникальный идентификатор |
| Title | string | required, max 200 | Заголовок задачи |
| Description | string? | max 2000 | Описание задачи |
| Status | TaskStatus | required | New (0), InProgress (1), Done (2) |
| Date | DateTime | required | Дата задачи |
| Order | int | required, >= 0 | Порядок в колонке |
| CreatedAt | DateTime | required | Время создания |
| UpdatedAt | DateTime | required | Время последнего обновления |
| Assignee | string? | max 100 | Исполнитель |

### TaskStatus (без изменений)

| Значение | Имя | Описание |
|----------|-----|----------|
| 0 | New | Новая задача |
| 1 | InProgress | В работе |
| 2 | Done | Выполнена |

**Определение «невыполненной» задачи**: `Status != Done` (т.е. `Status == New || Status == InProgress`).

## Новые сущности

### MoveIncompleteToTomorrowCommand

| Поле | Тип | Описание |
|------|-----|----------|
| _(нет параметров)_ | — | Команда не принимает параметров — сервер сам определяет невыполненные задачи |

### MoveIncompleteToTomorrowResponse

| Поле | Тип | Описание |
|------|-----|----------|
| Moved | int | Количество перенесённых задач |
| TargetDate | DateTime | Целевая дата (завтра) |

## Новые методы репозитория

### ITaskRepository.MoveIncompleteToTomorrowAsync

```
Task<int> MoveIncompleteToTomorrowAsync(DateTime tomorrow, CancellationToken cancellationToken)
```

**Поведение**: Обновляет `Date` всех задач с `Status != Done` на значение `tomorrow`. Устанавливает `UpdatedAt = DateTime.UtcNow`. Возвращает количество обновлённых записей.

**Реализация через EF Core**: `ExecuteUpdateAsync` для массового обновления без загрузки сущностей в память.

## Переходы состояний

### Перенос на завтра

```
[New, date=X] → [New, date=tomorrow]
[InProgress, date=X] → [InProgress, date=tomorrow]
[Done, date=X] → [Done, date=X]  (без изменений)
```

Задачи со статусом `Done` не затрагиваются операцией — остаются на своих текущих датах.

### Снятие выделения после массовой операции

```
selectedTaskIds: [1, 2, 3] → [] (после bulkDelete)
selectedTaskIds: [1, 2, 3] → [] (после bulkMove)
moveIncompleteToTomorrow → не влияет на selectedTaskIds (операция не связана с выделением)
```

## Инварианты

1. Операция «Перенести на завтра» не требует выделения задач — действует на все невыполненные задачи
2. Операция «Перенести на завтра» не требует подтверждения — выполняется мгновенно
3. После bulkDelete и bulkMove выделение снимается со всех задач
4. Панель массовых действий отображается только при `selectedTaskIds.length > 0`
5. Кнопка «Перенести на завтра» доступна всегда, независимо от выделения