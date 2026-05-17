# Research: Назначение исполнителей задач

**Date**: 2026-05-17 | **Plan**: [plan.md](plan.md)

## Summary

Все технические решения известны из существующего проекта. Новые технологии не требуются.

## Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Assignee хранение | `string?` на TaskEntity | Нет атрибутов у исполнителя, YAGNI |
| Список исполнителей | SELECT DISTINCT из задач | Не требует отдельного хранилища |
| Комбобокс | HTML5 datalist | Нативный, простой, доступный |
| Фильтр в URL | query-параметр `assignees` | Переживает reload и переключение дат |
| API фильтрация | GET /api/tasks?assignees=val1,val2 | REST-стиль, список через запятую |

## Alternatives Considered

| Alternative | Rejected Because |
|-------------|------------------|
| Отдельная таблица Assignees | Избыточно для строки без атрибутов |
| Фильтр в sessionStorage | Не переживает перезагрузку, user сказал хранить в URL |
| Модальный выбор исполнителя | Дольше, чем комбобокс |
