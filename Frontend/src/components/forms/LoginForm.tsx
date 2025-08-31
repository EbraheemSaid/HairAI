import React from 'react';
import { useForm } from '../../hooks/useForm';
import { loginSchema } from '../../validations/schemas';
import { Input } from '../forms/Input';
import { Button } from '../ui/Button';

interface LoginFormProps {
  onSubmit: (data: any) => void;
  isSubmitting?: boolean;
}

export const LoginForm: React.FC<LoginFormProps> = ({
  onSubmit,
  isSubmitting = false,
}) => {
  const { values, errors, handleChange, handleSubmit } = useForm({
    schema: loginSchema,
    initialValues: {
      email: '',
      password: '',
    },
    onSubmit,
  });

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div>
        <Input
          label="Email Address"
          id="email"
          name="email"
          type="email"
          autoComplete="email"
          required
          value={values.email}
          onChange={(e) => handleChange('email', e.target.value)}
          error={errors.email}
        />
      </div>

      <div>
        <Input
          label="Password"
          id="password"
          name="password"
          type="password"
          autoComplete="current-password"
          required
          value={values.password}
          onChange={(e) => handleChange('password', e.target.value)}
          error={errors.password}
        />
      </div>

      <div className="flex items-center justify-between">
        <div className="flex items-center">
          <input
            id="remember-me"
            name="remember-me"
            type="checkbox"
            className="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded"
          />
          <label htmlFor="remember-me" className="ml-2 block text-sm text-gray-900">
            Remember me
          </label>
        </div>

        <div className="text-sm">
          <a href="#" className="font-medium text-indigo-600 hover:text-indigo-500">
            Forgot your password?
          </a>
        </div>
      </div>

      <div>
        <Button type="submit" className="w-full" disabled={isSubmitting}>
          {isSubmitting ? 'Signing in...' : 'Sign in'}
        </Button>
      </div>
    </form>
  );
};