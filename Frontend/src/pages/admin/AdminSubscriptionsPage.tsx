import React, { useState, useEffect } from 'react';
import { PlusIcon, PencilIcon, CurrencyDollarIcon } from '@heroicons/react/24/outline';
import { adminService } from '../../services/adminService';
import type { SubscriptionPlan } from '../../services/adminService';
import { toast } from 'react-hot-toast';

export const AdminSubscriptionsPage: React.FC = () => {
  const [plans, setPlans] = useState<SubscriptionPlan[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [editingPlan, setEditingPlan] = useState<string | null>(null);
  const [creating, setCreating] = useState(false);
  const [updating, setUpdating] = useState(false);

  const [formData, setFormData] = useState({
    name: '',
    priceMonthly: '',
    currency: 'EGP',
    maxUsers: '',
    maxAnalysesPerMonth: '',
  });

  useEffect(() => {
    fetchSubscriptionPlans();
  }, []);

  const fetchSubscriptionPlans = async () => {
    try {
      setLoading(true);
      const data = await adminService.getAllSubscriptionPlans();
      setPlans(data);
    } catch (error) {
      toast.error('Failed to load subscription plans');
      console.error('Error fetching subscription plans:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setCreating(true);

    try {
      const requestData = {
        name: formData.name,
        priceMonthly: parseFloat(formData.priceMonthly),
        currency: formData.currency,
        maxUsers: parseInt(formData.maxUsers),
        maxAnalysesPerMonth: parseInt(formData.maxAnalysesPerMonth),
      };

      await adminService.createSubscriptionPlan(requestData);
      toast.success('Subscription plan created successfully');
      setShowCreateForm(false);
      setFormData({
        name: '',
        priceMonthly: '',
        currency: 'EGP',
        maxUsers: '',
        maxAnalysesPerMonth: '',
      });
      fetchSubscriptionPlans();
    } catch (error) {
      toast.error('Failed to create subscription plan');
      console.error('Error creating subscription plan:', error);
    } finally {
      setCreating(false);
    }
  };

  const handleUpdate = async (planId: string) => {
    setUpdating(true);

    try {
      const plan = plans.find(p => p.id === planId);
      if (!plan) return;

      const requestData = {
        name: formData.name || plan.name,
        priceMonthly: formData.priceMonthly ? parseFloat(formData.priceMonthly) : plan.priceMonthly,
        currency: formData.currency || plan.currency,
        maxUsers: formData.maxUsers ? parseInt(formData.maxUsers) : plan.maxUsers,
        maxAnalysesPerMonth: formData.maxAnalysesPerMonth 
          ? parseInt(formData.maxAnalysesPerMonth) 
          : plan.maxAnalysesPerMonth,
      };

      await adminService.updateSubscriptionPlan(planId, requestData);
      toast.success('Subscription plan updated successfully');
      setEditingPlan(null);
      setFormData({
        name: '',
        priceMonthly: '',
        currency: 'EGP',
        maxUsers: '',
        maxAnalysesPerMonth: '',
      });
      fetchSubscriptionPlans();
    } catch (error) {
      toast.error('Failed to update subscription plan');
      console.error('Error updating subscription plan:', error);
    } finally {
      setUpdating(false);
    }
  };

  const startEditing = (plan: SubscriptionPlan) => {
    setEditingPlan(plan.id);
    setFormData({
      name: plan.name,
      priceMonthly: plan.priceMonthly.toString(),
      currency: plan.currency,
      maxUsers: plan.maxUsers.toString(),
      maxAnalysesPerMonth: plan.maxAnalysesPerMonth.toString(),
    });
  };

  const cancelEditing = () => {
    setEditingPlan(null);
    setFormData({
      name: '',
      priceMonthly: '',
      currency: 'EGP',
      maxUsers: '',
      maxAnalysesPerMonth: '',
    });
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Subscription Plans</h1>
          <p className="mt-1 text-sm text-gray-500">
            Manage subscription plans for clinics
          </p>
        </div>
        <div className="mt-4 md:mt-0">
          <button
            onClick={() => setShowCreateForm(true)}
            className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            <PlusIcon className="-ml-1 mr-2 h-5 w-5" aria-hidden="true" />
            New Plan
          </button>
        </div>
      </div>

      {showCreateForm && (
        <div className="bg-white shadow sm:rounded-lg">
          <div className="px-4 py-5 sm:p-6">
            <div className="flex items-center justify-between">
              <h3 className="text-lg leading-6 font-medium text-gray-900">Create New Subscription Plan</h3>
              <button
                onClick={() => setShowCreateForm(false)}
                className="text-gray-400 hover:text-gray-500"
              >
                <XMarkIcon className="h-6 w-6" />
              </button>
            </div>
            <form onSubmit={handleCreate} className="mt-4 space-y-4">
              <div className="grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-6">
                <div className="sm:col-span-3">
                  <label htmlFor="name" className="block text-sm font-medium text-gray-700">
                    Plan Name
                  </label>
                  <input
                    type="text"
                    name="name"
                    id="name"
                    required
                    value={formData.name}
                    onChange={(e) => setFormData({...formData, name: e.target.value})}
                    className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  />
                </div>
                <div className="sm:col-span-3">
                  <label htmlFor="priceMonthly" className="block text-sm font-medium text-gray-700">
                    Monthly Price (EGP)
                  </label>
                  <input
                    type="number"
                    name="priceMonthly"
                    id="priceMonthly"
                    required
                    min="0"
                    step="0.01"
                    value={formData.priceMonthly}
                    onChange={(e) => setFormData({...formData, priceMonthly: e.target.value})}
                    className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  />
                </div>
                <div className="sm:col-span-2">
                  <label htmlFor="maxUsers" className="block text-sm font-medium text-gray-700">
                    Max Users
                  </label>
                  <input
                    type="number"
                    name="maxUsers"
                    id="maxUsers"
                    required
                    min="1"
                    value={formData.maxUsers}
                    onChange={(e) => setFormData({...formData, maxUsers: e.target.value})}
                    className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  />
                </div>
                <div className="sm:col-span-2">
                  <label htmlFor="maxAnalysesPerMonth" className="block text-sm font-medium text-gray-700">
                    Max Analyses/Month
                  </label>
                  <input
                    type="number"
                    name="maxAnalysesPerMonth"
                    id="maxAnalysesPerMonth"
                    required
                    min="0"
                    value={formData.maxAnalysesPerMonth}
                    onChange={(e) => setFormData({...formData, maxAnalysesPerMonth: e.target.value})}
                    className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  />
                </div>
                <div className="sm:col-span-2">
                  <label htmlFor="currency" className="block text-sm font-medium text-gray-700">
                    Currency
                  </label>
                  <select
                    id="currency"
                    name="currency"
                    required
                    value={formData.currency}
                    onChange={(e) => setFormData({...formData, currency: e.target.value})}
                    className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  >
                    <option value="EGP">EGP</option>
                    <option value="USD">USD</option>
                    <option value="EUR">EUR</option>
                  </select>
                </div>
              </div>
              <div className="flex justify-end space-x-3">
                <button
                  type="button"
                  onClick={() => setShowCreateForm(false)}
                  className="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={creating}
                  className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
                >
                  {creating ? (
                    <>
                      <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                      </svg>
                      Creating...
                    </>
                  ) : (
                    'Create Plan'
                  )}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      <div className="flex flex-col">
        <div className="-my-2 overflow-x-auto sm:-mx-6 lg:-mx-8">
          <div className="py-2 align-middle inline-block min-w-full sm:px-6 lg:px-8">
            <div className="shadow overflow-hidden border-b border-gray-200 sm:rounded-lg">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th
                      scope="col"
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                    >
                      Plan Name
                    </th>
                    <th
                      scope="col"
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                    >
                      Price
                    </th>
                    <th
                      scope="col"
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                    >
                      Features
                    </th>
                    <th scope="col" className="relative px-6 py-3">
                      <span className="sr-only">Actions</span>
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {plans.length === 0 ? (
                    <tr>
                      <td colSpan={4} className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-center">
                        No subscription plans found
                      </td>
                    </tr>
                  ) : (
                    plans.map((plan) => (
                      <tr key={plan.id} className="hover:bg-gray-50">
                        <td className="px-6 py-4 whitespace-nowrap">
                          {editingPlan === plan.id ? (
                            <input
                              type="text"
                              value={formData.name}
                              onChange={(e) => setFormData({...formData, name: e.target.value})}
                              className="block w-full border border-gray-300 rounded-md shadow-sm py-1 px-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                            />
                          ) : (
                            <div className="text-sm font-medium text-gray-900">{plan.name}</div>
                          )}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          {editingPlan === plan.id ? (
                            <div className="flex">
                              <select
                                value={formData.currency}
                                onChange={(e) => setFormData({...formData, currency: e.target.value})}
                                className="border border-gray-300 rounded-l-md shadow-sm py-1 px-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                              >
                                <option value="EGP">EGP</option>
                                <option value="USD">USD</option>
                                <option value="EUR">EUR</option>
                              </select>
                              <input
                                type="number"
                                step="0.01"
                                min="0"
                                value={formData.priceMonthly}
                                onChange={(e) => setFormData({...formData, priceMonthly: e.target.value})}
                                className="block w-full border border-gray-300 rounded-r-md shadow-sm py-1 px-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                              />
                            </div>
                          ) : (
                            <div className="text-sm text-gray-900">
                              {plan.currency} {plan.priceMonthly.toFixed(2)}/month
                            </div>
                          )}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          {editingPlan === plan.id ? (
                            <div className="space-y-2">
                              <div className="flex items-center">
                                <label className="text-xs text-gray-500 mr-1">Users:</label>
                                <input
                                  type="number"
                                  min="1"
                                  value={formData.maxUsers}
                                  onChange={(e) => setFormData({...formData, maxUsers: e.target.value})}
                                  className="w-16 border border-gray-300 rounded-md shadow-sm py-1 px-2 text-xs focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                                />
                              </div>
                              <div className="flex items-center">
                                <label className="text-xs text-gray-500 mr-1">Analyses:</label>
                                <input
                                  type="number"
                                  min="0"
                                  value={formData.maxAnalysesPerMonth}
                                  onChange={(e) => setFormData({...formData, maxAnalysesPerMonth: e.target.value})}
                                  className="w-16 border border-gray-300 rounded-md shadow-sm py-1 px-2 text-xs focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                                />
                              </div>
                            </div>
                          ) : (
                            <div className="text-sm text-gray-900">
                              <div>Up to {plan.maxUsers} users</div>
                              <div>{plan.maxAnalysesPerMonth} analyses/month</div>
                            </div>
                          )}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                          {editingPlan === plan.id ? (
                            <div className="flex space-x-2">
                              <button
                                onClick={() => handleUpdate(plan.id)}
                                disabled={updating}
                                className="text-green-600 hover:text-green-900"
                              >
                                {updating ? (
                                  <svg className="animate-spin h-5 w-5 text-green-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                  </svg>
                                ) : (
                                  <CheckIcon className="h-5 w-5" />
                                )}
                              </button>
                              <button
                                onClick={cancelEditing}
                                className="text-gray-600 hover:text-gray-900"
                              >
                                <XMarkIcon className="h-5 w-5" />
                              </button>
                            </div>
                          ) : (
                            <button
                              onClick={() => startEditing(plan)}
                              className="text-indigo-600 hover:text-indigo-900"
                            >
                              <PencilIcon className="h-5 w-5" />
                            </button>
                          )}
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      <div className="bg-blue-50 border border-blue-200 rounded-md p-4">
        <div className="flex">
          <div className="flex-shrink-0">
            <CurrencyDollarIcon className="h-5 w-5 text-blue-400" />
          </div>
          <div className="ml-3">
            <h3 className="text-sm font-medium text-blue-800">Subscription Plan Management</h3>
            <div className="mt-2 text-sm text-blue-700">
              <ul className="list-disc space-y-1 pl-5">
                <li>Subscription plans define pricing and feature limits for clinics</li>
                <li>Clinics can upgrade or downgrade between plans at any time</li>
                <li>New plans will be available to all clinics immediately after creation</li>
                <li>Existing subscriptions will continue on their current plans until changed</li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};