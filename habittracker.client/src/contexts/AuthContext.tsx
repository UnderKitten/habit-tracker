import { createContext } from "react";
import type { AuthResponse } from "../types";

interface AuthState {
  user: { email: string } | null;
  token: string | null;
  isLoading: boolean;
  isAuthenticated: boolean;
}

type AuthAction =
  | { type: "RESTORE_TOKEN"; payload: AuthResponse }
  | { type: "SIGN_IN"; payload: AuthResponse }
  | { type: "SIGN_OUT" };

export function authReducer(state: AuthState, action: AuthAction): AuthState {
  switch (action.type) {
    case "RESTORE_TOKEN":
    case "SIGN_IN":
      return {
        user: { email: action.payload.email },
        token: action.payload.token,
        isAuthenticated: true,
        isLoading: false,
      };
    case "SIGN_OUT":
      return {
        user: null,
        token: null,
        isAuthenticated: false,
        isLoading: false,
      };
    default:
      return state;
  }
}

interface AuthContextType {
  state: AuthState;
  dispatch: React.Dispatch<AuthAction>;
}

const AuthContext = createContext<AuthContextType | null>(null);

const initialAuthState: AuthState = {
  user: null,
  token: null,
  isLoading: true,
  isAuthenticated: false,
};
