# API Contracts: Kanban Calendar Board

**Feature**: 001-kanban-calendar  
**Date**: 2026-05-09  
**Last Updated**: 2026-05-11  
**Base URL**: `/api`
**Status**: Implemented

---

## Tasks Resource

### GET /tasks

Получить задачи на указанную дату.

**Query Parameters**:
- `date` (required): Дата в формате YYYY-MM-DD

**Response 200**:
```json
[
  {
    "id": 1,
    "title": "Купить продукты",
    "description": "Молоко, хлеб, яйца",
    "status": 0,
    "date": "2026-05-09",
    "order": 0,
    "createdAt": "2026-05-09T10:00:00Z",
    "updatedAt": null
  }
]
```

**Note**: `id` — integer (INT IDENTITY), `status` — integer enum (0=new, 1=inprogress, 2=done)

**Response 400** (неверный формат даты):
```json
{
  "errors": [
    { "propertyName": "date", "errorMessage": "Invalid date format. Use YYYY-MM-DD" }
  ]
}
```

---

### POST /tasks

Создать новую задачу.

**Request Body**:
```json
{
  "title": "Купить продукты",
  "description": "Молоко, хлеб, яйца",
  "date": "2026-05-09",
  "status": "new"
}
```

**Response 201**:
```json
{
  "id": 1,
  "title": "Купить продукты",
  "description": "Молоко, хлеб, яйца",
  "status": 0,
  "date": "2026-05-09",
  "order": 0,
  "createdAt": "2026-05-09T10:00:00Z",
  "updatedAt": null
}
```

**Response 400** (validation error):
```json
{
  "errors": [
    { "propertyName": "title", "errorMessage": "Title is required" },
    { "propertyName": "date", "errorMessage": "Date is required" }
  ]
}
```

---

### GET /tasks/{id}

Получить задачу по идентификатору.

**Path Parameters**:
- `id` (required): GUID задачи

**Response 200**:
```json
{
  "id": 1,
  "title": "Купить продукты",
  "description": "Молоко, хлеб, яйца",
  "status": 0,
  "date": "2026-05-09",
  "order": 0,
  "createdAt": "2026-05-09T10:00:00Z",
  "updatedAt": null
}
```

**Response 404**: `404 Not Found` (empty response)

**Implementation Note**: API возвращает `404 Not Found` без тела ответа при отсутствии задачи.

---

### PUT /tasks/{id}

Обновить задачу.

**Request Body**:
```json
{
  "title": "Купить продукты и товары для дома",
  "description": "Молоко, хлеб, яйца, шампунь",
  "date": "2026-05-10",
  "status": "inprogress"
}
```

**Response**: `204 No Content`

**Response 404**: `404 Not Found`

**Implementation Note**: API возвращает `204 No Content` при успешном обновлении.

---

### DELETE /tasks/{id}

Удалить задачу.

**Response**: `204 No Content`

**Response 404**: `404 Not Found`

---

## Implementation Notes

**Drag-n-Drop**: Изменение статуса задачи при drag-n-drop реализовано через `PUT /tasks/{id}` с передачей полного объекта задачи, включая новый `status` и `order`.

**Reorder**: Перетаскивание задач внутри колонки также использует `PUT /tasks/{id}` — порядок обновляется на бекенде автоматически при изменении `order`.

## Bulk Operations

### POST /tasks/bulk/delete

Массовое удаление задач.

**Request Body**:
```json
{
  "taskIds": [1, 2, 3]
}
```

**Response 200**:
```json
{
  "deletedCount": 3
}
```

---

### POST /tasks/bulk/move

Массовое перемещение задач на другую дату.

**Request Body**:
```json
{
  "taskIds": [1, 2, 3],
  "targetDate": "2026-05-10"
}
```

**Response 200**:
```json
{
  "movedCount": 3,
  "targetDate": "2026-05-10"
}
```

---

## Error Responses

### 400 Bad Request
```json
{
  "errors": [
    { "propertyName": "field", "errorMessage": "Error description" }
  ]
}
```

### 404 Not Found
```json
{
  "error": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "error": "An unexpected error occurred"
}
```

---

## Status Values

| Integer | Enum | Description |
|---------|------|-------------|
| 0 | `New` | Новые |
| 1 | `InProgress` | В процессе |
| 2 | `Done` | Сделаны |

**Note**: API возвращает статус как integer enum value (0, 1, 2).
