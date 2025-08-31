import React from 'react';
import { useForm } from '../../hooks/useForm';
import { registerSchema } from '../../validations/schemas';
import { Input } from '../forms/Input';
import { Button } from '../ui/Button';

interface RegistrationFormProps {
  onSubmit: (data: any) => void;
  onCancel: () => void;
  isSubmitting?: boolean;
}

export const RegistrationForm: React.FC<RegistrationFormProps> = ({
  onSubmit,
  onCancel,
  isSubmitting = false,
}) => {
  const { values, errors, handleChange, handleSubmit } = useForm({
    schema: registerSchema,
    initialValues: {
      clinicName: '',
      clinicPhone: '',
      clinicEmail: '',
      firstName: '',
      lastName: '',
      email: '',
      password: '',
    },
    onSubmit,
  });

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="space-y-6">
        <div>
          <h3 className="text-lg font-medium leading-6 text-gray-900">Clinic Information</h3>
          <p className="mt-1 text-sm text-gray-500">
            Enter your clinic's details to create your account.
          </p>
        </div>

        <div className="grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-6">
          <div className="sm:col-span-6">
            <Input
              label="Clinic Name"
              id="clinicName"
              name="clinicName"
              value={values.clinicName}
              onChange={(e) => handleChange('clinicName', e.target.value)}
              error={errors.clinicName}
              required
            />
          </div>

          <div className="sm:col-span-3">
            <Input
              label="Clinic Phone"
              id="clinicPhone"
              name="clinicPhone"
              type="tel"
              value={values.clinicPhone}
              onChange={(e) => handleChange('clinicPhone', e.target.value)}
              error={errors.clinicPhone}
              required
            />
          </div>

          <div className="sm:col-span-3">
            <Input
              label="Clinic Email"
              id="clinicEmail"
              name="clinicEmail"
              type="email"
              value={values.clinicEmail}
              onChange={(e) => handleChange('clinicEmail', e.target.value)}
              error={errors.clinicEmail}
              required
            />
          </div>
        </div>

        <div className="border-t border-gray-200 pt-6">
          <h3 className="text-lg font-medium leading-6 text-gray-900">Administrator Account</h3>
          <p className="mt-1 text-sm text-gray-500">
            Create your administrator account for managing the clinic.
          </p>
        </div>

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
              label="Email Address"
              id="email"
              name="email"
              type="email"
              value={values.email}
              onChange={(e) => handleChange('email', e.target.value)}
              error={errors.email}
              required
            />
          </div>

          <div className="sm:col-span-3">
            <Input
              label="Password"
              id="password"
              name="password"
              type="password"
              value={values.password}
              onChange={(e) => handleChange('password', e.target.value)}
              error={errors.password}
              required
              helperText="Must be at least 6 characters"
            />
          </div>
        </div>
      </div>

      <div className="flex justify-end space-x-3">
        <Button type="button" variant="secondary" onClick={onCancel} disabled={isSubmitting}>
          Cancel
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Creating Account...' : 'Create Account'}
        </Button>
      </div>
    </form>
  );
};