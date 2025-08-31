import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import {
  ArrowLeftIcon,
  CloudArrowUpIcon,
  PlayIcon,
} from "@heroicons/react/24/outline";
import { analysisService } from "../../services/analysisService";
import { patientService } from "../../services/patientService";
import { calibrationService } from "../../services/calibrationService";
import type { Patient, CalibrationProfile } from "../../types";
import { toast } from "react-hot-toast";

export const NewAnalysisPage: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { patientId } = location.state || {};

  const [patients, setPatients] = useState<Patient[]>([]);
  const [calibrationProfiles, setCalibrationProfiles] = useState<
    CalibrationProfile[]
  >([]);
  const [selectedPatient, setSelectedPatient] = useState(patientId || "");
  const [selectedCalibration, setSelectedCalibration] = useState("");
  const [selectedArea, setSelectedArea] = useState("");
  const [uploadedImages, setUploadedImages] = useState<File[]>([]);
  const [previews, setPreviews] = useState<string[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  // Head areas for analysis
  const headAreas = [
    "Crown",
    "Frontal",
    "Temporal Left",
    "Temporal Right",
    "Vertex",
    "Occipital",
    "Donor Area",
  ];

  useEffect(() => {
    fetchInitialData();
  }, []);

  const fetchInitialData = async () => {
    try {
      setLoading(true);
      const [patientsData, calibrationData] = await Promise.all([
        patientService.getPatients(),
        calibrationService.getActiveCalibrationProfiles(),
      ]);
      setPatients(patientsData);
      setCalibrationProfiles(calibrationData);

      // Pre-select first calibration profile if available
      if (calibrationData.length > 0) {
        setSelectedCalibration(calibrationData[0].id);
      }
    } catch (error) {
      toast.error("Failed to load initial data");
      console.error("Error fetching initial data:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleImageUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      const files = Array.from(e.target.files);
      setUploadedImages(files);

      // Create previews for uploaded images
      const newPreviews: string[] = [];
      files.forEach((file) => {
        const reader = new FileReader();
        reader.onload = () => {
          newPreviews.push(reader.result as string);
          if (newPreviews.length === files.length) {
            setPreviews(newPreviews);
          }
        };
        reader.readAsDataURL(file);
      });
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (
      !selectedPatient ||
      !selectedCalibration ||
      !selectedArea ||
      uploadedImages.length === 0
    ) {
      toast.error(
        "Please fill in all required fields and upload at least one image"
      );
      return;
    }

    setSubmitting(true);

    try {
      // Create analysis session
      const session = await analysisService.createAnalysisSession({
        patientId: selectedPatient,
      });

      // Upload each image
      const uploadPromises = uploadedImages.map((image) => {
        return analysisService.uploadAnalysisImage({
          sessionId: session.id,
          area: selectedArea,
          calibrationProfileId: selectedCalibration,
          image,
        });
      });

      await Promise.all(uploadPromises);

      toast.success("Analysis session created successfully!");
      navigate(`/analysis/session/${session.id}`);
    } catch (error) {
      toast.error("Failed to create analysis session");
      console.error("Error creating analysis session:", error);
    } finally {
      setSubmitting(false);
    }
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
      <div>
        <button
          onClick={() => navigate(-1)}
          className="inline-flex items-center text-sm font-medium text-indigo-600 hover:text-indigo-500"
        >
          <ArrowLeftIcon className="h-5 w-5 mr-1" />
          Back
        </button>
        <h1 className="mt-2 text-2xl font-bold text-gray-900">New Analysis</h1>
        <p className="mt-1 text-sm text-gray-500">
          Upload trichoscope images for AI analysis
        </p>
      </div>

      <div className="bg-white shadow sm:rounded-lg">
        <div className="px-4 py-5 sm:p-6">
          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-6">
              <div className="sm:col-span-3">
                <label
                  htmlFor="patient"
                  className="block text-sm font-medium text-gray-700"
                >
                  Patient
                </label>
                <select
                  id="patient"
                  value={selectedPatient}
                  onChange={(e) => setSelectedPatient(e.target.value)}
                  className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                >
                  <option value="">Select a patient</option>
                  {patients.map((patient) => (
                    <option key={patient.id} value={patient.id}>
                      {patient.firstName} {patient.lastName}{" "}
                      {patient.clinicPatientId
                        ? `(${patient.clinicPatientId})`
                        : ""}
                    </option>
                  ))}
                </select>
              </div>

              <div className="sm:col-span-3">
                <label
                  htmlFor="calibration"
                  className="block text-sm font-medium text-gray-700"
                >
                  Calibration Profile
                </label>
                <select
                  id="calibration"
                  value={selectedCalibration}
                  onChange={(e) => setSelectedCalibration(e.target.value)}
                  className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                >
                  <option value="">Select a calibration profile</option>
                  {calibrationProfiles.map((profile) => (
                    <option key={profile.id} value={profile.id}>
                      {profile.name} (v{profile.version})
                    </option>
                  ))}
                </select>
              </div>

              <div className="sm:col-span-3">
                <label
                  htmlFor="area"
                  className="block text-sm font-medium text-gray-700"
                >
                  Head Area
                </label>
                <select
                  id="area"
                  value={selectedArea}
                  onChange={(e) => setSelectedArea(e.target.value)}
                  className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                >
                  <option value="">Select head area</option>
                  {headAreas.map((area) => (
                    <option key={area} value={area}>
                      {area}
                    </option>
                  ))}
                </select>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">
                Upload Images
              </label>
              <div className="mt-1 flex justify-center px-6 pt-5 pb-6 border-2 border-gray-300 border-dashed rounded-md">
                <div className="space-y-1 text-center">
                  <CloudArrowUpIcon className="mx-auto h-12 w-12 text-gray-400" />
                  <div className="flex text-sm text-gray-600">
                    <label
                      htmlFor="file-upload"
                      className="relative cursor-pointer bg-white rounded-md font-medium text-indigo-600 hover:text-indigo-500 focus-within:outline-none focus-within:ring-2 focus-within:ring-offset-2 focus-within:ring-indigo-500"
                    >
                      <span>Upload files</span>
                      <input
                        id="file-upload"
                        name="file-upload"
                        type="file"
                        className="sr-only"
                        multiple
                        accept="image/*"
                        onChange={handleImageUpload}
                      />
                    </label>
                    <p className="pl-1">or drag and drop</p>
                  </div>
                  <p className="text-xs text-gray-500">
                    PNG, JPG, GIF up to 10MB
                  </p>
                </div>
              </div>
            </div>

            {previews.length > 0 && (
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Selected Images
                </label>
                <div className="mt-2 grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">
                  {previews.map((preview, index) => (
                    <div key={index} className="relative">
                      <img
                        src={preview}
                        alt={`Preview ${index + 1}`}
                        className="h-32 w-full object-cover rounded-md"
                      />
                      <div className="absolute inset-0 bg-black bg-opacity-20 rounded-md flex items-center justify-center opacity-0 hover:opacity-100 transition-opacity">
                        <span className="text-white text-sm font-medium">
                          {uploadedImages[index]?.name}
                        </span>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}

            <div className="flex justify-end">
              <button
                type="button"
                onClick={() => navigate(-1)}
                className="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={
                  submitting ||
                  !selectedPatient ||
                  !selectedCalibration ||
                  !selectedArea ||
                  uploadedImages.length === 0
                }
                className="ml-3 inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
              >
                {submitting ? (
                  <>
                    <svg
                      className="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
                      xmlns="http://www.w3.org/2000/svg"
                      fill="none"
                      viewBox="0 0 24 24"
                    >
                      <circle
                        className="opacity-25"
                        cx="12"
                        cy="12"
                        r="10"
                        stroke="currentColor"
                        strokeWidth="4"
                      ></circle>
                      <path
                        className="opacity-75"
                        fill="currentColor"
                        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                      ></path>
                    </svg>
                    Processing...
                  </>
                ) : (
                  <>
                    <PlayIcon className="-ml-1 mr-2 h-5 w-5" />
                    Start Analysis
                  </>
                )}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};
