export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: "Doctor" | "ClinicAdmin" | "SuperAdmin";
  clinicId: string;
  clinic?: Clinic;
}

export interface Clinic {
  id: string;
  name: string;
  address?: string;
  phone?: string;
  email?: string;
  createdAt: string;
}

export interface Patient {
  id: string;
  clinicId: string;
  clinicPatientId?: string;
  firstName: string;
  lastName: string;
  dateOfBirth?: string;
  createdAt: string;
}

export interface CalibrationProfile {
  id: string;
  name: string;
  version: string;
  isActive: boolean;
  clinicId: string;
  createdAt: string;
}

export interface AnalysisSession {
  id: string;
  patientId: string;
  patient?: Patient;
  createdAt: string;
  status: "Pending" | "Processing" | "Completed" | "Error";
  analysisJobs?: AnalysisJob[];
  finalReportData?: any;
}

export interface AnalysisJob {
  id: string;
  sessionId: string;
  area: string;
  calibrationProfileId: string;
  calibrationProfile?: CalibrationProfile;
  status: "Pending" | "Processing" | "Completed" | "Error";
  result?: AnalysisResult;
  imageUrl?: string;
  createdAt: string;
  updatedAt: string;
}

export interface AnalysisResult {
  id: string;
  jobId: string;
  totalHairs: number;
  density: number;
  follicularUnits: number;
  annotatedImageUrl: string;
  metrics: Record<string, number>;
  createdAt: string;
}

export interface Subscription {
  id: string;
  clinicId: string;
  plan: "Basic" | "Professional" | "Enterprise";
  status: "Active" | "Expired" | "Cancelled";
  startDate: string;
  endDate: string;
  billingHistory: BillingRecord[];
}

export interface BillingRecord {
  id: string;
  subscriptionId: string;
  amount: number;
  currency: string;
  paymentDate: string;
  paymentMethod: "CreditCard" | "BankTransfer" | "Cash";
  status: "Paid" | "Pending" | "Failed";
}

export interface Invitation {
  id: string;
  email: string;
  clinicId: string;
  token: string;
  expiresAt: string;
  used: boolean;
}
