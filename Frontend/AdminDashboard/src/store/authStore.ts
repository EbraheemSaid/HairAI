import { create } from "zustand";
import { persist } from "zustand/middleware";
import { authService, type User } from "../services/authService";

interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (user: User, token: string) => void;
  logout: () => Promise<void>;
  initialize: () => void;
  setLoading: (loading: boolean) => void;
  updateProfile: (user: Partial<User>) => void;
  clearStorage: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      user: null,
      token: null,
      isAuthenticated: false,
      isLoading: true,

      login: (user: User, token: string) => {
        // Only allow SuperAdmin access
        if (!user || !user.role || user.role !== "SuperAdmin") {
          throw new Error("Access denied. SuperAdmin privileges required.");
        }

        authService.setTokens(token, user);
        set({ user, token, isAuthenticated: true, isLoading: false });
      },

      logout: async () => {
        set({ isLoading: true });
        try {
          await authService.logout();
        } catch (error) {
          console.error("Logout error:", error);
        } finally {
          set({
            user: null,
            token: null,
            isAuthenticated: false,
            isLoading: false,
          });
        }
      },

      initialize: () => {
        try {
          const token = authService.getStoredToken();
          const user = authService.getStoredUser();

          if (token && user && user.role && user.role === "SuperAdmin") {
            set({
              user,
              token,
              isAuthenticated: true,
              isLoading: false,
            });
          } else {
            // Clear invalid tokens
            authService.clearTokens();
            set({
              user: null,
              token: null,
              isAuthenticated: false,
              isLoading: false,
            });
          }
        } catch (error) {
          console.warn("Auth initialization error:", error);
          // Clear any corrupted data
          authService.clearTokens();
          // Clear localStorage completely to remove any corrupted data
          localStorage.removeItem("hairai-admin-auth-storage");
          set({
            user: null,
            token: null,
            isAuthenticated: false,
            isLoading: false,
          });
        }
      },

      setLoading: (loading: boolean) => {
        set({ isLoading: loading });
      },

      updateProfile: (userData) =>
        set((state) => ({
          user: state.user ? { ...state.user, ...userData } : null,
        })),

      clearStorage: () => {
        authService.clearTokens();
        localStorage.removeItem("hairai-admin-auth-storage");
        set({
          user: null,
          token: null,
          isAuthenticated: false,
          isLoading: false,
        });
      },
    }),
    {
      name: "hairai-admin-auth-storage",
      partialize: (state) => ({
        user: state.user,
        token: state.token,
        isAuthenticated: state.isAuthenticated,
      }),
    }
  )
);
