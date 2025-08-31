import { apiService } from "./apiService";
import type { AnalysisSession, AnalysisJob, AnalysisResult } from "../types";

export interface CreateAnalysisSessionRequest {
  patientId: string;
}

export interface UploadAnalysisImageRequest {
  sessionId: string;
  area: string;
  calibrationProfileId: string;
  image: File;
}

export interface AnalysisJobStatusResponse {
  id: string;
  status: "Pending" | "Processing" | "Completed" | "Error";
  result?: AnalysisResult;
  errorMessage?: string;
}

class AnalysisService {
  async createAnalysisSession(
    data: CreateAnalysisSessionRequest
  ): Promise<AnalysisSession> {
    return apiService.post<AnalysisSession>("/analysis/session", data);
  }

  async getAnalysisSession(id: string): Promise<AnalysisSession> {
    return apiService.get<AnalysisSession>(`/analysis/session/${id}`);
  }

  async getAnalysisJobsBySession(sessionId: string): Promise<AnalysisJob[]> {
    // This endpoint doesn't exist in backend, we'll need to get session details instead
    const session = await this.getAnalysisSession(sessionId);
    return session.analysisJobs || [];
  }

  async uploadAnalysisImage(
    data: UploadAnalysisImageRequest
  ): Promise<AnalysisJob> {
    const formData = new FormData();
    formData.append("sessionId", data.sessionId);
    formData.append("area", data.area);
    formData.append("calibrationProfileId", data.calibrationProfileId);
    formData.append("image", data.image);

    return apiService.post<AnalysisJob>("/analysis/job", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  }

  async getAnalysisJobStatus(
    jobId: string
  ): Promise<AnalysisJobStatusResponse> {
    return apiService.get<AnalysisJobStatusResponse>(
      `/analysis/job/${jobId}/status`
    );
  }

  async getAnalysisJobResult(jobId: string): Promise<AnalysisResult> {
    return apiService.get<AnalysisResult>(`/analysis/job/${jobId}/result`);
  }

  async generateFinalReport(sessionId: string): Promise<any> {
    return apiService.post<any>(`/analysis/session/${sessionId}/report`);
  }
}

export const analysisService = new AnalysisService();
