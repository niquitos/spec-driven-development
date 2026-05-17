# API Contracts: Назначение исполнителей задач

**Date**: 2026-05-17

## GET /api/tasks

Получение задач с опциональной фильтрацией по исполнителям.

**Query Parameters** (изменение):

| Param | Type | Required | Description |
|-------|------|----------|-------------|
| date | string | yes | Дата в формате YYYY-MM-DD |
| **assignees** | **string** | **no** | **Список исполнителей через запятую (опционально)** |

**Пример**: `GET /api/tasks?date=2026-05-17&assignees=Иван,Петр`

**Response** (изменение):

```json
[
  {
    "id": 1,
    "title": "Задача",
    "description": "Описание",
    "status": 0,
    "date": "2026-05-17",
    "order": 0,
    "createdAt": "2026-05-17T10:00:00Z",
    "updatedAt": "2026-05-17T10:00:00Z",
    "assignee": "Иван"
  }
]
```

## POST /api/tasks

Создание задачи (изменение).

**Request Body** (изменение):

```json
{
  "title": "Задача",
  "description": "Описание",
  "date": "2026-05-17",
  "status": 0,
  "order": 0,
  "assignee": "Иван"
}
```

## PUT /api/tasks/{id}

Обновление задачи (изменение).

**Request Body** (изменение):

```json
{
  "title": "Задача",
  "description": "Описание",
  "date": "2026-05-17",
  "status": 0,
  "order": 0,
  "assignee": "Иван"
}
```

Для очистки исполнителя: `"assignee": null` или `"assignee": ""`.
