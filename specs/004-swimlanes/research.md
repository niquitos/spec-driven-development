# Исследование: Swimlane-группировка задач на доске

**Функция**: 004-swimlanes | **Дата**: 2026-05-25

## 1. Поле Swimlane на TaskEntity — паттерн Assignee

**Решение**: Добавить `string? Swimlane` на `TaskEntity` по аналогии с `Assignee` — nullable free-text колонка без отдельной таблицы.

**Обоснование**: Конституция предписывает DDD и простоту. Assignee уже реализован как nullable string-поле на TaskEntity без foreign key и без отдельной таблицы. Swimlane по спецификации — тоже виртуальная группировка, полностью определяемая значениями поля. Создавать отдельную сущность SwimlaneEntity нарушало бы спецификацию и добавляло бы ненужную сложность.

**Альтернативы**:
- Отдельная таблица Swimlanes — отклонено: спецификация явно говорит, что swimlane не является отдельной сущностью
- Enum для swimlane — отклонено: swimlane — свободный текст, пользователь вводит произвольные значения
- Использовать поле Assignee как swimlane — отклонено: разные доменные концепции, assignee и swimlane не взаимозаменяемы

**Последствия виртуальной сущности (CHK041)**:
- При удалении последней задачи из swimlane он перестаёт отображаться на доске (FR-009). Нет настроек swimlane (цвет, иконка и т.д.) — это зафиксировано в допущениях спецификации
- Swimlane формируется исключительно из значений поля у задач. Нет отдельного управления списком swimlane — они появляются и исчезают по мере изменения значений у задач
- Переименование swimlane у одной задачи НЕ влияет на другие задачи с тем же значением (CHK035)

## 2. Регистронезависимое сравнение имён swimlane

**Решение**: На backend — хранить оригинальное значение, при запросе уникальных значений использовать `.ToLower()` для case-insensitive группировки. На frontend — использовать **ту же нормализацию** `.toLowerCase()` (не `toLocaleLowerCase()`), чтобы гарантировать совпадение ключей между backend и frontend. Для отображения — первый встреченный вариант написания.

**Обоснование**: Спецификация FR-011 требует регистронезависимой обработки. Использование единой нормализации `.toLowerCase()` на обеих сторонах гарантирует консистентность ключей. Вариант с `toLocaleLowerCase()` на frontend создавал бы расхождение с backend `.ToLower()` для некоторых символов кириллицы в разных локалях (например, турецкий 'İ').

**Альтернативы**:
- `citext` расширение PostgreSQL — отклонено: требует включения расширения на уровне БД, избыточно для одного поля
- Хранить только lowercase — отклонено: спецификация требует отображение варианта, введённого первым (FR-011)
- `toLocaleLowerCase()` на frontend — отклонено: создаёт расхождение с backend `.ToLower()` для некоторых Unicode-символов в не-ASCII локалях

### Реализация на backend

```csharp
// TaskRepository.cs — GetSwimlanesAsync
public async Task<string[]> GetSwimlanesAsync(CancellationToken ct)
{
    return await _context.Tasks
        .Where(t => t.Swimlane != null)
        .GroupBy(t => t.Swimlane!.ToLower())
        .Select(g => g.First().Swimlane!)
        .OrderBy(s => s.ToLower() == "без категории" ? "" : s.ToLower())
        .ToArrayAsync(ct);
}
```

### Реализация на frontend

```typescript
// Единая функция нормализации ключей — используется и для группировки, и для localStorage
function normalizeSwimlaneKey(value: string): string {
  return value.toLowerCase();
}

// Группировка задач по swimlane с сохранением первого варианта написания
function groupBySwimlane(tasks: Task[]): SwimlaneGroup[] {
  const groupMap = new Map<string, { displayName: string; tasks: Task[] }>();
  for (const task of tasks) {
    const key = normalizeSwimlaneKey(task.swimlane ?? DEFAULT_SWIMLANE_KEY);
    if (!groupMap.has(key)) {
      groupMap.set(key, { displayName: task.swimlane ?? DEFAULT_SWIMLANE_DISPLAY, tasks: [] });
    }
    groupMap.get(key)!.tasks.push(task);
  }
  // Отображаемое имя — первое встреченное значение
  return [...groupMap.entries()]
    .sort(/* "Без категории" первый, остальные по алфавиту */)
    .map(([key, { displayName, tasks }]) => ({ key, displayName, tasks }));
}
```

### Ключ для свёрнутого состояния (CHK012, CHK015, CHK047)

LocalStorage хранит **нормализованные (lowercase) ключи** swimlane. Это гарантирует совпадение с droppableId и ключами группировки:

```typescript
const DEFAULT_SWIMLANE_KEY = normalizeSwimlaneKey("Без категории");
// localStorage хранит Set<string> нормализованных ключей
// При поиске свёрнутого состояния используется нормализованный ключ
function isSwimlaneCollapsed(swimlaneKey: string, collapsed: Set<string>): boolean {
  return collapsed.has(normalizeSwimlaneKey(swimlaneKey));
}
```

## 3. Drag-and-drop в матрице swimlane × колонка

**Решение**: Использовать составные `droppableId` в формате `{swimlaneKey}:{TaskStatus}`. При drop определять новый swimlane и status из destination.droppableId. Вертикальное перемещение (между swimlane) обновляет поле swimlane у задачи. Горизонтальное перемещение (между колонками) обновляет поле status.

**Обоснование**: Библиотека `@hello-pangea/dnd` уже используется в проекте. Она поддерживает произвольные `droppableId` и позволяет извлекать компоненты ID из строки. Составной формат `swimlaneKey:status` однозначно идентифицирует ячейку в матрице и прост в парсинге.

**Альтернативы**:
- Числовые ID с lookup-таблицей — отклонено: сложнее парсить, требует дополнительное состояние
- Переключение на `@dnd-kit` — отклонено: проект уже использует `@hello-pangea/dnd` в Board.tsx, переключение на другую библиотеку — избыточный рефакторинг
- Вложенные Droppable — отклонено: `@hello-pangea/dnd` не поддерживает вложенные Droppable одного типа

### Реализация

```typescript
// Droppable ID формат: `${normalizeSwimlaneKey(swimlane)}:${TaskStatus}`
// Примеры: "без категории:0", "фронтенд:1", "бэкенд:2"
// Ключ swimlane — всегда lowercase для консистентности

// Парсинг при drop:
const [swimlaneKey, statusStr] = destination.droppableId.split(':');
const newStatus = Number(statusStr) as TaskStatus;

// В moveTask — обновление swimlane при вертикальном перемещении
```

### Drag-and-drop в свёрнутый swimlane (CHK032)

При drag-and-drop задачи в свёрнутый swimlane:
- Свёрнутый swimlane остаётся свёрнутым визуально, но принимает задачу
- Задача перемещается в целевой swimlane, поле swimlane обновляется через API
- Пользователь видит изменение счётчика задач в заголовке свёрнутого swimlane
- Swimlane НЕ разворачивается автоматически — пользователь может развернуть его вручную, чтобы увидеть результат

### Drag-and-drop последней задачи из swimlane (CHK016, CHK026)

При перетаскивании последней задачи из swimlane:
- Optimistic update: задача мгновенно перемещается в целевой swimlane, исходный swimlane исчезает с доски
- Если API-запрос не удался — откат (rollback): задача возвращается в исходный swimlane, swimlane появляется снова
- Это согласовано с существующим optimistic update-паттерном в taskStore

### Пустой swimlane на доске (CHK034)

Пустой swimlane (задачи есть, но все в других колонках) отображается как горизонтальная полоса с пустыми ячейками в колонках, где нет задач. Каждая ячейка — Droppable-область, куда можно перетащить задачу. Это соответствует спецификации: «Swimlane отображается как пустая горизонтальная полоса в соответствующих колонках».

### Диагональное перемещение (CHK024)

При одновременном изменении swimlane и status (перетаскивание по диагонали):
- Оба поля обновляются за один API-вызов (`updateTask`)
- Порядок обновления: swimlane + status + order — одна транзакция
- Optimistic update обновляет оба поля одновременно

### Drag-and-drop из «Без категории» (CHK025)

Перемещение из swimlane «Без категории» в именованный swimlane устанавливает `task.swimlane = displayName` (оригинальное написание, не lowercase). Обратное перемещение — устанавливает `task.swimlane = null`. Это обрабатывается в handler drag-and-drop через проверку: если целевой ключ === DEFAULT_SWIMLANE_KEY, отправлять `swimlane: null`.

## 4. Состояние сворачивания swimlane — localStorage

**Решение**: Хранить множество свёрнутых swimlane в localStorage по ключу `tasktracker_collapsed_swimlanes`. Значение — JSON-массив **нормализованных (lowercase) ключей** swimlane. Чтение при монтировании компонента Board, запись при каждом toggle.

**Обоснование**: Спецификация FR-005 требует сохранения состояния между сеансами. localStorage — простое решение без backend-зависимостей. Спецификация допускает, что состояние сохраняется локально (допущение: «Состояние сворачивания swimlane сохраняется локально в браузере пользователя»).

**Альтернативы**:
- Серверное хранение (профиль пользователя) — отклонено: нет системы аутентификации, избыточно для UI-состояния
- sessionStorage — отклонено: не сохраняется между сеансами (FR-005)
- Zustand persist middleware — отклонено: избыточно для одного простого массива строк, добавляет зависимость

### Реализация

```typescript
const STORAGE_KEY = 'tasktracker_collapsed_swimlanes';
const DEFAULT_SWIMLANE_KEY = normalizeSwimlaneKey("Без категории");

function loadCollapsedSwimlanes(): Set<string> {
  try {
    const stored = localStorage.getItem(STORAGE_KEY);
    return stored ? new Set(JSON.parse(stored)) : new Set();
  } catch { return new Set(); }
}

function saveCollapsedSwimlanes(collapsed: Set<string>): void {
  localStorage.setItem(STORAGE_KEY, JSON.stringify([...collapsed]));
}
```

### Фантомные записи в localStorage (CHK033)

При удалении swimlane (последняя задача ушла) — запись остаётся в localStorage. Это не вызывает проблем: при следующем рендере несуществующий ключ не найдёт соответствующей группы и будет проигнорирован. Периодическая очистка не требуется — stale-записи не влияют на производительность при объёме до ~20 swimlane. Если пользователь заново создаёт swimlane с тем же именем, старая запись в localStorage автоматически активирует свёрнутое состояние — это ожидаемое поведение (сохранение предпочтения).

## 5. Паттерн SwimlaneCombobox — копия AssigneeCombobox

**Решение**: Создать `SwimlaneCombobox` как вариант `AssigneeCombobox` с изменёнными ARIA-атрибутами и метками. В дальнейшем возможен рефакторинг в обобщённый `StringCombobox`.

**Обоснование**: AssigneeCombobox уже реализует весь нужный UX: autocomplete, free-text input, keyboard navigation, case-insensitive filtering. Единственное отличие — ARIA-роли и placeholder. Создание отдельного компонента через копирование с модификацией следует паттерну YAGNI — обобщённый компонент можно создать позже при появлении третьего combobox.

**Альтернативы**:
- Обобщённый `StringCombobox` — отклонено на данном этапе: только два варианта (Assignee, Swimlane), преждевременная абстракция
- Использовать AssigneeCombobox напрямую — отклонено: ARIA-атрибуты должны отражать семантику поля

### Фильтрация в SwimlaneCombobox (CHK027)

Фильтрация в SwimlaneCombobox — регистронезависимый substring match (аналогично AssigneeCombobox): `opt.toLowerCase().includes(inputValue.toLowerCase())`. Это обеспечивает поиск по частичному совпадению независимо от регистра. При вводе символов другого алфавита (например, латиница при кириллических swimlane) — совпадений не будет, и список автодополнения будет пуст. Это ожидаемое поведение: пользователь видит пустой список и может ввести новое значение (free-text input).

### Создание задачи с swimlane (CHK023)

Поле swimlane в CreateTaskModal:
- Начальное значение: пустое (null) → задача попадает в «Без категории»
- Опциональное: пользователь может не указывать swimlane
- Autocomplete: предлагает существующие значения из swimlaneList
- Free-text: пользователь может ввести новое значение

## 6. Порядок swimlane на доске

**Решение**: На backend — `GetSwimlanesAsync` возвращает значения, отсортированные с «Без категории» первым, остальные — по алфавиту (case-insensitive). На frontend — отображать в полученном порядке.

**Обоснование**: Спецификация FR-013 требует: «Без категории» всегда первый, остальные — по алфавиту. Сортировка на backend гарантирует консистентный порядок независимо от frontend-реализации.

### «Без категории» — единственный swimlane (CHK003)

Когда все задачи без swimlane, «Без категории» — единственный swimlane на доске. Он отображается первым, как обычно. Когда на доске есть только именованные swimlane, «Без категории» всё равно отображается (если есть задачи без swimlane), согласно FR-002.

### Количество задач в swimlane (CHK002, CHK046)

Количество задач отображается в заголовке swimlane рядом с названием. Считается общее количество задач в swimlane (суммарно по всем колонкам), а не per-column. Формат: «Без категории (5)», «Фронтенд (12)».

## 7. Индексация в PostgreSQL

**Решение**: Добавить составной индекс `(Date, Swimlane)` для оптимизации запроса уникальных swimlane по дате. Не добавлять индекс на `(Date, Status, Swimlane)`, т.к. существующий индекс `(Date, Status)` достаточен для фильтрации по дате, а swimlane-группировка происходит в памяти на frontend.

**Обоснование**: Основной запрос — получение задач по дате с фильтром по assignee. Swimlane-группировка выполняется на frontend (разделение задач по группам). Индекс `(Date, Swimlane)` оптимизирует запрос `GetSwimlanesAsync` с фильтрацией по дате. Текущий индекс `(Date, Status)` покрывает фильтрацию по дате и статусу.

## 8. Производительность (CHK036, CHK037)

### SC-001: Отображение доски с 200 задачами за 2 секунды

Группировка задач по swimlane на frontend — операция O(n) по количеству задач. При 200 задачах и ~20 swimlane это не создаёт узкого места. Рендеринг матрицы swimlane × колонки оптимизируется React-мемоизацией. Индекс `(Date, Swimlane)` ускоряет backend-запрос.

### SC-004: Изменение swimlane за 2 секунды

При изменении swimlane у задачи:
1. Optimistic update — задача мгновенно перегруппируется на доске
2. API-вызов `updateTask` — обновляет поле swimlane
3. После успеха — вызов `loadSwimlaneList` для обновления списка swimlane (для обновления combobox и header'ов)

Обновление списка swimlane не блокирует визуальное перемещение — optimistic update обеспечивает мгновенный отклик.

### SC-002: Мгновенное сворачивание/разворачивание

Сворачивание/разворачивание swimlane — чисто frontend-операция (переключение CSS-класса и сохранение в localStorage). Целевой порог: <100мс (время на re-render и перерисовку DOM). Анимация перехода — CSS transition с `max-height`, длительность 200–300мс для визуальной плавности.

## 9. Доступность (Accessibility) (CHK038, CHK039, CHK040)

### ARIA-атрибуты для сворачивания/разворачивания

Заголовок swimlane с кнопкой сворачивания:
- `role="button"` на кнопке toggle
- `aria-expanded="true/false"` — текущее состояние
- `aria-controls="<id>"` — ссылка на содержимое swimlane
- `aria-label="Свернуть swimlane Фронтенд"` / `aria-label="Развернуть swimlane Фронтенд"`

### Клавиатурная навигация

- `Enter` или `Space` на заголовке swimlane — сворачивает/разворачивает
- Клавиатурная навигация внутри свёрнутого swimlane — невозможна (задачи скрыты, `display: none` или `aria-hidden="true"`)
- Tab-порядок: после заголовка swimlane, Tab переходит к заголовку следующего swimlane (если свёрнут) или к первой задаче (если развёрнут)

### Анимация сворачивания/разворачивания

- CSS `transition` на `max-height` и `opacity` — длительность 200мс
- Свёрнутый swimlane: `max-height: 0; overflow: hidden; opacity: 0; transition: max-height 200ms ease-out, opacity 150ms ease-out`
- Развёрнутый swimlane: `max-height: <calculated>; opacity: 1; transition: max-height 200ms ease-in, opacity 150ms ease-in`
- При `prefers-reduced-motion: reduce` — анимация отключается, переключение мгновенное

## 10. Взаимодействие с bulk-операциями (CHK007, CHK044)

Существующие bulk-операции из спецификации 003 должны корректно работать с полем swimlane:

- **Bulk delete**: при удалении задач swimlane обновляются. Если удалена последняя задача в swimlane — swimlane исчезает с доски
- **Bulk move**: при перемещении задач на другую дату swimlane задач сохраняется. Задачи появляются в соответствующих swimlane на целевой дате
- **Move incomplete to tomorrow**: задачи переносятся на завтра, поле swimlane сохраняется. На завтрашней доске появляются соответствующие swimlane

Во всех bulk-операциях поле swimlane НЕ очищается и NOT изменяется автоматически — оно переносится вместе с задачей.

## 11. Валидация swimlane (CHK030, CHK031)

### Максимальная длина (100 символов)

Если пользователь вводит swimlane длиной > 100 символов:
- Frontend: обрезает ввод в SwimlaneCombobox на 100 символов (maxlength на input)
- Backend: возвращает `400 Bad Request` с сообщением об ошибке валидации

### Пробельные строки

Если пользователь вводит swimlane, состоящий только из пробелов:
- Frontend: нормализует в пустое значение (trim), что эквивалентно null
- Backend: нормализует `string.IsNullOrWhiteSpace(swimlane) ? null : swimlane`
- Результат: задача перемещается в «Без категории»

## 12. Взаимодействие фильтров (CHK006, CHK048)

### Assignee-фильтр + swimlane-группировка

Фильтр по assignee и группировка по swimlane работают ортогонально:
1. Сначала фильтрация: из всех задач на дату выбираются те, что соответствуют фильтру по assignee
2. Затем группировка: отфильтрованные задачи группируются по swimlane
3. Если в swimlane нет задач после фильтрации — swimlane не отображается (аналогично FR-009)

Порядок операций: `фильтрация → группировка`. Это значит, что swimlane «Без категории» может содержать задачи разных assignee, и при фильтрации по конкретному assignee в «Без категории» останутся только его задачи.

## Сводка решений

| # | Вопрос | Решение |
|---|--------|---------|
| 1 | Тип поля Swimlane | `string?` на TaskEntity, аналогично Assignee |
| 2 | Регистронезависимость | `.ToLower()` в LINQ-запросах, `.toLowerCase()` на frontend (единообразно) |
| 3 | Drag-and-drop | Составные `droppableId`: `{swimlaneKey}:{status}`, `@hello-pangea/dnd` |
| 4 | Свёрнутое состояние | localStorage, ключ `tasktracker_collapsed_swimlanes`, Set<string> (нормализованные ключи) |
| 5 | Combobox | SwimlaneCombobox — копия AssigneeCombobox с другими ARIA-атрибутами |
| 6 | Порядок | Backend: «Без категории» первый, остальные по алфавиту |
| 7 | Индексация | Добавить `(Date, Swimlane)` к существующим индексам |
| 8 | Производительность | Optimistic update + мемоизация; сворачивание <100мс |
| 9 | Доступность | ARIA-атрибуты, клавиатурная навигация, prefers-reduced-motion |
| 10 | Bulk-операции | Swimlane сохраняется при bulk-операциях, не очищается и не переназначается |
| 11 | Валидация | Max 100 символов (ошибка 400 на backend, maxlength на frontend), пробелы → null |
| 12 | Фильтры | Фильтрация по assignee → группировка по swimlane (ортогонально) |