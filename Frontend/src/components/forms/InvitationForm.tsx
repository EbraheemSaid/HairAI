import React from 'react';
import { useForm } from '../../hooks/useForm';
import { invitationSchema } from '../../validations/schemas';
import { Input } from '../forms/Input';
import { Select } from '../forms/Select';
import { Button } from '../ui/Button';

interface InvitationFormProps {
  initialData?: {
    email: string;
    role: 'Doctor' | 'ClinicAdmin';
  };
  onSubmit: (data: any) => void;
  onCancel: () => void;
  isSubmitting?: boolean;
}

export const InvitationForm: React.FC<InvitationFormProps> = ({
  initialData,
  onSubmit,
  onCancel,
  isSubmitting = false,
}) => {
  const { values, errors, handleChange, handleSubmit } = useForm({
    schema: invitationSchema,
    initialValues: {
      email: initialData?.email || '',
      role: initialData?.role || 'Doctor',
    },
    onSubmit,
  });

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-6">
        <div className="sm:col-span-6">
          <Input
            label="Email Address"
            id="email"
            name="email"
            type="email"
            value={values.email}
            onChange={(e) => handleChange('email', e.target.value)}
            error={errors.email}
            required
            helperText="The email address of the person you want to invite"
          />
        </div>

        <div className="sm:col-span-6">
          <Select
            label="Role"
            id="role"
            name="role"
            value={values.role}
            onChange={(e) => handleChange('role', e.target.value as 'Doctor' | 'ClinicAdmin')}
            error={errors.role}
            options={[
              { value: 'Doctor', label: 'Doctor' },
              { value: 'ClinicAdmin', label: 'Clinic Administrator' },
            ]}
            required
            helperText="The role that will be assigned to the invited user"
          />
        </div>
      </div>

      <div className="flex justify-end space-x-3">
        <Button type="button" variant="secondary" onClick={onCancel} disabled={isSubmitting}>
          Cancel
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Sending...' : 'Send Invitation'}
        </Button>
      </div>
    </form>
  );
};