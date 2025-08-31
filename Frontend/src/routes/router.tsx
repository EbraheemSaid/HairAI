import React from "react";
import { createBrowserRouter, Navigate, Outlet } from "react-router-dom";
import { useAuthStore } from "../store/authStore";
import { MainLayout } from "../layouts/MainLayout";
import { LoginPage } from "../pages/auth/LoginPage";
import { RegisterPage } from "../pages/auth/RegisterPage";
import { AcceptInvitationPage } from "../pages/auth/AcceptInvitationPage";
import { DashboardPage } from "../pages/dashboard/DashboardPage";
import { PatientsPage } from "../pages/patients/PatientsPage";
import { NewPatientPage } from "../pages/patients/NewPatientPage";
import { PatientDetailPage } from "../pages/patients/PatientDetailPage";
import { EditPatientPage } from "../pages/patients/EditPatientPage";
import { NewAnalysisPage } from "../pages/analysis/NewAnalysisPage";
import { AnalysisSessionPage } from "../pages/analysis/AnalysisSessionPage";
import { AnalysisReportPage } from "../pages/analysis/AnalysisReportPage";
import { CalibrationProfilesPage } from "../pages/settings/CalibrationProfilesPage";
import { ClinicSettingsPage } from "../pages/settings/ClinicSettingsPage";
import { UsersPage } from "../pages/settings/UsersPage";
import { SubscriptionPage } from "../pages/settings/SubscriptionPage";
import { AdminClinicsPage } from "../pages/admin/AdminClinicsPage";
import { AdminSubscriptionsPage } from "../pages/admin/AdminSubscriptionsPage";

// Protected route wrapper
const ProtectedRoute: React.FC<{
  children: React.ReactNode;
  allowedRoles?: string[];
}> = ({ children }) => {
  const { isAuthenticated } = useAuthStore();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

// Role-based route wrapper for SuperAdmin
const SuperAdminRoute: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const { user } = useAuthStore();

  if (user?.role !== "SuperAdmin") {
    return <Navigate to="/dashboard" replace />;
  }

  return <>{children}</>;
};

// Role-based route wrapper for ClinicAdmin
const ClinicAdminRoute: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const { user } = useAuthStore();

  if (user?.role !== "ClinicAdmin" && user?.role !== "SuperAdmin") {
    return <Navigate to="/dashboard" replace />;
  }

  return <>{children}</>;
};

export const router = createBrowserRouter([
  {
    path: "/",
    element: (
      <ProtectedRoute>
        <MainLayout />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <Navigate to="/dashboard" replace /> },
      { path: "dashboard", element: <DashboardPage /> },

      // Patient management
      { path: "patients", element: <PatientsPage /> },
      { path: "patients/new", element: <NewPatientPage /> },
      { path: "patients/:patientId", element: <PatientDetailPage /> },
      { path: "patients/:patientId/edit", element: <EditPatientPage /> },

      // Analysis workflow
      { path: "analysis/new", element: <NewAnalysisPage /> },
      { path: "analysis/session/:sessionId", element: <AnalysisSessionPage /> },
      {
        path: "analysis/session/:sessionId/report",
        element: <AnalysisReportPage />,
      },

      // ClinicAdmin only routes
      {
        path: "settings",
        element: (
          <ClinicAdminRoute>
            <Outlet />
          </ClinicAdminRoute>
        ),
        children: [
          { path: "calibration", element: <CalibrationProfilesPage /> },
          { path: "clinic", element: <ClinicSettingsPage /> },
          { path: "users", element: <UsersPage /> },
          { path: "subscription", element: <SubscriptionPage /> },
        ],
      },

      // SuperAdmin only routes
      {
        path: "admin",
        element: (
          <SuperAdminRoute>
            <Outlet />
          </SuperAdminRoute>
        ),
        children: [
          { path: "clinics", element: <AdminClinicsPage /> },
          { path: "subscriptions", element: <AdminSubscriptionsPage /> },
        ],
      },
    ],
  },
  {
    path: "/login",
    element: <LoginPage />,
  },
  {
    path: "/register",
    element: <RegisterPage />,
  },
  {
    path: "/invitations/accept",
    element: <AcceptInvitationPage />,
  },
  {
    path: "*",
    element: <Navigate to="/dashboard" replace />,
  },
]);
