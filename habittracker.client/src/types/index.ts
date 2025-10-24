export interface RegisterCredentials {
  email: string;
  password: string;
}

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  email: string;
  expiration: string;
}

export interface Habit {
  id: string;
  name: string;
  description?: string;
  colorHex: string;
  frequency: number;
  created: string;
}

export interface CreateHabitDto {
  name: string;
  description?: string;
  colorHex: string;
  frequency: number;
}
