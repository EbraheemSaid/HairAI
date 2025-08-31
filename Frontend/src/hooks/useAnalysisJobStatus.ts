import { useState, useCallback } from "react";
import { analysisService } from "../services/analysisService";
import { usePolling } from "./usePolling";
import type { AnalysisJob } from "../types";
import { toast } from "react-hot-toast";

interface UseAnalysisJobStatusOptions {
  onCompleted?: (job: AnalysisJob) => void;
  onError?: (error: string) => void;
  pollingInterval?: number;
}

export const useAnalysisJobStatus = (
  jobId: string | null,
  options: UseAnalysisJobStatusOptions = {}
) => {
  const {
    onCompleted,
    onError,
    pollingInterval = 3000, // 3 seconds
  } = options;

  const [job, setJob] = useState<AnalysisJob | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchJobStatus = useCallback(async () => {
    if (!jobId) return;

    try {
      setLoading(true);
      setError(null);

      const statusResponse = await analysisService.getAnalysisJobStatus(jobId);
      const updatedJob: AnalysisJob = {
        id: jobId,
        sessionId: job?.sessionId || "",
        area: job?.area || "",
        calibrationProfileId: job?.calibrationProfileId || "",
        imageUrl: job?.imageUrl || "",
        status: statusResponse.status,
        result: statusResponse.result,
        errorMessage: statusResponse.errorMessage,
        createdAt: job?.createdAt || new Date().toISOString(),
      };

      setJob(updatedJob);

      // Handle status changes
      if (updatedJob.status === "Completed" && job?.status !== "Completed") {
        toast.success("Analysis completed successfully!");
        onCompleted?.(updatedJob);
      } else if (updatedJob.status === "Error" && job?.status !== "Error") {
        const errorMsg = updatedJob.errorMessage || "Analysis failed";
        toast.error(errorMsg);
        onError?.(errorMsg);
        setError(errorMsg);
      }
    } catch (err: any) {
      const errorMessage =
        err.response?.data?.message || "Failed to fetch job status";
      setError(errorMessage);
      console.error("Error fetching job status:", err);
    } finally {
      setLoading(false);
    }
  }, [
    jobId,
    job?.status,
    job?.sessionId,
    job?.area,
    job?.calibrationProfileId,
    job?.imageUrl,
    job?.createdAt,
    onCompleted,
    onError,
  ]);

  // Only poll if job exists and is not in a final state
  const shouldPoll = Boolean(
    jobId && job && job.status !== "Completed" && job.status !== "Error"
  );

  const { isPolling, startPolling, stopPolling } = usePolling(fetchJobStatus, {
    interval: pollingInterval,
    immediate: true,
    enabled: shouldPoll,
  });

  const startJob = useCallback((initialJob: AnalysisJob) => {
    setJob(initialJob);
    setError(null);
  }, []);

  const stopJob = useCallback(() => {
    stopPolling();
    setJob(null);
    setError(null);
  }, [stopPolling]);

  return {
    job,
    loading,
    error,
    isPolling,
    startJob,
    stopJob,
    refetch: fetchJobStatus,
  };
};

