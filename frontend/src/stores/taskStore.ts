import { create } from 'zustand';
import { Task, TaskStatus, CreateTaskDto, UpdateTaskDto } from '../types/task';
import { taskApi } from '../services/taskApi';
import { addToast } from '../components/Toast';
import { normalizeSwimlaneKey } from '../utils/swimlane';

// Получаем дату из URL или возвращаем сегодня
function getInitialDate(): Date {
  const params = new URLSearchParams(window.location.search);
  const dateParam = params.get('date');
  if (dateParam) {
    const parsed = new Date(dateParam);
    if (!isNaN(parsed.getTime())) {
      return parsed;
    }
  }
  return new Date();
}

// Получаем фильтр assignee из URL
function getInitialAssigneeFilter(): string[] {
  const params = new URLSearchParams(window.location.search);
  const assigneesParam = params.get('assignees');
  if (assigneesParam) {
    return assigneesParam.split(',').map((a) => a.trim()).filter(Boolean);
  }
  return [];
}

// Загружаем свёрнутые swimlane из localStorage
function loadCollapsedSwimlanes(): Set<string> {
  try {
    const stored = localStorage.getItem('tasktracker_collapsed_swimlanes');
    if (stored) {
      const arr: string[] = JSON.parse(stored);
      return new Set(arr.map(normalizeSwimlaneKey));
    }
  } catch {
    // Игнорируем ошибки парсинга
  }
  return new Set();
}

// Обновляем URL при изменении даты
function updateUrlDate(date: Date) {
  const params = new URLSearchParams(window.location.search);
  const dateStr = date.toISOString().split('T')[0];
  params.set('date', dateStr);
  const newUrl = `${window.location.pathname}?${params.toString()}`;
  window.history.replaceState({}, '', newUrl);
}

// Обновляем URL при изменении фильтра assignee
function updateUrlAssigneeFilter(assignees: string[]) {
  const params = new URLSearchParams(window.location.search);
  params.set('date', params.get('date') || new Date().toISOString().split('T')[0]);
  if (assignees.length > 0) {
    params.set('assignees', assignees.join(','));
  } else {
    params.delete('assignees');
  }
  const newUrl = `${window.location.pathname}?${params.toString()}`;
  window.history.replaceState({}, '', newUrl);
}

interface TaskState {
  tasks: Task[];
  selectedDate: Date;
  selectedTaskIds: number[];
  isLoading: boolean;
  error: string | null;
  isCreateModalOpen: boolean;
  editingTask: Task | null;
  assigneeFilter: string[];
  assigneeList: string[];
  swimlaneList: string[];
  collapsedSwimlanes: Set<string>;

  setTasks: (tasks: Task[]) => void;
  addTask: (task: Task) => void;
  updateTask: (id: number, updates: UpdateTaskDto) => Promise<void>;
  deleteTask: (id: number) => Promise<void>;
  setSelectedDate: (date: Date) => void;
  toggleTaskSelection: (id: number) => void;
  clearSelection: () => void;
  moveTask: (id: number, newStatus: TaskStatus, newOrder: number) => Promise<void>;
  reorderTask: (id: number, newOrder: number) => void;
  loadTasks: (date: Date) => Promise<void>;
  loadAssigneeList: () => Promise<void>;
  loadSwimlaneList: () => Promise<void>;
  toggleSwimlaneCollapse: (swimlaneKey: string) => void;
  createTask: (dto: CreateTaskDto) => Promise<void>;
  setIsCreateModalOpen: (open: boolean) => void;
  setEditingTask: (task: Task | null) => void;
  bulkDelete: () => Promise<void>;
  bulkMove: (targetDate: Date) => Promise<void>;
  isMovingIncomplete: boolean;
  moveIncompleteToDate: (targetDate: Date) => Promise<void>;
  setAssigneeFilter: (assignees: string[]) => void;
  getAssigneeList: () => string[];
}

export const useTaskStore = create<TaskState>((set, get) => ({
  tasks: [],
  selectedDate: getInitialDate(),
  selectedTaskIds: [],
  isLoading: false,
  error: null,
  isCreateModalOpen: false,
  editingTask: null,
  assigneeFilter: getInitialAssigneeFilter(),
  assigneeList: [],
  swimlaneList: [],
  collapsedSwimlanes: loadCollapsedSwimlanes(),
  isMovingIncomplete: false,

  setTasks: (tasks) => set({ tasks }),

  loadTasks: async (date) => {
    const state = get();
    set({ isLoading: true, error: null });
    try {
      const dateStr = date.toISOString().split('T')[0];
      const tasks = await taskApi.getTasks(dateStr, state.assigneeFilter.length > 0 ? state.assigneeFilter : undefined);
      set({ tasks, isLoading: false });
      if (tasks.length === 0 && state.assigneeFilter.length === 0) {
        addToast('Нет задач на эту дату', 'info');
      }
      get().loadAssigneeList();
      get().loadSwimlaneList();
    } catch (error) {
      set({ error: 'Failed to load tasks', isLoading: false });
    }
  },

  loadAssigneeList: async () => {
    try {
      const assigneeList = await taskApi.getAssignees();
      set({ assigneeList });
    } catch {
      // Не фатально
    }
  },

  loadSwimlaneList: async () => {
    try {
      const dateStr = get().selectedDate.toISOString().split('T')[0];
      const swimlaneList = await taskApi.getSwimlanes(dateStr);
      set({ swimlaneList });
    } catch {
      // Не фатально
    }
  },

  toggleSwimlaneCollapse: (swimlaneKey) => {
    const normalizedKey = normalizeSwimlaneKey(swimlaneKey);
    set((state) => {
      const newCollapsed = new Set(state.collapsedSwimlanes);
      if (newCollapsed.has(normalizedKey)) {
        newCollapsed.delete(normalizedKey);
      } else {
        newCollapsed.add(normalizedKey);
      }
      localStorage.setItem('tasktracker_collapsed_swimlanes', JSON.stringify(Array.from(newCollapsed)));
      return { collapsedSwimlanes: newCollapsed };
    });
  },

  addTask: (task) => {
    set((state) => ({ tasks: [...state.tasks, task] }));
    get().loadAssigneeList();
  },

  updateTask: async (id, updates) => {
    const state = useTaskStore.getState();
    const task = state.tasks.find(t => t.id === id);
    if (!task) return;

    const dateStr = state.selectedDate.toISOString().split('T')[0];
    const oldStatus = task.status;
    const newStatus = updates.status ?? oldStatus;
    const isStatusChanged = oldStatus !== newStatus;

    // Если статус изменился - пересчитываем порядок в обеих колонках
    if (isStatusChanged) {
      const dateTasks = state.tasks.filter(t =>
        new Date(t.date).toDateString() === new Date(dateStr).toDateString()
      );

      // Задачи старой колонки (без перемещаемой)
      const oldColumnTasks = dateTasks.filter(t => t.status === oldStatus && t.id !== id)
        .sort((a, b) => a.order - b.order);

      // Задачи новой колонки (без перемещаемой)
      const newColumnTasks = dateTasks.filter(t => t.status === newStatus && t.id !== id)
        .sort((a, b) => a.order - b.order);

      // Вставляем задачу в конец новой колонки
      newColumnTasks.push(task);

      // Обновляем order для всех задач обеих колонок
      const updatedTasks = new Map<number, Task>();
      oldColumnTasks.forEach((t, i) => {
        updatedTasks.set(t.id, { ...t, order: i, updatedAt: new Date().toISOString() });
      });
      newColumnTasks.forEach((t, i) => {
        updatedTasks.set(t.id, t.id === id
          ? { ...t, status: newStatus, order: i, updatedAt: new Date().toISOString(), ...updates }
          : { ...t, order: i, updatedAt: new Date().toISOString() }
        );
      });

      // Обновляем стейт
      set((state) => ({
        tasks: state.tasks.map((t) => {
          if (updatedTasks.has(t.id)) {
            return updatedTasks.get(t.id)!;
          }
          if (t.id === id) {
            return { ...t, ...updates, updatedAt: new Date().toISOString() };
          }
          return t;
        }),
      }));
    } else {
      // Статус не изменился - просто обновляем задачу
      set((state) => ({
        tasks: state.tasks.map((task) =>
          task.id === id ? { ...task, ...updates, updatedAt: new Date().toISOString() } : task
        ),
      }));
    }

    // Отправляем на бекенд
    await taskApi.updateTask(id, {
      title: updates.title ?? task.title,
      description: updates.description ?? task.description ?? undefined,
      date: updates.date ?? task.date,
      status: updates.status ?? task.status,
      order: updates.order ?? task.order,
      assignee: updates.assignee,
      swimlane: updates.swimlane,
    });

    get().loadAssigneeList();
    get().loadSwimlaneList();
  },

  setEditingTask: (task) => set({ editingTask: task }),

  deleteTask: async (id) => {
    // Optimistic update
    set((state) => ({
      tasks: state.tasks.filter((task) => task.id !== id),
      selectedTaskIds: state.selectedTaskIds.filter((taskId) => taskId !== id),
    }));
    // Send request to backend
    await taskApi.deleteTask(id);
    get().loadAssigneeList();
  },

  setSelectedDate: (date) => {
    updateUrlDate(date);
    set({ selectedDate: date });
  },

  toggleTaskSelection: (id) => set((state) => ({
    selectedTaskIds: state.selectedTaskIds.includes(id)
      ? state.selectedTaskIds.filter((taskId) => taskId !== id)
      : [...state.selectedTaskIds, id],
  })),

  clearSelection: () => set({ selectedTaskIds: [] }),

  moveTask: async (id, newStatus, newOrder) => {
    const state = useTaskStore.getState();
    const task = state.tasks.find(t => t.id === id);
    if (!task) return;

    const dateStr = state.selectedDate.toISOString().split('T')[0];

    // Все задачи этой даты
    const dateTasks = state.tasks.filter(t =>
      new Date(t.date).toDateString() === new Date(dateStr).toDateString()
    );

    // Задачи в целевой колонке (без перемещаемой)
    const targetColumnTasks = dateTasks.filter(t => t.status === newStatus && t.id !== id);

    // Сортируем по order
    const sorted = [...targetColumnTasks].sort((a, b) => a.order - b.order);

    // Вставляем на новую позицию
    const insertIndex = Math.min(newOrder, sorted.length);
    sorted.splice(insertIndex, 0, task);

    // Новый список задач: пересчитанная целевая колонка + остальные задачи
    const newTasks = [
      // Целевая колонка с новым order
      ...sorted.map((t, i) => t.id === id
        ? { ...t, status: newStatus, order: i, updatedAt: new Date().toISOString() }
        : { ...t, order: i, updatedAt: new Date().toISOString() }
      ),
      // Остальные задачи (не целевой колонки и не перемещаемая)
      ...dateTasks
        .filter(t => t.status !== newStatus && t.id !== id)
        .map((t, i) => ({ ...t, order: i, updatedAt: new Date().toISOString() })),
      // Задачи других дат
      ...state.tasks.filter(t => new Date(t.date).toDateString() !== new Date(dateStr).toDateString()),
    ];

    set({ tasks: newTasks });

    // Отправляем на бекенд
    await taskApi.updateTask(id, {
      title: task.title,
      description: task.description ?? undefined,
      date: task.date,
      status: newStatus,
      order: insertIndex,
      assignee: task.assignee ?? undefined,
    });

    get().loadAssigneeList();
  },

  reorderTask: (id, newOrder) => set((state) => ({
    tasks: state.tasks.map((task) =>
      task.id === id
        ? { ...task, order: newOrder, updatedAt: new Date().toISOString() }
        : task
    ),
  })),

  createTask: async (dto) => {
    const task = await taskApi.createTask(dto);
    set((state) => ({ tasks: [...state.tasks, task] }));
    get().loadSwimlaneList();
  },

  setIsCreateModalOpen: (open) => set({ isCreateModalOpen: open }),

  bulkDelete: async () => {
    const { selectedTaskIds } = useTaskStore.getState();
    if (selectedTaskIds.length === 0) return;

    const count = selectedTaskIds.length;
    try {
      await taskApi.bulkDelete(selectedTaskIds);
      set((state) => ({
        tasks: state.tasks.filter((task) => !state.selectedTaskIds.includes(task.id)),
        selectedTaskIds: [],
      }));
      get().loadAssigneeList();
      get().loadSwimlaneList();
      addToast(`Удалено ${count} ${count === 1 ? 'задача' : count < 5 ? 'задачи' : 'задач'}`);
    } catch {
      addToast('Ошибка при удалении задач', 'error');
    }
  },

  bulkMove: async (targetDate) => {
    const { selectedTaskIds } = useTaskStore.getState();
    if (selectedTaskIds.length === 0) return;

    const count = selectedTaskIds.length;
    const dateStr = targetDate.toISOString().split('T')[0];
    try {
      await taskApi.bulkMove(selectedTaskIds, dateStr);
      set((state) => ({
        tasks: state.tasks.filter((task) => !state.selectedTaskIds.includes(task.id)),
        selectedTaskIds: [],
      }));
      get().loadAssigneeList();
      get().loadSwimlaneList();
      addToast(`Перенесено ${count} ${count === 1 ? 'задача' : count < 5 ? 'задачи' : 'задач'}`);
    } catch {
      addToast('Ошибка при перемещении задач', 'error');
    }
  },

  moveIncompleteToDate: async (targetDate: Date) => {
    set({ isMovingIncomplete: true });
    try {
      const dateStr = targetDate.toISOString().split('T')[0];
      const result = await taskApi.moveIncompleteToDate(dateStr);
      if (result.moved > 0) {
        const count = result.moved;
        addToast(`Перенесено ${count} ${count === 1 ? 'задача' : count < 5 ? 'задачи' : 'задач'}`);
        await get().loadTasks(get().selectedDate);
      } else {
        addToast('Нет задач для переноса', 'info');
      }
    } catch {
      addToast('Ошибка при переносе задач', 'error');
    } finally {
      set({ isMovingIncomplete: false });
    }
  },

  setAssigneeFilter: (assignees) => {
    updateUrlAssigneeFilter(assignees);
    set({ assigneeFilter: assignees });
    // Перезагружаем задачи с новым фильтром
    const state = get();
    const dateStr = state.selectedDate.toISOString().split('T')[0];
    taskApi.getTasks(dateStr, assignees.length > 0 ? assignees : undefined)
      .then((tasks) => {
        set({ tasks });
        get().loadAssigneeList();
        get().loadSwimlaneList();
      })
      .catch(() => {});
  },

  getAssigneeList: () => {
    return get().assigneeList;
  },
}));