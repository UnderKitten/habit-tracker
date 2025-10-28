import { createContext, useReducer, useContext, use, useEffect } from "react";
import type {
  AuthResponse,
  LoginCredentials,
  RegisterCredentials,
} from "../types";
import { authAPI } from "../services/api";

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

function authReducer(state: AuthState, action: AuthAction): AuthState {
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

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [state, dispatch] = useReducer(authReducer, initialAuthState);
  useEffect(() => {
    const token = localStorage.getItem("token");
    const email = localStorage.getItem("email");
    if (token && email) {
      dispatch({ type: "RESTORE_TOKEN", payload: { token, email, expiration: "" } });
    } else {
      dispatch({ type: "SIGN_OUT" });
    }
  }, []);
  return (
    <AuthContext.Provider value={{ state, dispatch }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
