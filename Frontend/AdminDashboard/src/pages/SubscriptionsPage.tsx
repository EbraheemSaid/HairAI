import React, { useState, useEffect } from "react";
import { DataTable } from "../components/ui/DataTable";
import { Button } from "../components/ui/Button";
import { PlusIcon, PencilIcon, TrashIcon } from "@heroicons/react/24/outline";
import {
  adminService,
  type AdminSubscriptionResponse,
} from "../services/adminService";
import { toast } from "react-hot-toast";

export const SubscriptionsPage: React.FC = () => {
  const [subscriptions, setSubscriptions] = useState<
    AdminSubscriptionResponse[]
  >([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchSubscriptions();
  }, []);

  const fetchSubscriptions = async () => {
    try {
      setLoading(true);
      const data = await adminService.getAllSubscriptions();
      setSubscriptions(data);
    } catch (error) {
      toast.error("Failed to load subscriptions");
      console.error("Error fetching subscriptions:", error);
    } finally {
      setLoading(false);
    }
  };

  const plans = [
    {
      id: "basic",
      name: "Basic",
      price: 1500,
      currency: "EGP",
      features: ["Up to 5 users", "100 analyses/month", "Email support"],
    },
    {
      id: "professional",
      name: "Professional",
      price: 2500,
      currency: "EGP",
      features: [
        "Up to 15 users",
        "500 analyses/month",
        "Priority email support",
      ],
    },
    {
      id: "enterprise",
      name: "Enterprise",
      price: 5000,
      currency: "EGP",
      features: ["Unlimited users", "Unlimited analyses", "24/7 phone support"],
    },
  ];

  const subscriptionColumns = [
    {
      key: "clinic",
      title: "Clinic",
      sortable: true,
      render: (value: any, row: AdminSubscriptionResponse) => row.clinic.name,
    },
    {
      key: "plan",
      title: "Plan",
      sortable: true,
      render: (value: any, row: AdminSubscriptionResponse) => row.plan.name,
    },
    {
      key: "status",
      title: "Status",
      render: (value: string) => (
        <span
          className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
            value === "Active"
              ? "bg-green-100 text-green-800"
              : "bg-red-100 text-red-800"
          }`}
        >
          {value}
        </span>
      ),
    },
    {
      key: "startDate",
      title: "Start Date",
      render: (value: string) => new Date(value).toLocaleDateString(),
    },
    {
      key: "endDate",
      title: "End Date",
      render: (value: string) => new Date(value).toLocaleDateString(),
    },
    {
      key: "plan",
      title: "Amount",
      render: (value: any, row: AdminSubscriptionResponse) =>
        `${row.plan.currency} ${row.plan.priceMonthly.toLocaleString()}`,
    },
    {
      key: "actions",
      title: "Actions",
      render: (_: any, row: AdminSubscriptionResponse) => (
        <div className="flex space-x-2">
          <button
            className="text-indigo-600 hover:text-indigo-900"
            onClick={() => toast.info("Edit functionality will be implemented")}
          >
            <PencilIcon className="h-5 w-5" />
          </button>
          <button
            className="text-red-600 hover:text-red-900"
            onClick={() =>
              adminService.cancelSubscription(row.id).then(() => {
                toast.success("Subscription cancelled");
                fetchSubscriptions();
              })
            }
          >
            <TrashIcon className="h-5 w-5" />
          </button>
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Subscriptions</h1>
          <p className="mt-1 text-sm text-gray-500">
            Manage clinic subscriptions and plans
          </p>
        </div>
        <div className="mt-4 md:mt-0">
          <Button>
            <PlusIcon className="-ml-1 mr-2 h-5 w-5" aria-hidden="true" />
            New Subscription
          </Button>
        </div>
      </div>

      <div className="bg-white shadow sm:rounded-lg">
        <div className="px-4 py-5 sm:p-6">
          <h2 className="text-lg font-medium text-gray-900 mb-4">
            Active Subscriptions
          </h2>
          <DataTable
            data={subscriptions}
            columns={subscriptionColumns}
            pagination={true}
            itemsPerPage={10}
            loading={loading}
          />
        </div>
      </div>

      <div className="bg-white shadow sm:rounded-lg">
        <div className="px-4 py-5 sm:p-6">
          <h2 className="text-lg font-medium text-gray-900 mb-4">
            Subscription Plans
          </h2>
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {plans.map((plan) => (
              <div
                key={plan.id}
                className="border border-gray-200 rounded-lg shadow-sm divide-y divide-gray-200"
              >
                <div className="p-6">
                  <h3 className="text-lg font-medium text-gray-900">
                    {plan.name}
                  </h3>
                  <div className="mt-4">
                    <p className="text-4xl font-extrabold text-gray-900">
                      {plan.currency} {plan.price.toLocaleString()}
                    </p>
                    <p className="mt-1 text-sm text-gray-500">per month</p>
                  </div>
                  <ul className="mt-6 space-y-4">
                    {plan.features.map((feature, index) => (
                      <li key={index} className="flex items-start">
                        <div className="flex-shrink-0">
                          <svg
                            className="h-6 w-6 text-green-500"
                            fill="none"
                            viewBox="0 0 24 24"
                            stroke="currentColor"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              strokeWidth={2}
                              d="M5 13l4 4L19 7"
                            />
                          </svg>
                        </div>
                        <p className="ml-3 text-sm text-gray-500">{feature}</p>
                      </li>
                    ))}
                  </ul>
                </div>
                <div className="py-4 px-6 bg-gray-50">
                  <Button className="w-full" variant="secondary">
                    <PencilIcon
                      className="-ml-1 mr-2 h-5 w-5"
                      aria-hidden="true"
                    />
                    Edit Plan
                  </Button>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};
