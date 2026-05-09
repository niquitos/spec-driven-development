# API Contracts: Kanban Calendar Board

**Feature**: 001-kanban-calendar  
**Date**: 2026-05-09  
**Base URL**: `/api`

---

## Tasks Resource

### GET /tasks

Получить задачи на указанную дату.

**Query Parameters**:
- `date` (required): Дата в формате YYYY-MM-DD

**Response 200**:
```json
{
  "date": "2026-05-09",
  "tasks": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "title": "Купить продукты",
      "description": "Молоко, хлеб, яйца",
      "status": "new",
      "date": "2026-05-09",
      "order": 0,
      "createdAt": "2026-05-09T10:00:00Z"
    }
  ]
}
```

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
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Купить продукты",
  "description": "Молоко, хлеб, яйца",
  "status": "new",
  "date": "2026-05-09",
  "order": 0,
  "createdAt": "2026-05-09T10:00:00Z"
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
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Купить продукты",
  "description": "Молоко, хлеб, яйца",
  "status": "new",
  "date": "2026-05-09",
  "order": 0,
  "createdAt": "2026-05-09T10:00:00Z"
}
```

**Response 404**:
```json
{
  "error": "Task not found"
}
```

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

**Response 200**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Купить продукты и товары для дома",
  "description": "Молоко, хлеб, яйца, шампунь",
  "status": "inprogress",
  "date": "2026-05-10",
  "order": 0,
  "createdAt": "2026-05-09T10:00:00Z",
  "updatedAt": "2026-05-09T12:00:00Z"
}
```

**Response 404**:
```json
{
  "error": "Task not found"
}
```

---

### DELETE /tasks/{id}

Удалить задачу.

**Response 204**: No content

**Response 404**:
```json
{
  "error": "Task not found"
}
```

---

### PATCH /tasks/{id}/status

Изменить статус задачи (drag-n-drop между колонками).

**Request Body**:
```json
{
  "status": "inprogress"
}
```

**Response 200**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Купить продукты",
  "status": "inprogress",
  "order": 1
}
```

---

### PUT /tasks/reorder

Изменить порядок задач в колонке.

**Request Body**:
```json
{
  "date": "2026-05-09",
  "status": "new",
  "taskIds": ["uuid-1", "uuid-2", "uuid-3"]
}
```

**Response 200**:
```json
{
  "success": true,
  "updated": 3
}
```

---

## Bulk Operations

### POST /tasks/bulk/delete

Массовое удаление задач.

**Request Body**:
```json
{
  "taskIds": ["uuid-1", "uuid-2", "uuid-3"]
}
```

**Response 200**:
```json
{
  "deleted": 3
}
```

---

### POST /tasks/bulk/move

Массовое перемещение задач на другую дату.

**Request Body**:
```json
{
  "taskIds": ["uuid-1", "uuid-2", "uuid-3"],
  "targetDate": "2026-05-10"
}
```

**Response 200**:
```json
{
  "moved": 3,
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

| Value | Description |
|-------|-------------|
| `new` | Новые |
| `inprogress` | В процессе |
| `done` | Сделаны |
