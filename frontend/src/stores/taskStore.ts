import { create } from 'zustand';
import { Task, TaskStatus, CreateTaskDto, UpdateTaskDto } from '../types/task';
import { taskApi } from '../services/taskApi';

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

// Обновляем URL при изменении даты
function updateUrlDate(date: Date) {
  const params = new URLSearchParams(window.location.search);
  const dateStr = date.toISOString().split('T')[0];
  params.set('date', dateStr);
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

  setTasks: (tasks: Task[]) => void;
  addTask: (task: Task) => void;
  updateTask: (id: number, updates: UpdateTaskDto) => Promise<void>;
  deleteTask: (id: number) => void;
  setSelectedDate: (date: Date) => void;
  toggleTaskSelection: (id: number) => void;
  clearSelection: () => void;
  moveTask: (id: number, newStatus: TaskStatus, newOrder: number) => Promise<void>;
  reorderTask: (id: number, newOrder: number) => void;
  loadTasks: (date: Date) => Promise<void>;
  createTask: (dto: CreateTaskDto) => Promise<void>;
  setIsCreateModalOpen: (open: boolean) => void;
  setEditingTask: (task: Task | null) => void;
  bulkDelete: () => Promise<void>;
  bulkMove: (targetDate: Date) => Promise<void>;
}

export const useTaskStore = create<TaskState>((set) => ({
  tasks: [],
  selectedDate: getInitialDate(),
  selectedTaskIds: [],
  isLoading: false,
  error: null,
  isCreateModalOpen: false,
  editingTask: null,

  setTasks: (tasks) => set({ tasks }),

  loadTasks: async (date) => {
    set({ isLoading: true, error: null });
    try {
      const dateStr = date.toISOString().split('T')[0];
      const tasks = await taskApi.getTasks(dateStr);
      set({ tasks, isLoading: false });
    } catch (error) {
      set({ error: 'Failed to load tasks', isLoading: false });
    }
  },

  addTask: (task) => set((state) => ({ tasks: [...state.tasks, task] })),

  updateTask: async (id, updates) => {
    await taskApi.updateTask(id, updates);
    set((state) => ({
      tasks: state.tasks.map((task) =>
        task.id === id ? { ...task, ...updates, updatedAt: new Date().toISOString() } : task
      ),
    }));
  },

  setEditingTask: (task) => set({ editingTask: task }),

  deleteTask: (id) => set((state) => ({
    tasks: state.tasks.filter((task) => task.id !== id),
    selectedTaskIds: state.selectedTaskIds.filter((taskId) => taskId !== id),
  })),

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

    // Сначала оптимистично обновляем UI
    set((state) => ({
      tasks: state.tasks.map((t) =>
        t.id === id
          ? { ...t, status: newStatus, order: newOrder, updatedAt: new Date().toISOString() }
          : t
      ),
    }));
    // Затем отправляем полное обновление задачи
    await taskApi.updateTask(id, {
      title: task.title,
      description: task.description ?? undefined,
      date: task.date,
      status: newStatus,
      order: newOrder,
    });
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
  },

  setIsCreateModalOpen: (open) => set({ isCreateModalOpen: open }),

  bulkDelete: async () => {
    const { selectedTaskIds } = useTaskStore.getState();
    if (selectedTaskIds.length === 0) return;
    
    await taskApi.bulkDelete(selectedTaskIds);
    set((state) => ({
      tasks: state.tasks.filter((task) => !state.selectedTaskIds.includes(task.id)),
      selectedTaskIds: [],
    }));
  },

  bulkMove: async (targetDate) => {
    const { selectedTaskIds } = useTaskStore.getState();
    if (selectedTaskIds.length === 0) return;
    
    const dateStr = targetDate.toISOString().split('T')[0];
    await taskApi.bulkMove(selectedTaskIds, dateStr);
    set((state) => ({
      tasks: state.tasks.filter((task) => !state.selectedTaskIds.includes(task.id)),
      selectedTaskIds: [],
    }));
  },
}));
