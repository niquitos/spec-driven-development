# API-контракты: Swimlane-группировка задач на доске

**Функция**: 004-swimlanes | **Дата**: 2026-05-25

## Изменение существующих эндпоинтов

### GET /api/tasks?date={date}&assignees={assignees}&swimlanes={swimlanes}

Расширение существующего эндпоинта для фильтрации по swimlane.

**Параметры запроса**:

| Параметр | Обязательный | Тип | Описание |
|----------|-------------|-----|----------|
| `date` | Да | string (YYYY-MM-DD) | Дата задач |
| `assignees` | Нет | string (comma-separated) | Фильтр по исполнителям |
| `swimlanes` | Нет | string (comma-separated) | Фильтр по swimlane (case-insensitive) |

**Поведение фильтрации**:
- Если `swimlanes` не указан — возвращаются все задачи без фильтрации по swimlane
- Если `swimlanes` указан — возвращаются задачи, у которых `Swimlane` совпадает с одним из значений (case-insensitive)
- Специальное значение `"Без категории"` (или `"без категории"`) в `swimlanes` фильтрует задачи с `Swimlane = null`
- Параметры `assignees` и `swimlanes` комбинируются через **логическое И**: задача должна соответствовать обоим фильтрам одновременно (если оба указаны)
- Если указан только один параметр — фильтрация только по нему

**Примеры**:
```
GET /api/tasks?date=2026-05-25
GET /api/tasks?date=2026-05-25&swimlanes=Фронтенд
GET /api/tasks?date=2026-05-25&swimlanes=без категории
GET /api/tasks?date=2026-05-25&assignees=Иван&swimlanes=Фронтенд,Бэкенд
```

**Ответ** (без изменений в структуре, добавлено поле):

```json
[
  {
    "id": 1,
    "title": "Задача",
    "description": null,
    "status": 0,
    "date": "2026-05-25T00:00:00Z",
    "order": 0,
    "createdAt": "2026-05-25T10:00:00Z",
    "updatedAt": "2026-05-25T10:00:00Z",
    "assignee": "Иван",
    "swimlane": "Фронтенд"
  }
]
```

**Ошибки**:
- `400 Bad Request` — если параметр `date` не указан или имеет неверный формат

---

### POST /api/tasks

Расширение тела запроса новым полем.

**Тело запроса** (добавлено поле `swimlane`):

```json
{
  "title": "Новая задача",
  "date": "2026-05-25",
  "status": 0,
  "swimlane": "Фронтенд"
}
```

- `swimlane` (optional): строка до 100 символов. Если не указано или `null` — задача без группы (отображается в «Без категории»). Пустые строки и пробелы нормализуются в `null`.

**Валидация**:
- `swimlane` максимальная длина — 100 символов. При превышении — `400 Bad Request` с сообщением: `"Swimlane must be at most 100 characters long."`
- Пробельные строки (`"  "`) нормализуются в `null`

**Ответ**: `201 Created` с полным объектом `TaskEntity` (включая `swimlane`).

---

### PUT /api/tasks/{id}

Расширение тела запроса новым полем.

**Тело запроса** (добавлено поле `swimlane`):

```json
{
  "title": "Обновлённая задача",
  "description": "Описание",
  "date": "2026-05-25",
  "status": 1,
  "order": 0,
  "assignee": "Иван",
  "swimlane": "Бэкенд"
}
```

- `swimlane` (optional): при указании обновляет поле. Для очистки swimlane передать `null` или пустую строку.

**Валидация**:
- Те же правила, что и в POST: максимальная длина 100, пробелы → `null`

**Ответ**: `204 No Content` (без изменений).

---

## Новый эндпоинт

### GET /api/tasks/swimlanes?date={date}

Возвращает список уникальных значений swimlane для задач на указанную дату.

**Параметры запроса**:

| Параметр | Обязательный | Тип | Описание |
|----------|-------------|-----|----------|
| `date` | Да | string (YYYY-MM-DD) | Дата для фильтрации задач |

**Ответ**: `200 OK`

```json
["Фронтенд", "Бэкенд"]
```

**Поведение**:
- Возвращает уникальные значения `Swimlane` для задач на указанную дату, где `Swimlane != null`
- «Без категории» НЕ включается в список (он определяется `null`-значениями)
- Значения отсортированы по алфавиту (case-insensitive), но возвращаются в оригинальном написании первого вхождения
- Если на указанную дату нет задач или ни у одной задачи нет swimlane — возвращается пустой массив `[]`
- Параметр `date` обязателен; при отсутствии — `400 Bad Request`

**Ошибки**:
- `400 Bad Request` — если параметр `date` не указан или имеет неверный формат

---

## Контракты frontend → backend

### taskApi.ts — новые методы

```typescript
// Получить список уникальных swimlane на указанную дату
getSwimlanes(date: string): Promise<string[]>

// Получить задачи с фильтрацией по swimlane (расширение getTasks)
getTasks(date: string, assignees?: string[], swimlanes?: string[]): Promise<Task[]>
```

### taskStore.ts — новые элементы состояния

```typescript
// Состояние
swimlaneList: string[];              // Список уникальных swimlane (оригинальное написание)
collapsedSwimlanes: Set<string>;     // Свёрнутые swimlane (нормализованные lowercase-ключи, localStorage)

// Действия
loadSwimlaneList(): Promise<void>;   // Загрузить список swimlane (вызывается после мутаций)
toggleSwimlaneCollapse(swimlaneKey: string): void;  // Свернуть/развернуть swimlane (по нормализованному ключу)
```

### drag-and-drop — составные ID

```
droppableId: "{normalizeSwimlaneKey(swimlane)}:{TaskStatus}"
Примеры: "без категории:0", "фронтенд:1", "бэкенд:2"

draggableId: "{taskId}"  (без изменений)
```

При drop:
- `destination.droppableId` парсится на `swimlaneKey` (lowercase) и `status`
- `source.droppableId` парсится аналогично
- Если `swimlaneKey` изменился → обновить `task.swimlane` (оригинальное написание `displayName` группы, не ключ)
- Если `swimlaneKey` === `"без категории"` → отправить `swimlane: null`
- Если `status` изменился → обновить `task.status`
- Если `order` изменился → обновить `task.order`

### Взаимодействие с bulk-операциями

Bulk-операции (bulk delete, bulk move, move-incomplete-to-tomorrow) сохраняют поле `swimlane` у задач:
- **Bulk delete**: удалённые задачи убираются из swimlane; если удалена последняя задача в swimlane — swimlane исчезает с доски
- **Bulk move**: задачи переносятся на другую дату с сохранением swimlane; на целевой дате появляются соответствующие swimlane
- **Move incomplete to tomorrow**: задачи переносятся на завтра с сохранением swimlane

Поле swimlane НЕ изменяется автоматически при bulk-операциях — оно переносится вместе с задачей.