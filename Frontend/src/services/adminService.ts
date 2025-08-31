import { apiService } from "./apiService";
import type { Clinic, SubscriptionPlan } from "../types";

export interface ManuallyCreateClinicRequest {
  name: string;
  address?: string;
  phone?: string;
  email?: string;
}

export interface ManuallyActivateSubscriptionRequest {
  clinicId: string;
  planId: string;
  startDate: string;
  endDate: string;
}

export interface ManuallyLogPaymentRequest {
  subscriptionId: string;
  amount: number;
  currency: string;
  paymentMethod: string;
}

export interface AdminClinicResponse {
  id: string;
  name: string;
  address?: string;
  phone?: string;
  email?: string;
  createdAt: string;
  updatedAt: string;
  subscription?: {
    id: string;
    plan: SubscriptionPlan;
    status: string;
    startDate: string;
    endDate: string;
  };
}

class AdminService {
  // Clinic management
  async getAllClinics(): Promise<AdminClinicResponse[]> {
    return apiService.get<AdminClinicResponse[]>("/admin/clinics");
  }

  async manuallyCreateClinic(data: ManuallyCreateClinicRequest): Promise<AdminClinicResponse> {
    return apiService.post<AdminClinicResponse>("/admin/clinics", data);
  }

  async manuallyActivateSubscription(data: ManuallyActivateSubscriptionRequest): Promise<any> {
    return apiService.post<any>("/admin/subscriptions", data);
  }

  async manuallyLogPayment(data: ManuallyLogPaymentRequest): Promise<any> {
    return apiService.post<any>("/admin/payments", data);
  }

  // Subscription plan management
  async getAllSubscriptionPlans(): Promise<SubscriptionPlan[]> {
    return apiService.get<SubscriptionPlan[]>("/admin/subscription-plans");
  }

  async createSubscriptionPlan(data: Partial<SubscriptionPlan>): Promise<SubscriptionPlan> {
    return apiService.post<SubscriptionPlan>("/admin/subscription-plans", data);
  }

  async updateSubscriptionPlan(id: string, data: Partial<SubscriptionPlan>): Promise<SubscriptionPlan> {
    return apiService.put<SubscriptionPlan>(`/admin/subscription-plans/${id}`, data);
  }

  async deleteSubscriptionPlan(id: string): Promise<void> {
    return apiService.delete(`/admin/subscription-plans/${id}`);
  }
}

export const adminService = new AdminService();