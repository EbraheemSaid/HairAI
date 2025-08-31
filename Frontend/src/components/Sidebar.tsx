import React, { useState } from "react";
import { Link } from "react-router-dom";
import { useAuthStore } from "../store/authStore";
import {
  HomeIcon,
  UserGroupIcon,
  CogIcon,
  ChartBarIcon,
  ClipboardDocumentIcon,
  BuildingOfficeIcon,
} from "@heroicons/react/24/outline";

export const Sidebar: React.FC = () => {
  const { user } = useAuthStore();
  const [collapsed, setCollapsed] = useState(false);

  const toggleSidebar = () => {
    setCollapsed(!collapsed);
  };

  const getMenuItems = () => {
    const baseItems = [
      { name: "Dashboard", href: "/dashboard", icon: HomeIcon },
      { name: "Patients", href: "/patients", icon: UserGroupIcon },
    ];

    if (user?.role === "Doctor" || user?.role === "ClinicAdmin") {
      baseItems.push({
        name: "New Analysis",
        href: "/analysis/new",
        icon: ClipboardDocumentIcon,
      });
    }

    if (user?.role === "ClinicAdmin") {
      baseItems.push(
        { name: "Calibration", href: "/settings/calibration", icon: CogIcon },
        {
          name: "Clinic Settings",
          href: "/settings/clinic",
          icon: BuildingOfficeIcon,
        },
        { name: "Users", href: "/settings/users", icon: UserGroupIcon },
        {
          name: "Subscription",
          href: "/settings/subscription",
          icon: ChartBarIcon,
        }
      );
    }

    if (user?.role === "SuperAdmin") {
      baseItems.push(
        { name: "Clinics", href: "/admin/clinics", icon: BuildingOfficeIcon },
        {
          name: "Subscriptions",
          href: "/admin/subscriptions",
          icon: ChartBarIcon,
        }
      );
    }

    return baseItems;
  };

  const menuItems = getMenuItems();

  return (
    <div
      className={`bg-white shadow-md transition-all duration-300 ${
        collapsed ? "w-20" : "w-64"
      } flex flex-col`}
    >
      <div className="flex items-center justify-between p-4 border-b">
        {!collapsed && (
          <h1 className="text-xl font-bold text-indigo-600">HairAI</h1>
        )}
        <button
          onClick={toggleSidebar}
          className="text-gray-500 hover:text-gray-700 focus:outline-none"
        >
          {collapsed ? (
            <svg
              className="h-6 w-6"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M9 5l7 7-7 7"
              />
            </svg>
          ) : (
            <svg
              className="h-6 w-6"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M15 19l-7-7 7-7"
              />
            </svg>
          )}
        </button>
      </div>
      <nav className="flex-1 overflow-y-auto py-4">
        <ul>
          {menuItems.map((item) => (
            <li key={item.name}>
              <Link
                to={item.href}
                className="flex items-center px-4 py-3 text-gray-700 hover:bg-indigo-50 hover:text-indigo-600 transition-colors duration-200"
              >
                <item.icon className="h-6 w-6 flex-shrink-0" />
                {!collapsed && (
                  <span className="ml-3 font-medium">{item.name}</span>
                )}
              </Link>
            </li>
          ))}
        </ul>
      </nav>
    </div>
  );
};
