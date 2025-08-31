import { apiService } from "./apiService";
import type { User } from "../types";

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  user: User;
  token: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  clinicName?: string;
}

export interface AcceptInvitationRequest {
  token: string;
  firstName: string;
  lastName: string;
  password: string;
}

export interface AcceptInvitationResponse {
  user: User;
  token: string;
}

class AuthService {
  async login(data: LoginRequest): Promise<LoginResponse> {
    return apiService.post<LoginResponse>("/auth/login", data);
  }

  async register(data: RegisterRequest): Promise<LoginResponse> {
    return apiService.post<LoginResponse>("/auth/register", data);
  }

  async acceptInvitation(
    data: AcceptInvitationRequest
  ): Promise<AcceptInvitationResponse> {
    return apiService.post<AcceptInvitationResponse>(
      "/invitations/accept",
      data
    );
  }

  async logout(): Promise<void> {
    // In a real app, you might want to call an API endpoint to invalidate the token
    return Promise.resolve();
  }
}

export const authService = new AuthService();