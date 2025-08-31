import React, { useState, useEffect } from "react";
import { MetricCard } from "../components/ui/MetricCard";
import { BarChart, PieChart } from "../components/ui/Chart";
import {
  BuildingOfficeIcon,
  CreditCardIcon,
  UserGroupIcon,
  CurrencyDollarIcon,
} from "@heroicons/react/24/outline";
import { adminService } from "../services/adminService";
import { toast } from "react-hot-toast";

export const AdminDashboardPage: React.FC = () => {
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // For now, just simulate loading - we'll use mock data
    setTimeout(() => setLoading(false), 1000);
  }, []);

  const [stats, setStats] = useState([
    {
      title: "Total Clinics",
      value: "24",
      description: "Active clinics",
      trend: "up" as const,
      trendValue: "12%",
      icon: <BuildingOfficeIcon className="h-6 w-6 text-white" />,
      iconColor: "indigo" as const,
    },
    {
      title: "Active Subscriptions",
      value: "18",
      description: "Currently active",
      trend: "up" as const,
      trendValue: "8%",
      icon: <CreditCardIcon className="h-6 w-6 text-white" />,
      iconColor: "green" as const,
    },
    {
      title: "Total Users",
      value: "142",
      description: "Across all clinics",
      trend: "up" as const,
      trendValue: "5%",
      icon: <UserGroupIcon className="h-6 w-6 text-white" />,
      iconColor: "blue" as const,
    },
    {
      title: "Monthly Revenue",
      value: "EGP 42,567",
      description: "This month",
      trend: "up" as const,
      trendValue: "15%",
      icon: <CurrencyDollarIcon className="h-6 w-6 text-white" />,
      iconColor: "yellow" as const,
    },
  ]);

  // Mock chart data
  const clinicGrowthData = [
    { name: "Jan", value: 12 },
    { name: "Feb", value: 19 },
    { name: "Mar", value: 15 },
    { name: "Apr", value: 22 },
    { name: "May", value: 18 },
    { name: "Jun", value: 25 },
  ];

  const subscriptionDistributionData = [
    { name: "Basic", value: 8 },
    { name: "Professional", value: 12 },
    { name: "Enterprise", value: 4 },
  ];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p className="mt-1 text-sm text-gray-500">
          Overview of platform metrics and activity
        </p>
      </div>

      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
        {stats.map((stat, index) => (
          <MetricCard key={index} {...stat} />
        ))}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <BarChart
          title="Clinic Growth"
          data={clinicGrowthData}
          xAxisKey="name"
          yAxisKey="value"
          height={300}
        />

        <PieChart
          title="Subscription Distribution"
          data={subscriptionDistributionData}
          dataKey="value"
          nameKey="name"
          height={300}
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white shadow rounded-lg p-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">
            Recent Activity
          </h3>
          <div className="flow-root">
            <ul className="-mb-8">
              {[1, 2, 3, 4, 5].map((item) => (
                <li key={item}>
                  <div className="relative pb-8">
                    {item !== 5 && (
                      <span
                        className="absolute top-4 left-4 -ml-px h-full w-0.5 bg-gray-200"
                        aria-hidden="true"
                      />
                    )}
                    <div className="relative flex space-x-3">
                      <div>
                        <span className="h-8 w-8 rounded-full bg-indigo-500 flex items-center justify-center ring-8 ring-white">
                          <BuildingOfficeIcon
                            className="h-5 w-5 text-white"
                            aria-hidden="true"
                          />
                        </span>
                      </div>
                      <div className="min-w-0 flex-1 pt-1.5 flex justify-between space-x-4">
                        <div>
                          <p className="text-sm text-gray-500">
                            Clinic{" "}
                            <span className="font-medium text-gray-900">
                              Dermatology Center
                            </span>{" "}
                            was created
                          </p>
                        </div>
                        <div className="text-right text-sm whitespace-nowrap text-gray-500">
                          <time dateTime="2020-01-07">Jan 7</time>
                        </div>
                      </div>
                    </div>
                  </div>
                </li>
              ))}
            </ul>
          </div>
        </div>

        <div className="bg-white shadow rounded-lg p-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">
            System Status
          </h3>
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <span className="text-sm font-medium text-gray-500">
                API Server
              </span>
              <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                Online
              </span>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-sm font-medium text-gray-500">
                Database
              </span>
              <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                Online
              </span>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-sm font-medium text-gray-500">
                Message Queue
              </span>
              <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                Online
              </span>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-sm font-medium text-gray-500">Storage</span>
              <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                Online
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
