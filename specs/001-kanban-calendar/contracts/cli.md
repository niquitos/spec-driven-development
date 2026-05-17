# CLI Commands: Task Tracker

**Feature**: 001-kanban-calendar  
**Date**: 2026-05-09  
**Last Updated**: 2026-05-11  
**Status**: Deprecated

---

## Overview

**CLI удалён из проекта.** 

Решение об удалении CLI принято потому, что:
- Веб-интерфейс является основным и единственным UX приложения
- Все операции доступны через интерактивный UI
- Single-user система не требует скриптовой автоматизации

Вместо CLI используйте:
- **Веб-интерфейс**: http://localhost:3000
- **API endpoints**: См. [api.md](api.md)

---

## Historical Reference

Ниже приведена историческая документация CLI, которая была удалена. Эта информация сохраняется только для справки.

~~~

Получить задачи на дату.

**Usage**:
```bash
tasktracker tasks list --date 2026-05-09
tasktracker tasks list --date 2026-05-09 --format json
tasktracker tasks list --date 2026-05-09 --format table
```

**Options**:
- `--date` (required): Дата в формате YYYY-MM-DD
- `--format` (optional): `json` (default) или `table`

**Output (JSON)**:
```json
{
  "date": "2026-05-09",
  "tasks": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "title": "Купить продукты",
      "status": "new",
      "order": 0
    }
  ],
  "total": 1
}
```

**Output (Table)**:
```
Date: 2026-05-09

ID                                    Title              Status      Order
------------------------------------  -----------------  ----------  -----
550e8400-e29b-41d4-a716-446655440000  Купить продукты    new         0

Total: 1 task(s)
```

---

### `tasktracker tasks create`

Создать новую задачу.

**Usage**:
```bash
tasktracker tasks create --title "Купить продукты" --date 2026-05-09 --status new
tasktracker tasks create --title "Задача" --description "Описание" --date 2026-05-09
```

**Options**:
- `--title` (required): Название задачи
- `--description` (optional): Описание
- `--date` (required): Дата задачи (YYYY-MM-DD)
- `--status` (optional): `new` (default), `inprogress`, `done`

**Output**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Купить продукты",
  "status": "new",
  "date": "2026-05-09",
  "createdAt": "2026-05-09T10:00:00Z"
}
```

**Error (non-zero exit)**:
```json
{
  "errors": [
    { "propertyName": "title", "errorMessage": "Title is required" }
  ]
}
```

---

### `tasktracker tasks get`

Получить задачу по ID.

**Usage**:
```bash
tasktracker tasks get --id 550e8400-e29b-41d4-a716-446655440000
```

**Options**:
- `--id` (required): GUID задачи

**Output**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Купить продукты",
  "description": "Молоко, хлеб, яйца",
  "status": "new",
  "date": "2026-05-09",
  "order": 0,
  "createdAt": "2026-05-09T10:00:00Z"
}
```

---

### `tasktracker tasks update`

Обновить задачу.

**Usage**:
```bash
tasktracker tasks update --id 550e8400-e29b-41d4-a716-446655440000 --title "Новое название"
tasktracker tasks update --id 550e8400-e29b-41d4-a716-446655440000 --status done
tasktracker tasks update --id 550e8400-e29b-41d4-a716-446655440000 --date 2026-05-10
```

**Options**:
- `--id` (required): GUID задачи
- `--title` (optional): Новое название
- `--description` (optional): Новое описание
- `--date` (optional): Новая дата
- `--status` (optional): Новый статус

**Output**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Новое название",
  "updatedAt": "2026-05-09T12:00:00Z"
}
```

---

### `tasktracker tasks delete`

Удалить задачу.

**Usage**:
```bash
tasktracker tasks delete --id 550e8400-e29b-41d4-a716-446655440000
tasktracker tasks delete --id 550e8400-e29b-41d4-a716-446655440000 --confirm
```

**Options**:
- `--id` (required): GUID задачи
- `--confirm` (optional): Пропустить подтверждение

**Prompt** (без --confirm):
```
Delete task "Купить продукты"? [y/N]:
```

**Output**:
```
Task deleted.
```

---

### `tasktracker tasks move`

Переместить задачу в другую колонку.

**Usage**:
```bash
tasktracker tasks move --id 550e8400-e29b-41d4-a716-446655440000 --status inprogress
```

**Options**:
- `--id` (required): GUID задачи
- `--status` (required): Целевой статус (`new`, `inprogress`, `done`)

**Output**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "status": "inprogress",
  "order": 1
}
```

---

### `tasktracker tasks bulk-delete`

Массовое удаление задач.

**Usage**:
```bash
tasktracker tasks bulk-delete --ids 550e8400-e29b-41d4-a716-446655440000,uuid-2,uuid-3
```

**Options**:
- `--ids` (required): Список GUID через запятую

**Output**:
```json
{
  "deleted": 3
}
```

---

### `tasktracker tasks bulk-move`

Массовое перемещение задач на другую дату.

**Usage**:
```bash
tasktracker tasks bulk-move --ids uuid-1,uuid-2,uuid-3 --target-date 2026-05-10
```

**Options**:
- `--ids` (required): Список GUID через запятую
- `--target-date` (required): Целевая дата

**Output**:
```json
{
  "moved": 3,
  "targetDate": "2026-05-10"
}
```

---

## Global Options

### `--help`

Показать справку.

```bash
tasktracker --help
tasktracker tasks --help
tasktracker tasks create --help
```

### `--version`

Показать версию.

```bash
tasktracker --version
```

### `--verbose`

Включить подробный вывод.

```bash
tasktracker tasks list --date 2026-05-09 --verbose
```

### `--config`

Путь к файлу конфигурации.

```bash
tasktracker --config ./appsettings.json tasks list --date 2026-05-09
```

---

## Exit Codes

| Code | Description |
|------|-------------|
| 0 | Успех |
| 1 | Ошибка валидации |
| 2 | Ресурс не найден |
| 3 | Ошибка сервера |
| 130 | Отменено пользователем (Ctrl+C) |

---

## Environment Variables

| Variable | Description |
|----------|-------------|
| `TASKTRACKER_API_URL` | URL API (default: `http://localhost:5000`) |
| `TASKTRACKER_CONFIG` | Путь к конфигурации |

~~~
