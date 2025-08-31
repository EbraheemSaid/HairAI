import React, { useState } from 'react';
import { useForm } from '../../hooks/useForm';
import { acceptInvitationSchema } from '../../validations/schemas';
import { Input } from '../forms/Input';
import { Button } from '../ui/Button';

interface AcceptInvitationFormProps {
  onSubmit: (data: any) => void;
  isSubmitting?: boolean;
}

export const AcceptInvitationForm: React.FC<AcceptInvitationFormProps> = ({
  onSubmit,
  isSubmitting = false,
}) => {
  const [showPassword, setShowPassword] = useState(false);

  const { values, errors, handleChange, handleSubmit } = useForm({
    schema: acceptInvitationSchema,
    initialValues: {
      firstName: '',
      lastName: '',
      password: '',
      confirmPassword: '',
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

        <div className="sm:col-span-6">
          <Input
            label="Password"
            id="password"
            name="password"
            type={showPassword ? "text" : "password"}
            value={values.password}
            onChange={(e) => handleChange('password', e.target.value)}
            error={errors.password}
            required
            helperText="Must be at least 6 characters"
          />
        </div>

        <div className="sm:col-span-6">
          <Input
            label="Confirm Password"
            id="confirmPassword"
            name="confirmPassword"
            type={showPassword ? "text" : "password"}
            value={values.confirmPassword}
            onChange={(e) => handleChange('confirmPassword', e.target.value)}
            error={errors.confirmPassword}
            required
          />
        </div>

        <div className="sm:col-span-6">
          <div className="flex items-center">
            <input
              id="show-password"
              name="show-password"
              type="checkbox"
              checked={showPassword}
              onChange={(e) => setShowPassword(e.target.checked)}
              className="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded"
            />
            <label htmlFor="show-password" className="ml-2 block text-sm text-gray-900">
              Show passwords
            </label>
          </div>
        </div>
      </div>

      <div>
        <Button type="submit" className="w-full" disabled={isSubmitting}>
          {isSubmitting ? 'Creating Account...' : 'Create Account'}
        </Button>
      </div>
    </form>
  );
};