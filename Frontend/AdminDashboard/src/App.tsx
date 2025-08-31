import React, { useEffect } from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import { Toaster } from "react-hot-toast";
import { AdminLayout } from "./layouts/AdminLayout";
import { ErrorBoundary } from "./components/ErrorBoundary";
import { AdminDashboardPage } from "./pages/AdminDashboardPage";
import { ClinicsPage } from "./pages/ClinicsPage";
import { SubscriptionsPage } from "./pages/SubscriptionsPage";
import { UsersPage } from "./pages/UsersPage";
import { WorkingLoginPage } from "./pages/auth/WorkingLoginPage";
import { useAuthStore } from "./store/safeAuthStore";

function App() {
  const { isAuthenticated, isLoading, initialize } = useAuthStore();

  useEffect(() => {
    try {
      initialize();
    } catch (error) {
      console.error("Auth initialization failed:", error);
    }
  }, [initialize]);

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>
    );
  }

  return (
    <ErrorBoundary>
      <Router>
        <Routes>
          <Route path="/login" element={<WorkingLoginPage />} />
          <Route
            path="/*"
            element={
              isAuthenticated ? (
                <AdminLayout>
                  <Routes>
                    <Route
                      index
                      element={<Navigate to="/dashboard" replace />}
                    />
                    <Route path="dashboard" element={<AdminDashboardPage />} />
                    <Route path="clinics" element={<ClinicsPage />} />
                    <Route
                      path="subscriptions"
                      element={<SubscriptionsPage />}
                    />
                    <Route path="users" element={<UsersPage />} />
                    <Route
                      path="*"
                      element={<Navigate to="/dashboard" replace />}
                    />
                  </Routes>
                </AdminLayout>
              ) : (
                <Navigate to="/login" replace />
              )
            }
          />
        </Routes>
      </Router>
      <Toaster
        position="top-right"
        toastOptions={{
          duration: 4000,
          style: {
            background: "#363636",
            color: "#fff",
          },
          success: {
            duration: 3000,
            style: {
              background: "#10B981",
            },
          },
          error: {
            duration: 5000,
            style: {
              background: "#EF4444",
            },
          },
        }}
      />
    </ErrorBoundary>
  );
}

export default App;
