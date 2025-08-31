import React from 'react';
import { useForm } from '../../hooks/useForm';
import { patientSchema } from '../../validations/schemas';
import { Input, TextArea } from '../forms/Input';
import { Button } from '../ui/Button';

interface PatientFormProps {
  initialData?: {
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    phone?: string;
    email?: string;
    address?: string;
  };
  onSubmit: (data: any) => void;
  onCancel: () => void;
  isSubmitting?: boolean;
}

export const PatientForm: React.FC<PatientFormProps> = ({
  initialData,
  onSubmit,
  onCancel,
  isSubmitting = false,
}) => {
  const { values, errors, handleChange, handleSubmit } = useForm({
    schema: patientSchema,
    initialValues: {
      firstName: initialData?.firstName || '',
      lastName: initialData?.lastName || '',
      dateOfBirth: initialData?.dateOfBirth || '',
      phone: initialData?.phone || '',
      email: initialData?.email || '',
      address: initialData?.address || '',
    },
    onSubmit,
  });

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-6">
        <div className="sm:col-span-3">
          <Input
            label="First Name"
            id="firstName"
            name="firstName"
            value={values.firstName}
            onChange={(e) => handleChange('firstName', e.target.value)}
            error={errors.firstName}
            required
          />
        </div>

        <div className="sm:col-span-3">
          <Input
            label="Last Name"
            id="lastName"
            name="lastName"
            value={values.lastName}
            onChange={(e) => handleChange('lastName', e.target.value)}
            error={errors.lastName}
            required
          />
        </div>

        <div className="sm:col-span-3">
          <Input
            label="Date of Birth"
            id="dateOfBirth"
            name="dateOfBirth"
            type="date"
            value={values.dateOfBirth}
            onChange={(e) => handleChange('dateOfBirth', e.target.value)}
            error={errors.dateOfBirth}
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
          {isSubmitting ? 'Saving...' : 'Save Patient'}
        </Button>
      </div>
    </form>
  );
};