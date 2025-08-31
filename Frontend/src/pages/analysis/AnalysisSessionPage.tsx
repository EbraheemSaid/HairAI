import React, { useState, useEffect, useCallback } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  ArrowLeftIcon,
  DocumentTextIcon,
  ChartBarIcon,
} from "@heroicons/react/24/outline";
import { analysisService } from "../../services/analysisService";
import { usePolling } from "../../hooks/usePolling";
import type { AnalysisSession, AnalysisJob } from "../../types";
import { toast } from "react-hot-toast";

export const AnalysisSessionPage: React.FC = () => {
  const { sessionId } = useParams<{ sessionId: string }>();
  const navigate = useNavigate();
  const [session, setSession] = useState<AnalysisSession | null>(null);
  const [analysisJobs, setAnalysisJobs] = useState<AnalysisJob[]>([]);
  const [loading, setLoading] = useState(true);
  const [generatingReport, setGeneratingReport] = useState(false);

  const fetchSessionData = useCallback(async () => {
    if (!sessionId) return;

    try {
      const [sessionData, jobsData] = await Promise.all([
        analysisService.getAnalysisSession(sessionId),
        analysisService.getAnalysisJobsBySession(sessionId),
      ]);

      setSession(sessionData);

      // Check for job status changes
      if (analysisJobs.length > 0) {
        jobsData.forEach((newJob) => {
          const oldJob = analysisJobs.find((job) => job.id === newJob.id);
          if (oldJob && oldJob.status !== newJob.status) {
            if (newJob.status === "Completed") {
              toast.success(`Analysis completed for ${newJob.area} area`);
            } else if (newJob.status === "Error") {
              toast.error(
                `Analysis failed for ${newJob.area} area: ${
                  newJob.errorMessage || "Unknown error"
                }`
              );
            }
          }
        });
      }

      setAnalysisJobs(jobsData);
    } catch (error) {
      toast.error("Failed to load analysis session data");
      console.error("Error fetching session data:", error);
    } finally {
      if (loading) {
        setLoading(false);
      }
    }
  }, [sessionId, analysisJobs, loading]);

  useEffect(() => {
    if (sessionId) {
      fetchSessionData();
    }
  }, [sessionId]);

  // Determine if polling should be active
  const hasProcessingJobs = analysisJobs.some(
    (job) => job.status === "Pending" || job.status === "Processing"
  );

  // Use the polling hook
  const { isPolling } = usePolling(fetchSessionData, {
    interval: 3000, // 3 seconds for faster updates
    immediate: false, // Don't fetch immediately since useEffect already does
    enabled: hasProcessingJobs, // Only poll when there are processing jobs
  });

  const handleGenerateReport = async () => {
    setGeneratingReport(true);
    try {
      await analysisService.generateFinalReport(sessionId!);
      toast.success("Report generated successfully!");
      // Refresh the session data to show the report
      fetchSessionData();
    } catch (error) {
      toast.error("Failed to generate report");
      console.error("Error generating report:", error);
    } finally {
      setGeneratingReport(false);
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>
    );
  }

  if (!session) {
    return (
      <div className="text-center py-12">
        <h3 className="mt-2 text-sm font-medium text-gray-900">
          Session not found
        </h3>
        <p className="mt-1 text-sm text-gray-500">
          The analysis session you're looking for doesn't exist.
        </p>
        <div className="mt-6">
          <button
            type="button"
            onClick={() => navigate("/analysis/new")}
            className="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            <ArrowLeftIcon className="-ml-1 mr-2 h-5 w-5" aria-hidden="true" />
            New Analysis
          </button>
        </div>
      </div>
    );
  }

  const completedJobs = analysisJobs.filter(
    (job) => job.status === "Completed"
  );
  const pendingJobs = analysisJobs.filter(
    (job) => job.status === "Pending" || job.status === "Processing"
  );
  const failedJobs = analysisJobs.filter((job) => job.status === "Error");

  const allJobsCompleted = analysisJobs.length > 0 && pendingJobs.length === 0;

  return (
    <div className="space-y-6">
      <div>
        <button
          onClick={() => navigate("/patients")}
          className="inline-flex items-center text-sm font-medium text-indigo-600 hover:text-indigo-500"
        >
          <ArrowLeftIcon className="h-5 w-5 mr-1" />
          Back to patients
        </button>
        <div className="mt-2 flex items-center justify-between">
          <h1 className="text-2xl font-bold text-gray-900">Analysis Session</h1>
          {allJobsCompleted && (
            <button
              onClick={handleGenerateReport}
              disabled={generatingReport}
              className="inline-flex items-center px-3 py-2 border border-gray-300 shadow-sm text-sm leading-4 font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
            >
              {generatingReport ? (
                <>
                  <svg
                    className="animate-spin -ml-1 mr-2 h-4 w-4"
                    xmlns="http://www.w3.org/2000/svg"
                    fill="none"
                    viewBox="0 0 24 24"
                  >
                    <circle
                      className="opacity-25"
                      cx="12"
                      cy="12"
                      r="10"
                      stroke="currentColor"
                      strokeWidth="4"
                    ></circle>
                    <path
                      className="opacity-75"
                      fill="currentColor"
                      d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                    ></path>
                  </svg>
                  Generating...
                </>
              ) : (
                <>
                  <DocumentTextIcon className="h-4 w-4 mr-1" />
                  Generate Report
                </>
              )}
            </button>
          )}
        </div>
        <div className="mt-1 flex items-center justify-between">
          <p className="text-sm text-gray-500">
            Session ID: {session.id.substring(0, 8)} • Created:{" "}
            {session.createdAt
              ? new Date(session.createdAt).toLocaleDateString()
              : "N/A"}
          </p>
          {isPolling && (
            <div className="flex items-center text-xs text-green-600">
              <div className="animate-pulse h-2 w-2 bg-green-500 rounded-full mr-2"></div>
              Live updates active
            </div>
          )}
        </div>
      </div>

      <div className="grid grid-cols-1 gap-6 sm:grid-cols-3">
        <div className="bg-white overflow-hidden shadow rounded-lg">
          <div className="px-4 py-5 sm:p-6">
            <div className="flex items-center">
              <div className="flex-shrink-0 bg-indigo-500 rounded-md p-3">
                <ChartBarIcon className="h-6 w-6 text-white" />
              </div>
              <div className="ml-5 w-0 flex-1">
                <dl>
                  <dt className="text-sm font-medium text-gray-500 truncate">
                    Total Images
                  </dt>
                  <dd className="flex items-baseline">
                    <div className="text-2xl font-semibold text-gray-900">
                      {analysisJobs.length}
                    </div>
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div className="bg-white overflow-hidden shadow rounded-lg">
          <div className="px-4 py-5 sm:p-6">
            <div className="flex items-center">
              <div className="flex-shrink-0 bg-green-500 rounded-md p-3">
                <svg
                  className="h-6 w-6 text-white"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                  />
                </svg>
              </div>
              <div className="ml-5 w-0 flex-1">
                <dl>
                  <dt className="text-sm font-medium text-gray-500 truncate">
                    Completed
                  </dt>
                  <dd className="flex items-baseline">
                    <div className="text-2xl font-semibold text-gray-900">
                      {completedJobs.length}
                    </div>
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div className="bg-white overflow-hidden shadow rounded-lg">
          <div className="px-4 py-5 sm:p-6">
            <div className="flex items-center">
              <div className="flex-shrink-0 bg-yellow-500 rounded-md p-3">
                <svg
                  className="h-6 w-6 text-white"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                  />
                </svg>
              </div>
              <div className="ml-5 w-0 flex-1">
                <dl>
                  <dt className="text-sm font-medium text-gray-500 truncate">
                    Processing
                  </dt>
                  <dd className="flex items-baseline">
                    <div className="text-2xl font-semibold text-gray-900">
                      {pendingJobs.length}
                    </div>
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="bg-white shadow sm:rounded-lg">
        <div className="px-4 py-5 sm:px-6">
          <h3 className="text-lg leading-6 font-medium text-gray-900">
            Analysis Jobs
          </h3>
          <p className="mt-1 max-w-2xl text-sm text-gray-500">
            Individual image analysis results.
          </p>
        </div>
        <div className="border-t border-gray-200">
          <ul className="divide-y divide-gray-200">
            {analysisJobs.map((job) => (
              <li key={job.id}>
                <div className="block hover:bg-gray-50">
                  <div className="px-4 py-4 sm:px-6">
                    <div className="flex items-center justify-between">
                      <p className="text-sm font-medium text-indigo-600 truncate">
                        {job.area} • {job.id.substring(0, 8)}
                      </p>
                      <div className="ml-2 flex-shrink-0 flex">
                        <p
                          className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                            job.status === "Completed"
                              ? "bg-green-100 text-green-800"
                              : job.status === "Error"
                              ? "bg-red-100 text-red-800"
                              : "bg-yellow-100 text-yellow-800"
                          }`}
                        >
                          {job.status}
                        </p>
                      </div>
                    </div>
                    <div className="mt-2 sm:flex sm:justify-between">
                      <div className="sm:flex">
                        {job.result && (
                          <p className="flex items-center text-sm text-gray-500">
                            <ChartBarIcon className="flex-shrink-0 mr-1.5 h-5 w-5 text-gray-400" />
                            {job.result.totalHairs} hairs detected
                          </p>
                        )}
                      </div>
                      <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0">
                        <svg
                          className="flex-shrink-0 mr-1.5 h-5 w-5 text-gray-400"
                          fill="none"
                          viewBox="0 0 24 24"
                          stroke="currentColor"
                        >
                          <path
                            strokeLinecap="round"
                            strokeLinejoin="round"
                            strokeWidth={2}
                            d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                          />
                        </svg>
                        {job.createdAt
                          ? new Date(job.createdAt).toLocaleDateString()
                          : "N/A"}
                      </div>
                    </div>
                    {job.status === "Completed" &&
                      job.result?.annotatedImageUrl && (
                        <div className="mt-4">
                          <img
                            src={job.result.annotatedImageUrl}
                            alt="Annotated analysis result"
                            className="h-48 w-full object-contain rounded-md border border-gray-200"
                          />
                        </div>
                      )}
                    {job.status === "Error" && job.errorMessage && (
                      <div className="mt-2 text-sm text-red-600">
                        Error: {job.errorMessage}
                      </div>
                    )}
                  </div>
                </div>
              </li>
            ))}
          </ul>
        </div>
      </div>

      {session.finalReportData && (
        <div className="bg-white shadow sm:rounded-lg">
          <div className="px-4 py-5 sm:px-6">
            <div className="flex items-center justify-between">
              <h3 className="text-lg leading-6 font-medium text-gray-900">
                Final Report
              </h3>
              <button
                onClick={() =>
                  navigate(`/analysis/session/${sessionId}/report`)
                }
                className="inline-flex items-center px-3 py-2 border border-gray-300 shadow-sm text-sm leading-4 font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              >
                <DocumentTextIcon className="h-4 w-4 mr-1" />
                View Full Report
              </button>
            </div>
            <div className="mt-4 bg-gray-50 p-4 rounded-md">
              <p className="text-sm text-gray-600">
                The final report has been generated with aggregated metrics from
                all analysis jobs.
              </p>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
