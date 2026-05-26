import { Task } from '../types/task';

export const DEFAULT_SWIMLANE_KEY = 'без категории';
export const DEFAULT_SWIMLANE_DISPLAY = 'Без категории';

export interface SwimlaneGroup {
  key: string;
  displayName: string;
  tasks: Task[];
}

export function normalizeSwimlaneKey(value: string): string {
  return value.toLowerCase();
}

export function groupBySwimlane(tasks: Task[]): SwimlaneGroup[] {
  const map = new Map<string, Task[]>();

  for (const task of tasks) {
    const key = task.swimlane
      ? normalizeSwimlaneKey(task.swimlane)
      : DEFAULT_SWIMLANE_KEY;
    const group = map.get(key);
    if (group) {
      group.push(task);
    } else {
      map.set(key, [task]);
    }
  }

  const groups: SwimlaneGroup[] = [];

  // "Без категории" comes first
  const defaultGroup = map.get(DEFAULT_SWIMLANE_KEY);
  if (defaultGroup) {
    groups.push({
      key: DEFAULT_SWIMLANE_KEY,
      displayName: DEFAULT_SWIMLANE_DISPLAY,
      tasks: defaultGroup,
    });
    map.delete(DEFAULT_SWIMLANE_KEY);
  }

  // Remaining groups sorted alphabetically by key
  const remaining = Array.from(map.entries()).sort(([a], [b]) =>
    a.localeCompare(b, 'ru'),
  );

  for (const [key, tasks] of remaining) {
    groups.push({
      key,
      displayName: key.charAt(0).toUpperCase() + key.slice(1),
      tasks,
    });
  }

  return groups;
}