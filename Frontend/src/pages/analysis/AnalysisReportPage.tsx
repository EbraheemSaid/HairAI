import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  ArrowLeftIcon,
  PrinterIcon,
  DocumentArrowDownIcon,
} from "@heroicons/react/24/outline";
import { analysisService } from "../../services/analysisService";
import { patientService } from "../../services/patientService";
import type { AnalysisSession, Patient } from "../../types";
import { toast } from "react-hot-toast";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
} from "recharts";

export const AnalysisReportPage: React.FC = () => {
  const { sessionId } = useParams<{ sessionId: string }>();
  const navigate = useNavigate();
  const [session, setSession] = useState<AnalysisSession | null>(null);
  const [patient, setPatient] = useState<Patient | null>(null);
  const [loading, setLoading] = useState(true);

  // Mock data for charts - in a real app, this would come from the API
  const hairDensityData = [
    { name: "Crown", density: 85 },
    { name: "Frontal", density: 120 },
    { name: "Temporal L", density: 95 },
    { name: "Temporal R", density: 92 },
    { name: "Vertex", density: 78 },
  ];

  const follicularUnitData = [
    { name: "1 Hair", value: 25 },
    { name: "2 Hairs", value: 45 },
    { name: "3 Hairs", value: 20 },
    { name: "4+ Hairs", value: 10 },
  ];

  const COLORS = ["#0088FE", "#00C49F", "#FFBB28", "#FF8042"];

  useEffect(() => {
    if (sessionId) {
      fetchReportData();
    }
  }, [sessionId]);

  const fetchReportData = async () => {
    try {
      setLoading(true);
      const sessionData = await analysisService.getAnalysisSession(sessionId!);
      setSession(sessionData);

      if (sessionData.patientId) {
        const patientData = await patientService.getPatientById(
          sessionData.patientId
        );
        setPatient(patientData);
      }
    } catch (error) {
      toast.error("Failed to load report data");
      console.error("Error fetching report data:", error);
    } finally {
      setLoading(false);
    }
  };

  const handlePrint = () => {
    window.print();
  };

  const handleDownload = () => {
    toast.info("Report download functionality would be implemented here");
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
          Report not found
        </h3>
        <p className="mt-1 text-sm text-gray-500">
          The analysis report you're looking for doesn't exist.
        </p>
        <div className="mt-6">
          <button
            type="button"
            onClick={() => navigate(`/analysis/session/${sessionId}`)}
            className="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            <ArrowLeftIcon className="-ml-1 mr-2 h-5 w-5" aria-hidden="true" />
            Back to Session
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6 print:space-y-4">
      {/* Header - hidden when printing */}
      <div className="print:hidden">
        <div>
          <button
            onClick={() => navigate(`/analysis/session/${sessionId}`)}
            className="inline-flex items-center text-sm font-medium text-indigo-600 hover:text-indigo-500"
          >
            <ArrowLeftIcon className="h-5 w-5 mr-1" />
            Back to session
          </button>
          <div className="mt-2 flex items-center justify-between">
            <h1 className="text-2xl font-bold text-gray-900">
              Analysis Report
            </h1>
            <div className="flex space-x-3">
              <button
                onClick={handleDownload}
                className="inline-flex items-center px-3 py-2 border border-gray-300 shadow-sm text-sm leading-4 font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              >
                <DocumentArrowDownIcon className="h-4 w-4 mr-1" />
                Download
              </button>
              <button
                onClick={handlePrint}
                className="inline-flex items-center px-3 py-2 border border-gray-300 shadow-sm text-sm leading-4 font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              >
                <PrinterIcon className="h-4 w-4 mr-1" />
                Print
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Report Content */}
      <div className="bg-white shadow sm:rounded-lg print:shadow-none print:rounded-none">
        <div className="px-4 py-5 sm:px-6 print:px-4 print:py-4">
          <div className="print:text-center">
            <h1 className="text-2xl font-bold text-gray-900 print:text-xl">
              Hair Analysis Report
            </h1>
            <p className="mt-1 max-w-2xl text-sm text-gray-500 print:text-xs">
              Detailed analysis results for patient
            </p>
          </div>
        </div>
        <div className="border-t border-gray-200 print:border-none">
          <div className="px-4 py-5 sm:px-6 print:px-4 print:py-4">
            <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 print:grid-cols-2">
              <div>
                <h3 className="text-lg font-medium text-gray-900 print:text-base">
                  Patient Information
                </h3>
                <dl className="mt-4 space-y-3 print:space-y-2">
                  <div className="flex justify-between print:text-sm">
                    <dt className="text-gray-500">Name</dt>
                    <dd className="text-gray-900 font-medium">
                      {patient
                        ? `${patient.firstName} ${patient.lastName}`
                        : "N/A"}
                    </dd>
                  </div>
                  <div className="flex justify-between print:text-sm">
                    <dt className="text-gray-500">Patient ID</dt>
                    <dd className="text-gray-900">
                      {patient?.clinicPatientId || "N/A"}
                    </dd>
                  </div>
                  <div className="flex justify-between print:text-sm">
                    <dt className="text-gray-500">Date of Birth</dt>
                    <dd className="text-gray-900">
                      {patient?.dateOfBirth
                        ? new Date(patient.dateOfBirth).toLocaleDateString()
                        : "N/A"}
                    </dd>
                  </div>
                </dl>
              </div>

              <div>
                <h3 className="text-lg font-medium text-gray-900 print:text-base">
                  Session Information
                </h3>
                <dl className="mt-4 space-y-3 print:space-y-2">
                  <div className="flex justify-between print:text-sm">
                    <dt className="text-gray-500">Session ID</dt>
                    <dd className="text-gray-900 font-mono text-sm print:text-xs">
                      {session.id.substring(0, 8)}
                    </dd>
                  </div>
                  <div className="flex justify-between print:text-sm">
                    <dt className="text-gray-500">Analysis Date</dt>
                    <dd className="text-gray-900">
                      {session.createdAt
                        ? new Date(session.createdAt).toLocaleDateString()
                        : "N/A"}
                    </dd>
                  </div>
                  <div className="flex justify-between print:text-sm">
                    <dt className="text-gray-500">Status</dt>
                    <dd className="text-gray-900">
                      <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 print:text-xs">
                        {session.status}
                      </span>
                    </dd>
                  </div>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Summary Statistics */}
      <div className="bg-white shadow sm:rounded-lg print:shadow-none print:rounded-none">
        <div className="px-4 py-5 sm:px-6 print:px-4 print:py-4">
          <h3 className="text-lg font-medium text-gray-900 print:text-base">
            Summary Statistics
          </h3>
          <div className="mt-6 grid grid-cols-1 gap-5 sm:grid-cols-3 print:grid-cols-3">
            <div className="bg-gray-50 overflow-hidden shadow rounded-lg print:shadow-none print:rounded">
              <div className="px-4 py-5 sm:p-6 print:px-4 print:py-3">
                <div className="flex items-center">
                  <div className="flex-shrink-0 bg-indigo-500 rounded-md p-2 print:p-1">
                    <svg
                      className="h-5 w-5 text-white print:h-4 print:w-4"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z"
                      />
                    </svg>
                  </div>
                  <div className="ml-4 w-0 flex-1">
                    <dt className="text-sm font-medium text-gray-500 truncate print:text-xs">
                      Total Hairs
                    </dt>
                    <dd className="flex items-baseline">
                      <div className="text-lg font-semibold text-gray-900 print:text-base">
                        12,450
                      </div>
                    </dd>
                  </div>
                </div>
              </div>
            </div>

            <div className="bg-gray-50 overflow-hidden shadow rounded-lg print:shadow-none print:rounded">
              <div className="px-4 py-5 sm:p-6 print:px-4 print:py-3">
                <div className="flex items-center">
                  <div className="flex-shrink-0 bg-green-500 rounded-md p-2 print:p-1">
                    <svg
                      className="h-5 w-5 text-white print:h-4 print:w-4"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                      />
                    </svg>
                  </div>
                  <div className="ml-4 w-0 flex-1">
                    <dt className="text-sm font-medium text-gray-500 truncate print:text-xs">
                      Avg. Density
                    </dt>
                    <dd className="flex items-baseline">
                      <div className="text-lg font-semibold text-gray-900 print:text-base">
                        92 hairs/cm²
                      </div>
                    </dd>
                  </div>
                </div>
              </div>
            </div>

            <div className="bg-gray-50 overflow-hidden shadow rounded-lg print:shadow-none print:rounded">
              <div className="px-4 py-5 sm:p-6 print:px-4 print:py-3">
                <div className="flex items-center">
                  <div className="flex-shrink-0 bg-yellow-500 rounded-md p-2 print:p-1">
                    <svg
                      className="h-5 w-5 text-white print:h-4 print:w-4"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"
                      />
                    </svg>
                  </div>
                  <div className="ml-4 w-0 flex-1">
                    <dt className="text-sm font-medium text-gray-500 truncate print:text-xs">
                      Follicular Units
                    </dt>
                    <dd className="flex items-baseline">
                      <div className="text-lg font-semibold text-gray-900 print:text-base">
                        8,750
                      </div>
                    </dd>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Charts */}
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2 print:grid-cols-1">
        <div className="bg-white shadow sm:rounded-lg print:shadow-none print:rounded-none">
          <div className="px-4 py-5 sm:px-6 print:px-4 print:py-4">
            <h3 className="text-lg font-medium text-gray-900 print:text-base">
              Hair Density by Area
            </h3>
            <div className="mt-4 h-80 print:h-64">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart
                  data={hairDensityData}
                  margin={{
                    top: 5,
                    right: 30,
                    left: 20,
                    bottom: 5,
                  }}
                >
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="name" />
                  <YAxis
                    label={{
                      value: "Hairs/cm²",
                      angle: -90,
                      position: "insideLeft",
                    }}
                  />
                  <Tooltip />
                  <Legend />
                  <Bar dataKey="density" fill="#4f46e5" name="Hair Density" />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>

        <div className="bg-white shadow sm:rounded-lg print:shadow-none print:rounded-none">
          <div className="px-4 py-5 sm:px-6 print:px-4 print:py-4">
            <h3 className="text-lg font-medium text-gray-900 print:text-base">
              Follicular Unit Distribution
            </h3>
            <div className="mt-4 h-80 print:h-64">
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={follicularUnitData}
                    cx="50%"
                    cy="50%"
                    labelLine={false}
                    outerRadius={80}
                    fill="#8884d8"
                    dataKey="value"
                    label={({ name, percent }) =>
                      `${name}: ${(percent * 100).toFixed(0)}%`
                    }
                  >
                    {follicularUnitData.map((entry, index) => (
                      <Cell
                        key={`cell-${index}`}
                        fill={COLORS[index % COLORS.length]}
                      />
                    ))}
                  </Pie>
                  <Tooltip />
                  <Legend />
                </PieChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>
      </div>

      {/* Notes Section */}
      <div className="bg-white shadow sm:rounded-lg print:shadow-none print:rounded-none">
        <div className="px-4 py-5 sm:px-6 print:px-4 print:py-4">
          <h3 className="text-lg font-medium text-gray-900 print:text-base">
            Doctor's Notes
          </h3>
          <div className="mt-4">
            <div className="border border-gray-300 rounded-md p-4 min-h-32 print:min-h-24">
              <p className="text-gray-500 italic print:text-sm">
                No notes have been added to this analysis session.
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Footer - hidden when printing */}
      <div className="print:hidden text-center text-sm text-gray-500">
        <p>Generated by HairAI - Professional Hair Analysis Platform</p>
        <p className="mt-1">{new Date().toLocaleString()}</p>
      </div>
    </div>
  );
};
