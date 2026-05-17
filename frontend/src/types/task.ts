export enum TaskStatus {
  New = 0,
  InProgress = 1,
  Done = 2,
}

export interface Task {
  id: number;
  title: string;
  description: string | null;
  status: TaskStatus;
  date: string;
  order: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskDto {
  title: string;
  description?: string;
  date: string;
  status: TaskStatus;
  order?: number;
}

export interface UpdateTaskDto {
  title?: string;
  description?: string;
  date?: string;
  status?: TaskStatus;
  order?: number;
}
