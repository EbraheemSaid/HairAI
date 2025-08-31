//// ------------------------------------------------------
//// HairAI SaaS Platform / Final Database Schema
//// Version: 12.0 (dbdiagram.io DSL with Session Management)
//// ------------------------------------------------------

//// -----------------
//// Identity & Access
//// -----------------

Table AspNetRoles {
  Id text [pk]
  Name varchar(256) [unique, not null]
  NormalizedName varchar(256) [unique]
}

Table AspNetUsers {
  Id text [pk]
  UserName varchar(256) [unique]
  NormalizedUserName varchar(256) [unique]
  Email varchar(256) [unique]
  NormalizedEmail varchar(256) [unique]
  EmailConfirmed boolean [not null]
  PasswordHash text
  SecurityStamp text
  ConcurrencyStamp text
  PhoneNumber text
  PhoneNumberConfirmed boolean [not null]
  TwoFactorEnabled boolean [not null]
  LockoutEnd timestamptz
  LockoutEnabled boolean [not null]
  AccessFailedCount integer [not null]
  FirstName varchar(100) [not null]
  LastName varchar(100) [not null]
  ClinicId uuid [ref: > clinics.id]
}

Table AspNetUserRoles {
  UserId text [pk, ref: > AspNetUsers.Id]
  RoleId text [pk, ref: > AspNetRoles.Id]
}


//// -----------------
//// Core Application
//// -----------------

Table clinics {
  id uuid [pk, default: `gen_random_uuid()`]
  name varchar(255) [not null]
  created_at timestamptz [not null, default: `now()`]
  updated_at timestamptz [not null, default: `now()`]
}

Table patients {
  id uuid [pk, default: `gen_random_uuid()`]
  clinic_id uuid [not null, ref: > clinics.id]
  clinic_patient_id varchar(50) [note: 'Optional, human-readable ID assigned by the clinic']
  first_name varchar(100) [not null]
  last_name varchar(100) [not null]
  date_of_birth date
  created_at timestamptz [not null, default: `now()`]

  indexes {
    (clinic_id, clinic_patient_id) [unique]
  }
}

Table calibration_profiles {
  id uuid [pk, default: `gen_random_uuid()`]
  clinic_id uuid [not null, ref: > clinics.id]
  profile_name varchar(100) [not null]
  calibration_data jsonb [not null, note: 'e.g., {"pixels_per_mm": 15.7}']
  version int [not null, default: 1]
  is_active boolean [not null, default: true]
  created_at timestamptz [not null, default: `now()`]

  indexes {
    (clinic_id, profile_name, version) [unique, note: 'Ensures a profile version is unique within a clinic']
  }
}

// NEW TABLE: This groups multiple analyses into a single patient visit/report
Table analysis_sessions {
  id uuid [pk, default: `gen_random_uuid()`]
  patient_id uuid [not null, ref: > patients.id]
  created_by_user_id text [not null, ref: > AspNetUsers.Id]
  session_date date [not null, default: `now()`]
  status varchar(50) [not null, note: 'e.g., in_progress, completed']
  final_report_data jsonb [note: 'Stores aggregated metrics for the final report']
  created_at timestamptz [not null, default: `now()`]
}

Table analysis_jobs {
  id uuid [pk, default: `gen_random_uuid()`]
  // ADDED: Links this individual analysis to a main session
  session_id uuid [not null, ref: > analysis_sessions.id]
  patient_id uuid [not null, ref: > patients.id]
  calibration_profile_id uuid [not null, ref: > calibration_profiles.id]
  created_by_user_id text [not null, ref: > AspNetUsers.Id]
  // ADDED: The specific location on the 3D head model
  location_tag varchar(100) [not null, note: 'e.g., Crown, Donor Area']
  image_storage_key text [not null, note: 'Path to the ORIGINAL image']
  annotated_image_key text [note: 'Path to the ANNOTATED image']
  status varchar(50) [not null, note: 'e.g., pending, processing, completed, failed']
  analysis_result jsonb [note: 'Raw JSON output from the AI model']
  doctor_notes text
  created_at timestamptz [not null, default: `now()`]
  started_at timestamptz
  completed_at timestamptz
  error_message text
  processing_time_ms int
}


//// ---------------------
//// Subscription & Billing
//// ---------------------

Table subscription_plans {
  id uuid [pk, default: `gen_random_uuid()`]
  name varchar(100) [not null, unique]
  price_monthly decimal(10, 2) [not null]
  currency varchar(3) [not null, note: 'e.g., EGP, USD']
  max_users int [not null]
  max_analyses_per_month int [not null]
}

Table subscriptions {
  id uuid [pk, default: `gen_random_uuid()`]
  clinic_id uuid [unique, not null, ref: > clinics.id]
  plan_id uuid [not null, ref: > subscription_plans.id]
  status varchar(50) [not null, note: 'e.g., active, past_due, canceled']
  current_period_start timestamptz
  current_period_end timestamptz
}

Table payments {
  id uuid [pk, default: `gen_random_uuid()`]
  subscription_id uuid [not null, ref: > subscriptions.id]
  amount decimal(10, 2) [not null]
  currency varchar(3) [not null]
  status varchar(50) [not null, note: 'e.g., succeeded, failed, refunded']
  payment_gateway_reference text
  processed_at timestamptz [not null, default: `now()`]
}


//// ---------------------
//// Operational & Auditing
//// ---------------------

Table clinic_invitations {
  id uuid [pk, default: `gen_random_uuid()`]
  clinic_id uuid [not null, ref: > clinics.id]
  invited_by_user_id text [not null, ref: > AspNetUsers.Id]
  email varchar(255) [not null]
  role varchar(50) [not null, note: 'Role to be assigned, e.g., Doctor']
  token varchar(255) [unique, not null]
  status varchar(50) [not null, note: 'e.g., pending, accepted, expired']
  expires_at timestamptz [not null]
}

Table audit_logs {
  id bigserial [pk]
  user_id text [ref: > AspNetUsers.Id, note: 'Can be null for system actions']
  target_entity_type varchar(50) [note: 'e.g., Patient, AnalysisJob']
  target_entity_id varchar(255)
  action_type varchar(100) [not null, note: 'e.g., CREATE_PATIENT']
  details jsonb [note: 'Stores before/after values of a change']
  "timestamp" timestamptz [not null, default: `now()`]
}

//// ---------------------
//// Manual Index Definitions (for implementation reference)
//// ---------------------
// Note: These are for the final SQL script. dbdiagram.io infers indexes from PK/FK/Unique constraints.

// -- Core App Indexes
// CREATE INDEX idx_patients_clinic_name ON patients(clinic_id, last_name, first_name);
// CREATE INDEX idx_cal_profiles_clinic_active ON calibration_profiles(clinic_id, is_active);
// CREATE INDEX idx_analysis_sessions_patient_id ON analysis_sessions(patient_id);
// CREATE INDEX idx_analysis_jobs_session_id ON analysis_jobs(session_id);
// CREATE INDEX idx_analysis_jobs_status ON analysis_jobs(status);
// CREATE INDEX idx_analysis_jobs_queue ON analysis_jobs(status, created_at) WHERE status = 'pending';

// -- Operational Indexes
// CREATE INDEX idx_invitations_email ON clinic_invitations(email);
// CREATE INDEX idx_audit_logs_user_id ON audit_logs(user_id);
// CREATE INDEX idx_audit_logs_entity_type_id ON audit_logs(target_entity_type, target_entity_id);
