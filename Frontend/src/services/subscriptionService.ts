import { apiService } from "./apiService";
import type { Subscription, BillingRecord } from "../types";

export interface CreateSubscriptionRequest {
  clinicId: string;
  planId: string;
}

export interface SubscriptionPlan {
  id: string;
  name: string;
  price: number;
  currency: string;
  maxUsers: number;
  maxAnalysesPerMonth: number;
}

export interface SubscriptionResponse {
  id: string;
  clinicId: string;
  plan: SubscriptionPlan;
  status: "Active" | "Expired" | "Cancelled";
  startDate: string;
  endDate: string;
  billingHistory: BillingRecord[];
}

class SubscriptionService {
  async getSubscriptionPlans(): Promise<SubscriptionPlan[]> {
    return apiService.get<SubscriptionPlan[]>("/subscriptions/plans");
  }

  async getSubscriptionByClinic(clinicId: string): Promise<SubscriptionResponse> {
    return apiService.get<SubscriptionResponse>(`/subscriptions/clinic/${clinicId}`);
  }

  async createSubscription(data: CreateSubscriptionRequest): Promise<SubscriptionResponse> {
    return apiService.post<SubscriptionResponse>("/subscriptions", data);
  }

  async cancelSubscription(id: string): Promise<void> {
    return apiService.delete(`/subscriptions/${id}`);
  }

  async getBillingHistory(subscriptionId: string): Promise<BillingRecord[]> {
    return apiService.get<BillingRecord[]>(`/subscriptions/${subscriptionId}/billing`);
  }
}

export const subscriptionService = new SubscriptionService();