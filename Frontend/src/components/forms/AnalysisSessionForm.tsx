import React from 'react';
import { useForm } from '../../hooks/useForm';
import { analysisSessionSchema } from '../../validations/schemas';
import { Select } from '../forms/Select';
import { Button } from '../ui/Button';

interface AnalysisSessionFormProps {
  patients: { id: string; firstName: string; lastName: string; code: string }[];
  initialData?: {
    patientId: string;
  };
  onSubmit: (data: any) => void;
  onCancel: () => void;
  isSubmitting?: boolean;
}

export const AnalysisSessionForm: React.FC<AnalysisSessionFormProps> = ({
  patients,
  initialData,
  onSubmit,
  onCancel,
  isSubmitting = false,
}) => {
  const { values, errors, handleChange, handleSubmit } = useForm({
    schema: analysisSessionSchema,
    initialValues: {
      patientId: initialData?.patientId || '',
    },
    onSubmit,
  });

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-6">
        <div className="sm:col-span-6">
          <Select
            label="Select Patient"
            id="patientId"
            name="patientId"
            value={values.patientId}
            onChange={(e) => handleChange('patientId', e.target.value)}
            error={errors.patientId}
            options={patients.map((patient) => ({
              value: patient.id,
              label: `${patient.firstName} ${patient.lastName} (${patient.code})`,
            }))}
            required
          />
        </div>
      </div>

      <div className="flex justify-end space-x-3">
        <Button type="button" variant="secondary" onClick={onCancel} disabled={isSubmitting}>
          Cancel
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Creating...' : 'Create Session'}
        </Button>
      </div>
    </form>
  );
};