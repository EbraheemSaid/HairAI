import React, { useState } from 'react';
import { useForm } from '../../hooks/useForm';
import { analysisJobSchema } from '../../validations/schemas';
import { Select } from '../forms/Select';
import { Input } from '../forms/Input';
import { Button } from '../ui/Button';

interface AnalysisJobFormProps {
  sessions: { id: string; createdAt: string }[];
  calibrationProfiles: { id: string; name: string; version: number }[];
  headAreas: string[];
  initialData?: {
    sessionId: string;
    area: string;
    calibrationProfileId: string;
  };
  onSubmit: (data: any) => void;
  onCancel: () => void;
  isSubmitting?: boolean;
}

export const AnalysisJobForm: React.FC<AnalysisJobFormProps> = ({
  sessions,
  calibrationProfiles,
  headAreas,
  initialData,
  onSubmit,
  onCancel,
  isSubmitting = false,
}) => {
  const [selectedImage, setSelectedImage] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(null);

  const { values, errors, handleChange, handleSubmit } = useForm({
    schema: analysisJobSchema,
    initialValues: {
      sessionId: initialData?.sessionId || '',
      area: initialData?.area || '',
      calibrationProfileId: initialData?.calibrationProfileId || '',
      image: null as unknown as File, // Type assertion to satisfy Zod
    },
    onSubmit: (data) => {
      // Add the selected image to the form data
      const formData = new FormData();
      formData.append('sessionId', data.sessionId);
      formData.append('area', data.area);
      formData.append('calibrationProfileId', data.calibrationProfileId);
      if (selectedImage) {
        formData.append('image', selectedImage);
      }
      return onSubmit(formData);
    },
  });

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      const file = e.target.files[0];
      setSelectedImage(file);
      
      // Create preview
      const reader = new FileReader();
      reader.onload = () => {
        setImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
      
      // Update form state
      handleChange('image', file);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-6">
        <div className="sm:col-span-6">
          <Select
            label="Analysis Session"
            id="sessionId"
            name="sessionId"
            value={values.sessionId}
            onChange={(e) => handleChange('sessionId', e.target.value)}
            error={errors.sessionId}
            options={sessions.map((session) => ({
              value: session.id,
              label: `Session ${session.id.substring(0, 8)} (${new Date(session.createdAt).toLocaleDateString()})`,
            }))}
            required
          />
        </div>

        <div className="sm:col-span-3">
          <Select
            label="Head Area"
            id="area"
            name="area"
            value={values.area}
            onChange={(e) => handleChange('area', e.target.value)}
            error={errors.area}
            options={headAreas.map((area) => ({
              value: area,
              label: area,
            }))}
            required
          />
        </div>

        <div className="sm:col-span-3">
          <Select
            label="Calibration Profile"
            id="calibrationProfileId"
            name="calibrationProfileId"
            value={values.calibrationProfileId}
            onChange={(e) => handleChange('calibrationProfileId', e.target.value)}
            error={errors.calibrationProfileId}
            options={calibrationProfiles.map((profile) => ({
              value: profile.id,
              label: `${profile.name} (v${profile.version})`,
            }))}
            required
          />
        </div>

        <div className="sm:col-span-6">
          <div className="flex items-center justify-center px-6 pt-5 pb-6 border-2 border-dashed border-gray-300 rounded-md">
            <div className="space-y-1 text-center">
              <svg
                className="mx-auto h-12 w-12 text-gray-400"
                stroke="currentColor"
                fill="none"
                viewBox="0 0 48 48"
                aria-hidden="true"
              >
                <path
                  d="M28 8H12a4 4 0 00-4 4v20m32-12v8m0 0v8a4 4 0 01-4 4H12a4 4 0 01-4-4v-4m32-4l-3.172-3.172a4 4 0 00-5.656 0L28 28M8 32l9.172-9.172a4 4 0 015.656 0L28 28m0 0l4 4m4-24h8m-4-4v8m-12 4h.02"
                  strokeWidth={2}
                  strokeLinecap="round"
                  strokeLinejoin="round"
                />
              </svg>
              <div className="flex text-sm text-gray-600">
                <label
                  htmlFor="image"
                  className="relative cursor-pointer bg-white rounded-md font-medium text-indigo-600 hover:text-indigo-500 focus-within:outline-none focus-within:ring-2 focus-within:ring-offset-2 focus-within:ring-indigo-500"
                >
                  <span>Upload a file</span>
                  <input
                    id="image"
                    name="image"
                    type="file"
                    className="sr-only"
                    accept="image/*"
                    onChange={handleImageChange}
                    required={!selectedImage}
                  />
                </label>
                <p className="pl-1">or drag and drop</p>
              </div>
              <p className="text-xs text-gray-500">PNG, JPG, GIF up to 10MB</p>
            </div>
          </div>
          {errors.image && <p className="mt-1 text-sm text-red-600">{errors.image}</p>}
        </div>

        {imagePreview && (
          <div className="sm:col-span-6">
            <div className="mt-2">
              <img
                src={imagePreview}
                alt="Preview"
                className="h-32 w-full object-contain rounded-md border border-gray-200"
              />
            </div>
          </div>
        )}
      </div>

      <div className="flex justify-end space-x-3">
        <Button type="button" variant="secondary" onClick={onCancel} disabled={isSubmitting}>
          Cancel
        </Button>
        <Button type="submit" disabled={isSubmitting || !selectedImage}>
          {isSubmitting ? 'Uploading...' : 'Upload Image'}
        </Button>
      </div>
    </form>
  );
};