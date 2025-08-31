// Re-export types from services for convenience
export type {
  AdminClinicResponse,
  CreateClinicRequest,
  AdminUserResponse,
  CreateUserRequest,
  AdminSubscriptionResponse,
  DashboardStatsResponse,
  User,
  LoginRequest,
  LoginResponse,
} from "../services/adminService";
export type { User as AuthUser } from "../services/authService";

// Additional types for UI components
export interface MetricCardProps {
  title: string;
  value: string;
  description: string;
  trend?: "up" | "down" | "neutral";
  trendValue?: string;
  icon: React.ReactNode;
  iconColor: "indigo" | "green" | "blue" | "yellow" | "red";
}

export interface ChartDataPoint {
  name: string;
  value: number;
}

export interface TableColumn {
  key: string;
  title: string;
  sortable?: boolean;
  render?: (value: any, row: any) => React.ReactNode;
}

export interface DataTableProps {
  data: any[];
  columns: TableColumn[];
  pagination?: boolean;
  itemsPerPage?: number;
  loading?: boolean;
  emptyMessage?: string;
}

// Status enums
export enum SubscriptionStatus {
  ACTIVE = "Active",
  EXPIRED = "Expired",
  CANCELLED = "Cancelled",
  PENDING = "Pending",
}

export enum UserRole {
  SUPER_ADMIN = "SuperAdmin",
  CLINIC_ADMIN = "ClinicAdmin",
  DOCTOR = "Doctor",
}

export enum SystemStatus {
  ONLINE = "online",
  OFFLINE = "offline",
  MAINTENANCE = "maintenance",
}

