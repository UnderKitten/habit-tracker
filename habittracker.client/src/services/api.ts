import axios from "axios";
import * as habitTypes from "../types";
const baseURL = "http://localhost:5050";

const api = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authAPI = {
  login: async (credentials: habitTypes.LoginCredentials): Promise<habitTypes.AuthResponse> => {
    const response = await api.post<habitTypes.AuthResponse>("/api/auth/login", credentials);
    return response.data;
  },
  register: async (credentials: habitTypes.RegisterCredentials): Promise<habitTypes.AuthResponse> => {
    const response = await api.post<habitTypes.AuthResponse>("/api/auth/register", credentials);
    return response.data;
  },
};

export const habitsAPI = {
  getAll: async (): Promise<habitTypes.Habit[]> => {
    const response = await api.get<habitTypes.Habit[]>("/api/habits")
    return response.data;
  },
  getById: async (id: string): Promise<habitTypes.Habit> => {
    const response = await api.get<habitTypes.Habit>(`/api/habits/${id}`);
    return response.data;
  },
  create: async(habit: habitTypes.CreateHabitDto): Promise<habitTypes.Habit> => {
    const reponse = await api.post<habitTypes.Habit>("/api/habits", habit);
    return reponse.data;
  },
  update: async(id: string, habit: habitTypes.CreateHabitDto): Promise<habitTypes.Habit> => {
    const response = await api.put<habitTypes.Habit>(`/api/habits/${id}`, habit);
    return response.data;
  },
  delete: async(id: string): Promise<void> => {
    await api.delete<void>(`/api/habits/${id}`);
  },
};

export default api;
