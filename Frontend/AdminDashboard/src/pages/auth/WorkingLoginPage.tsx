import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "../../store/safeAuthStore";

export const WorkingLoginPage: React.FC = () => {
  const [email, setEmail] = useState("admin@hairai.com");
  const [password, setPassword] = useState("SuperAdmin123!");
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState("");
  const navigate = useNavigate();
  const { login } = useAuthStore();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setMessage("");

    // Validate inputs before making API call
    if (!email || !password) {
      setMessage("Error: Email and password are required");
      setLoading(false);
      return;
    }

    try {
      console.log("Starting login...");

      const response = await fetch("http://localhost:5000/api/auth/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ email, password }),
      });

      console.log("Response received:", response.status);

      const response_data = await response.json();

      // The backend wraps responses in { success, message, data, errors }
      const data = response_data.data || response_data; // Fallback for direct responses

      if (response.ok && (response_data.success || data.success)) {
        console.log("Login successful, saving data...");

        try {
          // Transform backend response to expected user object format
          const userObject = {
            id: data.userId,
            firstName: data.firstName,
            lastName: data.lastName,
            email: email, // Use the email from form since backend doesn't return it
            role: "SuperAdmin", // Backend doesn't return role directly, but we know it's SuperAdmin for admin login
            clinicId: data.clinicId || undefined,
          };

          // Use the auth store to handle login
          login(userObject, data.token);
          setMessage("Success! Redirecting...");

          // Use React Router navigation
          setTimeout(() => {
            navigate("/dashboard", { replace: true });
          }, 100);
        } catch (error) {
          console.error("Auth store login failed:", error);
          setMessage(
            "Error: " +
              (error instanceof Error ? error.message : "Authentication failed")
          );
        }
      } else {
        console.log("Login failed:", response_data.message || data.message);
        setMessage(
          "Error: " + (response_data.message || data.message || "Login failed")
        );
      }
    } catch (error) {
      console.error("Network error:", error);
      setMessage("Network error. Is the backend running?");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-full flex flex-col justify-center py-12 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <h1 className="text-3xl font-bold text-center text-indigo-600">
          HairAI Admin
        </h1>
        <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900">
          Sign in to your account
        </h2>
      </div>

      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
          <form className="space-y-6" onSubmit={handleSubmit}>
            <div>
              <label
                htmlFor="email"
                className="block text-sm font-medium text-gray-700"
              >
                Email address
              </label>
              <div className="mt-1">
                <input
                  id="email"
                  name="email"
                  type="email"
                  autoComplete="email"
                  required
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md placeholder-gray-400 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                />
              </div>
            </div>

            <div>
              <label
                htmlFor="password"
                className="block text-sm font-medium text-gray-700"
              >
                Password
              </label>
              <div className="mt-1">
                <input
                  id="password"
                  name="password"
                  type="password"
                  autoComplete="current-password"
                  required
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md placeholder-gray-400 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                />
              </div>
            </div>

            {message && (
              <div
                className={`border rounded-md p-4 ${
                  message.includes("Success") || message.includes("Redirecting")
                    ? "bg-green-50 border-green-200"
                    : "bg-red-50 border-red-200"
                }`}
              >
                <p
                  className={`text-sm ${
                    message.includes("Success") ||
                    message.includes("Redirecting")
                      ? "text-green-600"
                      : "text-red-600"
                  }`}
                >
                  {message}
                </p>
              </div>
            )}

            <div>
              <button
                type="submit"
                disabled={loading}
                className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
              >
                {loading ? "Signing in..." : "Sign in"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};
