import { apiService } from "./apiService";
import type { Patient } from "../types";

export interface CreatePatientRequest {
  clinicId: string;
  clinicPatientId?: string;
  firstName: string;
  lastName: string;
  dateOfBirth?: string;
}

export interface UpdatePatientRequest {
  clinicPatientId?: string;
  firstName: string;
  lastName: string;
  dateOfBirth?: string;
}

class PatientService {
  async getPatients(clinicId?: string): Promise<Patient[]> {
    const params = clinicId ? `?clinicId=${clinicId}` : "";
    return apiService.get<Patient[]>(`/patients${params}`);
  }

  async getPatientById(id: string): Promise<Patient> {
    return apiService.get<Patient>(`/patients/${id}`);
  }

  async createPatient(data: CreatePatientRequest): Promise<Patient> {
    return apiService.post<Patient>("/patients", data);
  }

  async updatePatient(
    id: string,
    data: UpdatePatientRequest
  ): Promise<Patient> {
    return apiService.put<Patient>(`/patients/${id}`, data);
  }

  async deletePatient(id: string): Promise<void> {
    return apiService.delete(`/patients/${id}`);
  }
}

export const patientService = new PatientService();
