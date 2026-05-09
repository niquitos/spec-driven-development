import axios from 'axios';
import { Task, CreateTaskDto, UpdateTaskDto, TaskStatus } from '../types/task';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
});

export const taskApi = {
  async getTasks(date: string): Promise<Task[]> {
    const response = await api.get<Task[]>('/tasks', { params: { date } });
    return response.data;
  },

  async createTask(dto: CreateTaskDto): Promise<Task> {
    const response = await api.post<Task>('/tasks', dto);
    return response.data;
  },

  async updateTask(id: number, dto: UpdateTaskDto): Promise<Task> {
    const response = await api.put<Task>(`/tasks/${id}`, dto);
    return response.data;
  },

  async deleteTask(id: number): Promise<void> {
    await api.delete(`/tasks/${id}`);
  },

  async moveTask(id: number, status: TaskStatus, order: number): Promise<Task> {
    const response = await api.patch<Task>(`/tasks/${id}/status`, { status, order });
    return response.data;
  },

  async bulkDelete(ids: number[]): Promise<void> {
    await api.post('/tasks/bulk-delete', { ids });
  },

  async bulkMove(ids: number[], status: TaskStatus): Promise<void> {
    await api.post('/tasks/bulk-move', { ids, status });
  },
};
