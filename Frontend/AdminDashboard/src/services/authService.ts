import { apiService } from "./apiService";

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    role: string;
    clinicId?: string;
  };
}

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  clinicId?: string;
}

class AuthService {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await apiService.post<LoginResponse>(
      "/auth/login",
      credentials
    );

    // Only allow SuperAdmin access to AdminDashboard
    if (response.user.role !== "SuperAdmin") {
      throw new Error("Access denied. SuperAdmin privileges required.");
    }

    return response;
  }

  async logout(): Promise<void> {
    try {
      await apiService.post("/auth/logout");
    } catch (error) {
      // Even if logout fails on server, clear local storage
      console.error("Logout error:", error);
    } finally {
      this.clearTokens();
    }
  }

  async getCurrentUser(): Promise<User> {
    return apiService.get<User>("/auth/me");
  }

  async refreshToken(): Promise<{ token: string }> {
    return apiService.post<{ token: string }>("/auth/refresh");
  }

  setTokens(token: string, user: User): void {
    localStorage.setItem("adminToken", token);
    localStorage.setItem("adminUser", JSON.stringify(user));
  }

  clearTokens(): void {
    localStorage.removeItem("adminToken");
    localStorage.removeItem("adminUser");
  }

  getStoredToken(): string | null {
    return localStorage.getItem("adminToken");
  }

  getStoredUser(): User | null {
    try {
      const userStr = localStorage.getItem("adminUser");
      if (!userStr) return null;

      const user = JSON.parse(userStr);
      // Ensure user has required properties
      if (!user || typeof user !== "object" || !user.role) {
        return null;
      }

      return user;
    } catch (error) {
      console.warn("Error parsing stored user:", error);
      return null;
    }
  }

  isAuthenticated(): boolean {
    const token = this.getStoredToken();
    const user = this.getStoredUser();
    return !!(token && user && user.role === "SuperAdmin");
  }
}

export const authService = new AuthService();
