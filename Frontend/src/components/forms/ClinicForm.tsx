import React from 'react';
import { useForm } from '../../hooks/useForm';
import { clinicSchema } from '../../validations/schemas';
import { Input, TextArea } from '../forms/Input';
import { Button } from '../ui/Button';

interface ClinicFormProps {
  initialData?: {
    name: string;
    address?: string;
    phone?: string;
    email?: string;
  };
  onSubmit: (data: any) => void;
  onCancel: () => void;
  isSubmitting?: boolean;
}

export const ClinicForm: React.FC<ClinicFormProps> = ({
  initialData,
  onSubmit,
  onCancel,
  isSubmitting = false,
}) => {
  const { values, errors, handleChange, handleSubmit } = useForm({
    schema: clinicSchema,
    initialValues: {
      name: initialData?.name || '',
      address: initialData?.address || '',
      phone: initialData?.phone || '',
      email: initialData?.email || '',
    },
    onSubmit,
  });

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-6">
        <div className="sm:col-span-6">
          <Input
            label="Clinic Name"
            id="name"
            name="name"
            value={values.name}
            onChange={(e) => handleChange('name', e.target.value)}
            error={errors.name}
            required
          />
        </div>

        <div className="sm:col-span-3">
          <Input
            label="Phone"
            id="phone"
            name="phone"
            type="tel"
            value={values.phone}
            onChange={(e) => handleChange('phone', e.target.value)}
            error={errors.phone}
          />
        </div>

        <div className="sm:col-span-3">
          <Input
            label="Email"
            id="email"
            name="email"
            type="email"
            value={values.email}
            onChange={(e) => handleChange('email', e.target.value)}
            error={errors.email}
          />
        </div>

        <div className="sm:col-span-6">
          <TextArea
            label="Address"
            id="address"
            name="address"
            value={values.address}
            onChange={(e) => handleChange('address', e.target.value)}
            error={errors.address}
            rows={3}
          />
        </div>
      </div>

      <div className="flex justify-end space-x-3">
        <Button type="button" variant="secondary" onClick={onCancel} disabled={isSubmitting}>
          Cancel
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Saving...' : 'Save Clinic'}
        </Button>
      </div>
    </form>
  );
};