import React, { useState, useEffect } from "react";
import {
  CheckIcon,
  XMarkIcon,
  CreditCardIcon,
  DocumentTextIcon,
} from "@heroicons/react/24/outline";
import { useAuthStore } from "../../store/authStore";
import { subscriptionService } from "../../services/subscriptionService";
import { clinicService } from "../../services/clinicService";
import type { Subscription, Clinic } from "../../types";
import { toast } from "react-hot-toast";

interface SubscriptionPlan {
  id: string;
  name: string;
  priceMonthly: number;
  currency: string;
  maxUsers: number;
  maxAnalysesPerMonth: number;
  features: string[];
}

export const SubscriptionPage: React.FC = () => {
  const { user } = useAuthStore();
  const [clinic, setClinic] = useState<Clinic | null>(null);
  const [subscription, setSubscription] = useState<Subscription | null>(null);
  const [availablePlans, setAvailablePlans] = useState<SubscriptionPlan[]>([]);
  const [loading, setLoading] = useState(true);
  const [showPlans, setShowPlans] = useState(false);

  useEffect(() => {
    if (user?.clinicId) {
      fetchSubscriptionData();
    }
  }, [user]);

  const fetchSubscriptionData = async () => {
    try {
      setLoading(true);
      const [clinicData, subscriptionData, plansData] = await Promise.all([
        clinicService.getClinicById(user!.clinicId!),
        subscriptionService.getSubscriptionForClinic(user!.clinicId!),
        subscriptionService.getSubscriptionPlans(),
      ]);

      setClinic(clinicData);
      setSubscription(subscriptionData);
      setAvailablePlans(plansData);
    } catch (error) {
      toast.error("Failed to load subscription data");
      console.error("Error fetching subscription data:", error);
    } finally {
      setLoading(false);
    }
  };

  const getStatusBadge = (status: string) => {
    switch (status) {
      case "Active":
        return (
          <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
            <CheckIcon className="h-3 w-3 mr-1" />
            Active
          </span>
        );
      case "Expired":
        return (
          <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">
            <XMarkIcon className="h-3 w-3 mr-1" />
            Expired
          </span>
        );
      case "Cancelled":
        return (
          <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
            <XMarkIcon className="h-3 w-3 mr-1" />
            Cancelled
          </span>
        );
      default:
        return (
          <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-yellow-100 text-yellow-800">
            {status}
          </span>
        );
    }
  };

  const handleUpgrade = async (planId: string) => {
    try {
      await subscriptionService.createSubscription({
        clinicId: user!.clinicId!,
        planId,
        paymentMethodId: "pm_placeholder", // In real implementation, would collect payment method
      });
      toast.success("Subscription updated successfully");
      fetchSubscriptionData();
      setShowPlans(false);
    } catch (error) {
      toast.error("Failed to update subscription");
      console.error("Error updating subscription:", error);
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>
    );
  }

  // Only show this page to ClinicAdmins
  if (user?.role !== "ClinicAdmin" && user?.role !== "SuperAdmin") {
    return (
      <div className="text-center py-12">
        <h3 className="mt-2 text-sm font-medium text-gray-900">
          Access Denied
        </h3>
        <p className="mt-1 text-sm text-gray-500">
          You don't have permission to view subscription details.
        </p>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Subscription</h1>
        <p className="mt-1 text-sm text-gray-500">
          Manage your clinic's subscription and billing
        </p>
      </div>

      {/* Current Subscription */}
      <div className="bg-white shadow sm:rounded-lg">
        <div className="px-4 py-5 sm:p-6">
          <h3 className="text-lg leading-6 font-medium text-gray-900">
            Current Subscription
          </h3>

          {subscription ? (
            <div className="mt-5">
              <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-3">
                <div className="bg-gray-50 rounded-lg p-4">
                  <div className="flex items-center">
                    <div className="flex-shrink-0">
                      <CreditCardIcon className="h-6 w-6 text-indigo-600" />
                    </div>
                    <div className="ml-3">
                      <div className="text-sm font-medium text-gray-900">
                        Plan
                      </div>
                      <div className="text-sm text-gray-500">
                        {subscription.plan}
                      </div>
                    </div>
                  </div>
                </div>

                <div className="bg-gray-50 rounded-lg p-4">
                  <div className="flex items-center">
                    <div className="flex-shrink-0">
                      <div className="h-6 w-6 flex items-center justify-center">
                        {getStatusBadge(subscription.status)}
                      </div>
                    </div>
                    <div className="ml-3">
                      <div className="text-sm font-medium text-gray-900">
                        Status
                      </div>
                    </div>
                  </div>
                </div>

                <div className="bg-gray-50 rounded-lg p-4">
                  <div className="flex items-center">
                    <div className="flex-shrink-0">
                      <DocumentTextIcon className="h-6 w-6 text-indigo-600" />
                    </div>
                    <div className="ml-3">
                      <div className="text-sm font-medium text-gray-900">
                        Billing Period
                      </div>
                      <div className="text-sm text-gray-500">
                        {subscription.startDate
                          ? `${new Date(
                              subscription.startDate
                            ).toLocaleDateString()} - ${new Date(
                              subscription.endDate
                            ).toLocaleDateString()}`
                          : "N/A"}
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <div className="mt-5">
                <button
                  onClick={() => setShowPlans(!showPlans)}
                  className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  {showPlans ? "Hide Plans" : "Change Plan"}
                </button>
              </div>
            </div>
          ) : (
            <div className="mt-5">
              <div className="text-center">
                <svg
                  className="mx-auto h-12 w-12 text-gray-400"
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
                <h3 className="mt-2 text-sm font-medium text-gray-900">
                  No Active Subscription
                </h3>
                <p className="mt-1 text-sm text-gray-500">
                  Get started by choosing a subscription plan.
                </p>
                <div className="mt-6">
                  <button
                    onClick={() => setShowPlans(true)}
                    className="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                  >
                    View Plans
                  </button>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* Available Plans */}
      {showPlans && (
        <div className="bg-white shadow sm:rounded-lg">
          <div className="px-4 py-5 sm:p-6">
            <h3 className="text-lg leading-6 font-medium text-gray-900">
              Available Plans
            </h3>
            <div className="mt-5 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
              {availablePlans.map((plan) => (
                <div
                  key={plan.id}
                  className="border border-gray-200 rounded-lg p-6 hover:border-indigo-500 transition-colors"
                >
                  <div className="text-center">
                    <h4 className="text-lg font-medium text-gray-900">
                      {plan.name}
                    </h4>
                    <div className="mt-4">
                      <span className="text-4xl font-bold text-gray-900">
                        {plan.priceMonthly}
                      </span>
                      <span className="text-base font-medium text-gray-500">
                        /{plan.currency} per month
                      </span>
                    </div>
                  </div>

                  <ul className="mt-6 space-y-3">
                    <li className="flex items-center">
                      <CheckIcon className="h-5 w-5 text-green-500 mr-2" />
                      <span className="text-sm text-gray-600">
                        Up to {plan.maxUsers} users
                      </span>
                    </li>
                    <li className="flex items-center">
                      <CheckIcon className="h-5 w-5 text-green-500 mr-2" />
                      <span className="text-sm text-gray-600">
                        {plan.maxAnalysesPerMonth} analyses per month
                      </span>
                    </li>
                    {plan.features?.map((feature, index) => (
                      <li key={index} className="flex items-center">
                        <CheckIcon className="h-5 w-5 text-green-500 mr-2" />
                        <span className="text-sm text-gray-600">{feature}</span>
                      </li>
                    ))}
                  </ul>

                  <div className="mt-6">
                    <button
                      onClick={() => handleUpgrade(plan.id)}
                      className={`w-full inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md focus:outline-none focus:ring-2 focus:ring-offset-2 ${
                        subscription?.plan === plan.name
                          ? "text-gray-500 bg-gray-100 cursor-not-allowed"
                          : "text-white bg-indigo-600 hover:bg-indigo-700 focus:ring-indigo-500"
                      }`}
                      disabled={subscription?.plan === plan.name}
                    >
                      {subscription?.plan === plan.name
                        ? "Current Plan"
                        : "Select Plan"}
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      )}

      {/* Billing History */}
      {subscription?.billingHistory &&
        subscription.billingHistory.length > 0 && (
          <div className="bg-white shadow sm:rounded-lg">
            <div className="px-4 py-5 sm:px-6">
              <h3 className="text-lg leading-6 font-medium text-gray-900">
                Billing History
              </h3>
            </div>
            <div className="border-t border-gray-200">
              <div className="overflow-hidden">
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Date
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Amount
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Payment Method
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Status
                      </th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {subscription.billingHistory.map((payment) => (
                      <tr key={payment.id}>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {new Date(payment.paymentDate).toLocaleDateString()}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {payment.amount} {payment.currency}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                          {payment.paymentMethod}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span
                            className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                              payment.status === "Paid"
                                ? "bg-green-100 text-green-800"
                                : payment.status === "Failed"
                                ? "bg-red-100 text-red-800"
                                : "bg-yellow-100 text-yellow-800"
                            }`}
                          >
                            {payment.status}
                          </span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        )}

      {/* Contact Support */}
      <div className="bg-blue-50 border border-blue-200 rounded-md p-4">
        <div className="flex">
          <div className="flex-shrink-0">
            <svg
              className="h-5 w-5 text-blue-400"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
              />
            </svg>
          </div>
          <div className="ml-3">
            <h3 className="text-sm font-medium text-blue-800">Need Help?</h3>
            <div className="mt-2 text-sm text-blue-700">
              <p>
                If you have questions about your subscription or billing, please
                contact our support team.
              </p>
              <div className="mt-2">
                <button
                  onClick={() =>
                    toast.info(
                      "Support contact feature would be implemented here"
                    )
                  }
                  className="text-blue-700 underline hover:text-blue-600"
                >
                  Contact Support
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
