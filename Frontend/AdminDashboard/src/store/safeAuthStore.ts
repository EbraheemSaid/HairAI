import { create } from "zustand";

interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  clinicId?: string;
}

interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (user: User, token: string) => void;
  logout: () => void;
  initialize: () => void;
  setLoading: (loading: boolean) => void;
}

// Safe getter functions to prevent undefined errors
const safeGetUser = (): User | null => {
  try {
    const userStr = localStorage.getItem("adminUser");
    if (!userStr) return null;
    const user = JSON.parse(userStr);
    // Ensure all required properties exist
    if (!user || !user.id || !user.email || !user.role) return null;
    return user;
  } catch {
    return null;
  }
};

const safeGetToken = (): string | null => {
  try {
    return localStorage.getItem("adminToken");
  } catch {
    return null;
  }
};

const safeClearStorage = (): void => {
  try {
    localStorage.removeItem("adminToken");
    localStorage.removeItem("adminUser");
  } catch {
    // Ignore errors
  }
};

export const useAuthStore = create<AuthState>((set, get) => {
  // Initialize state immediately from localStorage
  const initializeFromStorage = () => {
    try {
      const token = safeGetToken();
      const user = safeGetUser();

      if (token && user && user.role === "SuperAdmin") {
        return {
          user,
          token,
          isAuthenticated: true,
          isLoading: false,
        };
      } else {
        // Clear any invalid data
        safeClearStorage();
        return {
          user: null,
          token: null,
          isAuthenticated: false,
          isLoading: false,
        };
      }
    } catch (error) {
      console.error("Initial auth state initialization failed:", error);
      safeClearStorage();
      return {
        user: null,
        token: null,
        isAuthenticated: false,
        isLoading: false,
      };
    }
  };

  const initialState = initializeFromStorage();

  return {
    ...initialState,

    login: (user: User, token: string) => {
      // Validate user object completely
      if (!user || !user.id || !user.email) {
        const missing = [];
        if (!user) missing.push("user object is null/undefined");
        if (!user?.id) missing.push("id");
        if (!user?.email) missing.push("email");
        throw new Error(
          `Invalid user object - missing required fields: ${missing.join(", ")}`
        );
      }

      // For AdminDashboard, if role is missing, assume SuperAdmin (since only SuperAdmins should access this)
      const userWithRole = {
        ...user,
        role: user.role || "SuperAdmin",
      };

      if (userWithRole.role !== "SuperAdmin") {
        throw new Error("Access denied. SuperAdmin privileges required.");
      }

      try {
        localStorage.setItem("adminToken", token);
        localStorage.setItem("adminUser", JSON.stringify(userWithRole));
        set({
          user: userWithRole,
          token,
          isAuthenticated: true,
          isLoading: false,
        });
      } catch (error) {
        console.error("Failed to save auth data:", error);
        throw new Error("Failed to save authentication data");
      }
    },

    logout: () => {
      safeClearStorage();
      set({
        user: null,
        token: null,
        isAuthenticated: false,
        isLoading: false,
      });
    },

    initialize: () => {
      // Since we now initialize at store creation, this method just re-syncs if needed
      try {
        const token = safeGetToken();
        const user = safeGetUser();
        const currentState = get();

        // Only update if current state doesn't match localStorage
        if (token && user && user.role === "SuperAdmin") {
          if (!currentState.isAuthenticated || currentState.token !== token) {
            set({
              user,
              token,
              isAuthenticated: true,
              isLoading: false,
            });
          }
        } else {
          if (currentState.isAuthenticated) {
            // Clear any invalid data
            safeClearStorage();
            set({
              user: null,
              token: null,
              isAuthenticated: false,
              isLoading: false,
            });
          }
        }
      } catch (error) {
        console.error("Auth re-initialization failed:", error);
        safeClearStorage();
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
  };
});
