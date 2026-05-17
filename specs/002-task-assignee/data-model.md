# Data Model: Назначение исполнителей задач

**Date**: 2026-05-17 | **Spec**: [spec.md](spec.md)

## Entities

### TaskEntity (изменение)

| Field | Type | Changes |
|-------|------|---------|
| Id | int | unchanged |
| Title | string | unchanged |
| Description | string? | unchanged |
| Status | TaskStatus | unchanged |
| Date | DateTime | unchanged |
| Order | int | unchanged |
| CreatedAt | DateTime | unchanged |
| UpdatedAt | DateTime | unchanged |
| **Assignee** | **string?** | **NEW — nullable, max 100 chars** |

### Assignee (логическая сущность)

Assignee не является отдельной сущностью БД. Список уникальных исполнителей формируется через SELECT DISTINCT Assignee FROM Tasks WHERE Assignee IS NOT NULL.

| Attribute | Type | Notes |
|-----------|------|-------|
| Name | string | max 100 chars, case-insensitive comparison |

## Validation Rules

- Assignee: nullable (может отсутствовать)
- Если задан: не пустая строка, не только пробелы
- Максимальная длина: 100 символов
- Регистронезависимое сравнение при поиске дубликатов

## State Transitions

- Task без Assignee → Task с Assignee (при создании или редактировании)
- Task с Assignee X → Task с Assignee Y (при редактировании)
- Task с Assignee → Task без Assignee (очистка поля при редактировании)
