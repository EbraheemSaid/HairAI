import { apiService } from "./apiService";
import type { Clinic } from "../types";

export interface CreateClinicRequest {
  name: string;
  address?: string;
  phone?: string;
  email?: string;
}

export interface UpdateClinicRequest {
  name: string;
  address?: string;
  phone?: string;
  email?: string;
}

export interface ClinicResponse {
  id: string;
  name: string;
  address?: string;
  phone?: string;
  email?: string;
  createdAt: string;
  updatedAt: string;
}

class ClinicService {
  async getAllClinics(): Promise<ClinicResponse[]> {
    return apiService.get<ClinicResponse[]>("/clinics");
  }

  async getClinicById(id: string): Promise<ClinicResponse> {
    return apiService.get<ClinicResponse>(`/clinics/${id}`);
  }

  async createClinic(data: CreateClinicRequest): Promise<ClinicResponse> {
    return apiService.post<ClinicResponse>("/clinics", data);
  }

  async updateClinic(
    id: string,
    data: UpdateClinicRequest
  ): Promise<ClinicResponse> {
    return apiService.put<ClinicResponse>(`/clinics/${id}`, data);
  }

  async deleteClinic(id: string): Promise<void> {
    return apiService.delete(`/clinics/${id}`);
  }
}

export const clinicService = new ClinicService();