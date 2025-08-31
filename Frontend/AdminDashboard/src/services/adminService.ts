import { apiService } from "./apiService";

// Types based on backend DTOs
export interface AdminClinicResponse {
  id: string;
  name: string;
  address?: string;
  phone?: string;
  email?: string;
  createdAt: string;
  subscription?: {
    id: string;
    status: string;
    plan?: {
      name: string;
      priceMonthly: number;
      currency: string;
    };
    startDate: string;
    endDate: string;
  };
}

export interface CreateClinicRequest {
  name: string;
  address?: string;
  phone?: string;
  email?: string;
}

export interface AdminUserResponse {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  clinicId?: string;
  clinic?: {
    id: string;
    name: string;
  };
  lastLoginAt?: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  role: "SuperAdmin" | "ClinicAdmin" | "Doctor";
  clinicId?: string;
}

export interface AdminSubscriptionResponse {
  id: string;
  clinicId: string;
  clinic: {
    id: string;
    name: string;
  };
  plan: {
    id: string;
    name: string;
    priceMonthly: number;
    currency: string;
    maxUsers: number;
    maxAnalysesPerMonth: number;
  };
  status: string;
  startDate: string;
  endDate: string;
  createdAt: string;
}

export interface DashboardStatsResponse {
  totalClinics: number;
  activeClinics: number;
  totalUsers: number;
  activeUsers: number;
  totalSubscriptions: number;
  activeSubscriptions: number;
  monthlyRevenue: number;
  currency: string;
}

class AdminService {
  // Clinic Management
  async getAllClinics(): Promise<AdminClinicResponse[]> {
    try {
      const response = await apiService.get<{
        success: boolean;
        message: string;
        clinics: AdminClinicResponse[];
      }>("/clinics");

      if (response && response.clinics) {
        return response.clinics;
      }

      // If the response structure is different, try to handle it
      if (Array.isArray(response)) {
        return response;
      }

      return [];
    } catch (error) {
      console.error("Error fetching clinics:", error);
      return [];
    }
  }

  async getClinicById(id: string): Promise<AdminClinicResponse> {
    return apiService.get<AdminClinicResponse>(`/clinics/${id}`);
  }

  async createClinic(data: CreateClinicRequest): Promise<AdminClinicResponse> {
    return apiService.post<AdminClinicResponse>("/admin/clinics", data);
  }

  async updateClinic(
    id: string,
    data: Partial<CreateClinicRequest>
  ): Promise<AdminClinicResponse> {
    return apiService.put<AdminClinicResponse>(`/clinics/${id}`, data);
  }

  async deleteClinic(id: string): Promise<void> {
    return apiService.delete<void>(`/clinics/${id}`);
  }

  // User Management
  async getAllUsers(): Promise<AdminUserResponse[]> {
    try {
      const response = await apiService.get<{
        success: boolean;
        message: string;
        users: AdminUserResponse[];
      }>("/admin/users");

      console.log("Users API response:", response);

      if (response && response.users) {
        return response.users;
      }

      // If the response structure is different, try to handle it
      if (Array.isArray(response)) {
        return response;
      }

      return [];
    } catch (error) {
      console.error("Error fetching users:", error);
      return [];
    }
  }

  async getUserById(id: string): Promise<AdminUserResponse> {
    return apiService.get<AdminUserResponse>(`/admin/users/${id}`);
  }

  async createUser(data: CreateUserRequest): Promise<AdminUserResponse> {
    const response = await apiService.post<{
      success: boolean;
      message: string;
      userId?: string;
      email?: string;
      firstName?: string;
      lastName?: string;
      role?: string;
      clinicId?: string;
    }>("/admin/users", data);

    if (!response.success) {
      throw new Error(response.message);
    }

    // Transform response to match AdminUserResponse
    return {
      id: response.userId || "",
      firstName: response.firstName || "",
      lastName: response.lastName || "",
      email: response.email || "",
      role: response.role || "",
      clinicId: response.clinicId,
      lastLoginAt: undefined,
      isActive: true,
      createdAt: new Date().toISOString(),
    };
  }

  async updateUser(
    id: string,
    data: Partial<CreateUserRequest>
  ): Promise<AdminUserResponse> {
    return apiService.put<AdminUserResponse>(`/admin/users/${id}`, data);
  }

  async deleteUser(id: string): Promise<void> {
    const response = await apiService.delete<{
      success: boolean;
      message: string;
    }>(`/admin/users/${id}`);

    if (!response.success) {
      throw new Error(response.message);
    }
  }

  async deactivateUser(id: string): Promise<AdminUserResponse> {
    const response = await apiService.patch<{
      success: boolean;
      message: string;
      isActive: boolean;
    }>(`/admin/users/${id}/deactivate`);

    if (!response.success) {
      throw new Error(response.message);
    }

    // Return a partial response - the calling code will refresh the list
    return {} as AdminUserResponse;
  }

  async activateUser(id: string): Promise<AdminUserResponse> {
    const response = await apiService.patch<{
      success: boolean;
      message: string;
      isActive: boolean;
    }>(`/admin/users/${id}/activate`);

    if (!response.success) {
      throw new Error(response.message);
    }

    // Return a partial response - the calling code will refresh the list
    return {} as AdminUserResponse;
  }

  // Subscription Management
  async getAllSubscriptions(): Promise<AdminSubscriptionResponse[]> {
    // For now, return mock data since subscription management isn't fully implemented
    return [
      {
        id: "1",
        clinicId: "1",
        clinic: { id: "1", name: "Sample Clinic" },
        plan: {
          id: "1",
          name: "Professional",
          priceMonthly: 2500,
          currency: "EGP",
          maxUsers: 15,
          maxAnalysesPerMonth: 500,
        },
        status: "Active",
        startDate: "2023-01-15T00:00:00Z",
        endDate: "2024-01-15T00:00:00Z",
        createdAt: "2023-01-15T00:00:00Z",
      },
    ];
  }

  async getSubscriptionById(id: string): Promise<AdminSubscriptionResponse> {
    return apiService.get<AdminSubscriptionResponse>(`/subscriptions/${id}`);
  }

  async cancelSubscription(id: string): Promise<AdminSubscriptionResponse> {
    return apiService.delete<AdminSubscriptionResponse>(`/subscriptions/${id}`);
  }

  async renewSubscription(id: string): Promise<AdminSubscriptionResponse> {
    return apiService.patch<AdminSubscriptionResponse>(
      `/subscriptions/${id}/renew`
    );
  }

  // Dashboard Stats
  async getDashboardStats(): Promise<DashboardStatsResponse> {
    return apiService.get<DashboardStatsResponse>("/admin/dashboard/stats");
  }

  // System Health
  async getSystemHealth(): Promise<{
    database: "online" | "offline";
    messageQueue: "online" | "offline";
    storage: "online" | "offline";
    api: "online" | "offline";
  }> {
    return apiService.get("/admin/system/health");
  }
}

export const adminService = new AdminService();
