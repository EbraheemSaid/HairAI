import React, { useState, useEffect } from 'react';
import { PlusIcon, PencilIcon, TrashIcon, CheckIcon, XMarkIcon } from '@heroicons/react/24/outline';
import { calibrationService } from '../../services/calibrationService';
import type { CalibrationProfile } from '../../types';
import { toast } from 'react-hot-toast';

export const CalibrationProfilesPage: React.FC = () => {
  const [profiles, setProfiles] = useState<CalibrationProfile[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [editingProfile, setEditingProfile] = useState<string | null>(null);
  
  const [formData, setFormData] = useState({
    name: '',
    pixelsPerMm: '',
  });

  useEffect(() => {
    fetchCalibrationProfiles();
  }, []);

  const fetchCalibrationProfiles = async () => {
    try {
      setLoading(true);
      const data = await calibrationService.getActiveCalibrationProfiles();
      setProfiles(data);
    } catch (error) {
      toast.error('Failed to load calibration profiles');
      console.error('Error fetching calibration profiles:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    
    try {
      const requestData = {
        name: formData.name,
        calibrationData: {
          pixels_per_mm: parseFloat(formData.pixelsPerMm)
        }
      };
      
      await calibrationService.createCalibrationProfile(requestData);
      toast.success('Calibration profile created successfully');
      setShowCreateForm(false);
      setFormData({ name: '', pixelsPerMm: '' });
      fetchCalibrationProfiles();
    } catch (error) {
      toast.error('Failed to create calibration profile');
      console.error('Error creating calibration profile:', error);
    }
  };

  const handleUpdate = async (profileId: string) => {
    try {
      const profile = profiles.find(p => p.id === profileId);
      if (!profile) return;
      
      const requestData = {
        name: formData.name || profile.name,
        calibrationData: {
          pixels_per_mm: formData.pixelsPerMm ? parseFloat(formData.pixelsPerMm) : profile.calibrationData.pixels_per_mm
        },
        isActive: profile.isActive
      };
      
      await calibrationService.updateCalibrationProfile(profileId, requestData);
      toast.success('Calibration profile updated successfully');
      setEditingProfile(null);
      setFormData({ name: '', pixelsPerMm: '' });
      fetchCalibrationProfiles();
    } catch (error) {
      toast.error('Failed to update calibration profile');
      console.error('Error updating calibration profile:', error);
    }
  };

  const handleDelete = async (profileId: string) => {
    if (!window.confirm('Are you sure you want to deactivate this calibration profile?')) {
      return;
    }
    
    try {
      await calibrationService.deactivateCalibrationProfile(profileId);
      toast.success('Calibration profile deactivated successfully');
      fetchCalibrationProfiles();
    } catch (error) {
      toast.error('Failed to deactivate calibration profile');
      console.error('Error deactivating calibration profile:', error);
    }
  };

  const startEditing = (profile: CalibrationProfile) => {
    setEditingProfile(profile.id);
    setFormData({
      name: profile.name,
      pixelsPerMm: profile.calibrationData.pixels_per_mm?.toString() || ''
    });
  };

  const cancelEditing = () => {
    setEditingProfile(null);
    setFormData({ name: '', pixelsPerMm: '' });
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
          <h1 className="text-2xl font-bold text-gray-900">Calibration Profiles</h1>
          <p className="mt-1 text-sm text-gray-500">
            Manage camera calibration settings for accurate measurements
          </p>
        </div>
        <div className="mt-4 md:mt-0">
          <button
            onClick={() => setShowCreateForm(true)}
            className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            <PlusIcon className="-ml-1 mr-2 h-5 w-5" aria-hidden="true" />
            New Profile
          </button>
        </div>
      </div>

      {showCreateForm && (
        <div className="bg-white shadow sm:rounded-lg">
          <div className="px-4 py-5 sm:p-6">
            <h3 className="text-lg leading-6 font-medium text-gray-900">Create New Calibration Profile</h3>
            <form onSubmit={handleCreate} className="mt-4 space-y-4">
              <div>
                <label htmlFor="name" className="block text-sm font-medium text-gray-700">
                  Profile Name
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
              <div>
                <label htmlFor="pixelsPerMm" className="block text-sm font-medium text-gray-700">
                  Pixels per Millimeter
                </label>
                <input
                  type="number"
                  name="pixelsPerMm"
                  id="pixelsPerMm"
                  required
                  step="0.01"
                  min="0"
                  value={formData.pixelsPerMm}
                  onChange={(e) => setFormData({...formData, pixelsPerMm: e.target.value})}
                  className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                />
                <p className="mt-1 text-sm text-gray-500">
                  Enter the number of pixels that represent one millimeter in your trichoscope images
                </p>
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
                  className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  Create Profile
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
                      Profile Name
                    </th>
                    <th
                      scope="col"
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                    >
                      Version
                    </th>
                    <th
                      scope="col"
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                    >
                      Calibration Data
                    </th>
                    <th
                      scope="col"
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                    >
                      Status
                    </th>
                    <th scope="col" className="relative px-6 py-3">
                      <span className="sr-only">Actions</span>
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {profiles.length === 0 ? (
                    <tr>
                      <td colSpan={5} className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-center">
                        No calibration profiles found
                      </td>
                    </tr>
                  ) : (
                    profiles.map((profile) => (
                      <tr key={profile.id} className="hover:bg-gray-50">
                        <td className="px-6 py-4 whitespace-nowrap">
                          {editingProfile === profile.id ? (
                            <input
                              type="text"
                              value={formData.name}
                              onChange={(e) => setFormData({...formData, name: e.target.value})}
                              className="block w-full border border-gray-300 rounded-md shadow-sm py-1 px-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                            />
                          ) : (
                            <div className="text-sm font-medium text-gray-900">{profile.name}</div>
                          )}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="text-sm text-gray-900">v{profile.version}</div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          {editingProfile === profile.id ? (
                            <input
                              type="number"
                              step="0.01"
                              min="0"
                              value={formData.pixelsPerMm}
                              onChange={(e) => setFormData({...formData, pixelsPerMm: e.target.value})}
                              className="block w-full border border-gray-300 rounded-md shadow-sm py-1 px-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                            />
                          ) : (
                            <div className="text-sm text-gray-900">
                              {profile.calibrationData.pixels_per_mm || 'N/A'} pixels/mm
                            </div>
                          )}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                            profile.isActive 
                              ? 'bg-green-100 text-green-800' 
                              : 'bg-gray-100 text-gray-800'
                          }`}>
                            {profile.isActive ? 'Active' : 'Inactive'}
                          </span>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                          {editingProfile === profile.id ? (
                            <div className="flex space-x-2">
                              <button
                                onClick={() => handleUpdate(profile.id)}
                                className="text-green-600 hover:text-green-900"
                              >
                                <CheckIcon className="h-5 w-5" />
                              </button>
                              <button
                                onClick={cancelEditing}
                                className="text-gray-600 hover:text-gray-900"
                              >
                                <XMarkIcon className="h-5 w-5" />
                              </button>
                            </div>
                          ) : (
                            <div className="flex space-x-2">
                              <button
                                onClick={() => startEditing(profile)}
                                className="text-indigo-600 hover:text-indigo-900"
                              >
                                <PencilIcon className="h-5 w-5" />
                              </button>
                              <button
                                onClick={() => handleDelete(profile.id)}
                                className="text-red-600 hover:text-red-900"
                              >
                                <TrashIcon className="h-5 w-5" />
                              </button>
                            </div>
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
            <svg className="h-5 w-5 text-blue-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <div className="ml-3">
            <h3 className="text-sm font-medium text-blue-800">Calibration Tips</h3>
            <div className="mt-2 text-sm text-blue-700">
              <ul className="list-disc space-y-1 pl-5">
                <li>Calibrate your trichoscope regularly for accurate measurements</li>
                <li>Use a standard calibration slide with known dimensions</li>
                <li>Higher pixels/mm values indicate higher magnification</li>
                <li>Each new calibration creates a new version of the profile</li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};