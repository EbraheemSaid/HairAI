import { apiService } from "./apiService";
import type { Invitation } from "../types";

export interface CreateInvitationRequest {
  email: string;
  role: string;
  clinicId: string;
}

export interface AcceptInvitationRequest {
  token: string;
  firstName: string;
  lastName: string;
  password: string;
}

export interface InvitationResponse {
  id: string;
  email: string;
  clinicId: string;
  role: string;
  token: string;
  expiresAt: string;
  used: boolean;
  createdAt: string;
}

class InvitationService {
  async getInvitationByToken(token: string): Promise<InvitationResponse> {
    return apiService.get<InvitationResponse>(`/invitations/${token}`);
  }

  async createInvitation(data: CreateInvitationRequest): Promise<InvitationResponse> {
    return apiService.post<InvitationResponse>("/invitations", data);
  }

  async acceptInvitation(data: AcceptInvitationRequest): Promise<any> {
    return apiService.post<any>("/invitations/accept", data);
  }

  async getAllInvitationsForClinic(clinicId: string): Promise<InvitationResponse[]> {
    return apiService.get<InvitationResponse[]>(`/invitations/clinic/${clinicId}`);
  }

  async cancelInvitation(id: string): Promise<void> {
    return apiService.delete(`/invitations/${id}`);
  }
}

export const invitationService = new InvitationService();