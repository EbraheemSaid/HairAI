import React from 'react';
import { useForm } from '../../hooks/useForm';
import { calibrationProfileSchema } from '../../validations/schemas';
import { Input } from '../forms/Input';
import { Button } from '../ui/Button';

interface CalibrationProfileFormProps {
  initialData?: {
    name: string;
    pixelsPerMm: number;
  };
  onSubmit: (data: any) => void;
  onCancel: () => void;
  isSubmitting?: boolean;
}

export const CalibrationProfileForm: React.FC<CalibrationProfileFormProps> = ({
  initialData,
  onSubmit,
  onCancel,
  isSubmitting = false,
}) => {
  const { values, errors, handleChange, handleSubmit } = useForm({
    schema: calibrationProfileSchema,
    initialValues: {
      name: initialData?.name || '',
      pixelsPerMm: initialData?.pixelsPerMm || 0,
    },
    onSubmit,
  });

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-6">
        <div className="sm:col-span-6">
          <Input
            label="Profile Name"
            id="name"
            name="name"
            value={values.name}
            onChange={(e) => handleChange('name', e.target.value)}
            error={errors.name}
            required
          />
        </div>

        <div className="sm:col-span-6">
          <Input
            label="Pixels per Millimeter"
            id="pixelsPerMm"
            name="pixelsPerMm"
            type="number"
            step="0.01"
            min="0"
            value={values.pixelsPerMm}
            onChange={(e) => handleChange('pixelsPerMm', parseFloat(e.target.value) || 0)}
            error={errors.pixelsPerMm}
            required
            helperText="Enter the number of pixels that represent one millimeter in your trichoscope images"
          />
        </div>
      </div>

      <div className="flex justify-end space-x-3">
        <Button type="button" variant="secondary" onClick={onCancel} disabled={isSubmitting}>
          Cancel
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Saving...' : 'Save Profile'}
        </Button>
      </div>
    </form>
  );
};