import { apiService } from "./apiService";
import type { CalibrationProfile } from "../types";

export interface CreateCalibrationProfileRequest {
  name: string;
  calibrationData: Record<string, any>;
}

export interface UpdateCalibrationProfileRequest {
  name: string;
  calibrationData: Record<string, any>;
  isActive: boolean;
}

class CalibrationService {
  async getActiveCalibrationProfiles(
    clinicId?: string
  ): Promise<CalibrationProfile[]> {
    const params = clinicId ? `?clinicId=${clinicId}` : "";
    return apiService.get<CalibrationProfile[]>(`/calibration${params}`);
  }

  async getAllCalibrationProfiles(): Promise<CalibrationProfile[]> {
    return apiService.get<CalibrationProfile[]>("/calibration");
  }

  async getCalibrationProfileById(id: string): Promise<CalibrationProfile> {
    return apiService.get<CalibrationProfile>(`/calibration/${id}`);
  }

  async createCalibrationProfile(
    data: CreateCalibrationProfileRequest
  ): Promise<CalibrationProfile> {
    return apiService.post<CalibrationProfile>("/calibration", data);
  }

  async updateCalibrationProfile(
    id: string,
    data: UpdateCalibrationProfileRequest
  ): Promise<CalibrationProfile> {
    return apiService.put<CalibrationProfile>(`/calibration/${id}`, data);
  }

  async deactivateCalibrationProfile(id: string): Promise<void> {
    return apiService.delete(`/calibration/${id}`);
  }
}

export const calibrationService = new CalibrationService();
