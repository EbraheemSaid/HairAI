import { z } from 'zod';

export const loginSchema = z.object({
  email: z.string().email('Invalid email address'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
});

export const registerSchema = z.object({
  clinicName: z.string().min(1, 'Clinic name is required'),
  clinicPhone: z.string().min(1, 'Clinic phone is required'),
  clinicEmail: z.string().email('Invalid email address'),
  firstName: z.string().min(1, 'First name is required'),
  lastName: z.string().min(1, 'Last name is required'),
  email: z.string().email('Invalid email address'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
});

export const acceptInvitationSchema = z.object({
  firstName: z.string().min(1, 'First name is required'),
  lastName: z.string().min(1, 'Last name is required'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
  confirmPassword: z.string(),
}).refine((data) => data.password === data.confirmPassword, {
  message: "Passwords don't match",
  path: ["confirmPassword"],
});

export const patientSchema = z.object({
  firstName: z.string().min(1, 'First name is required'),
  lastName: z.string().min(1, 'Last name is required'),
  dateOfBirth: z.string().min(1, 'Date of birth is required'),
  phone: z.string().optional(),
  email: z.string().email('Invalid email address').optional().or(z.literal('')),
  address: z.string().optional(),
});

export const clinicSchema = z.object({
  name: z.string().min(1, 'Clinic name is required'),
  address: z.string().optional(),
  phone: z.string().optional(),
  email: z.string().email('Invalid email address').optional().or(z.literal('')),
});

export const calibrationProfileSchema = z.object({
  name: z.string().min(1, 'Profile name is required'),
  pixelsPerMm: z.number().positive('Pixels per mm must be positive'),
});

export const analysisSessionSchema = z.object({
  patientId: z.string().min(1, 'Patient is required'),
});

export const analysisJobSchema = z.object({
  sessionId: z.string().min(1, 'Session is required'),
  area: z.string().min(1, 'Head area is required'),
  calibrationProfileId: z.string().min(1, 'Calibration profile is required'),
  image: z.instanceof(File, { message: 'Image file is required' }),
});

export const invitationSchema = z.object({
  email: z.string().email('Invalid email address'),
  role: z.enum(['Doctor', 'ClinicAdmin'], {
    errorMap: () => ({ message: 'Role must be either Doctor or ClinicAdmin' }),
  }),
});