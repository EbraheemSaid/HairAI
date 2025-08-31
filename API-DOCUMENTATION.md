# HairAI API Endpoints and Workflow

## Overview
This document provides a comprehensive overview of the HairAI API endpoints, their functionality, and the overall workflow of the system. It also verifies that the Postman collection matches the implemented API endpoints.

## Authentication & Authorization
The HairAI API uses JWT tokens for authentication and role-based access control (RBAC) with three roles:
1. SuperAdmin - Platform administrators with full access
2. ClinicAdmin - Clinic administrators with clinic-specific access
3. Doctor - Clinic users with limited access to patient and analysis functions

## API Endpoints

### 1. Authentication (`/api/auth`)
- **POST `/api/auth/register`** - Register a new user account
- **POST `/api/auth/login`** - Login and obtain JWT token
- **POST `/api/auth/bootstrap-admin`** - Bootstrap first SuperAdmin user (Development only)

### 2. Admin Functions (`/api/admin`)
*Requires SuperAdmin role*

- **POST `/api/admin/clinics`** - Create a new clinic
- **POST `/api/admin/subscriptions`** - Manually activate a subscription for a clinic
- **POST `/api/admin/payments`** - Manually log a payment for a subscription
- **GET `/api/admin/users`** - Retrieve a list of all users in the system
- **POST `/api/admin/users`** - Create a new user account
- **DELETE `/api/admin/users/{id}`** - Delete a user account
- **PATCH `/api/admin/users/{id}/activate`** - Activate a user account
- **PATCH `/api/admin/users/{id}/deactivate`** - Deactivate a user account

### 3. Clinics (`/api/clinics`)
- **GET `/api/clinics`** - Retrieve a list of all clinics
- **GET `/api/clinics/{id}`** - Retrieve details of a specific clinic
- **POST `/api/clinics`** - Create a new clinic
- **PUT `/api/clinics/{id}`** - Update an existing clinic

### 4. Patients (`/api/patients`)
- **GET `/api/patients`** - Retrieve a list of patients for a specific clinic
- **GET `/api/patients/{id}`** - Retrieve details of a specific patient
- **POST `/api/patients`** - Create a new patient
- **PUT `/api/patients/{id}`** - Update an existing patient

### 5. Calibration Profiles (`/api/calibration`)
- **GET `/api/calibration/active`** - Retrieve active calibration profiles for a specific clinic
- **POST `/api/calibration`** - Create a new calibration profile
- **PUT `/api/calibration/{id}`** - Update an existing calibration profile
- **DELETE `/api/calibration/{id}`** - Deactivate a calibration profile

### 6. Analysis (`/api/analysis`)
- **GET `/api/analysis/sessions`** - Get paginated list of analysis sessions with filtering
- **GET `/api/analysis/session/{id}`** - Get analysis session details
- **GET `/api/analysis/job/{id}/status`** - Get the status of a specific analysis job
- **GET `/api/analysis/job/{id}/result`** - Get the result of a completed analysis job
- **POST `/api/analysis/session`** - Create a new analysis session
- **POST `/api/analysis/job`** - Upload an image for analysis
- **POST `/api/analysis/job/{id}/notes`** - Add doctor notes to an analysis job
- **POST `/api/analysis/session/{id}/report`** - Generate a final report for an analysis session

### 7. Subscriptions (`/api/subscriptions`)
- **GET `/api/subscriptions/plans`** - Retrieve a list of all subscription plans
- **GET `/api/subscriptions/clinic/{id}`** - Retrieve subscription details for a specific clinic
- **POST `/api/subscriptions`** - Create a new subscription
- **DELETE `/api/subscriptions/{id}`** - Cancel an existing subscription

### 8. Invitations (`/api/invitations`)
- **GET `/api/invitations/{token}`** - Retrieve invitation details by token
- **POST `/api/invitations`** - Create a new invitation for a user to join a clinic
- **POST `/api/invitations/accept`** - Accept an invitation and create a user account

### 9. Payments (`/api/payments`)
- **POST `/api/payments/webhook`** - Handle payment webhook from payment gateway

### 10. Health (`/health`)
- **GET `/health`** - Check the health status of the API service

## Core Workflow

### 1. Platform Setup
1. Bootstrap SuperAdmin user via `/api/auth/bootstrap-admin` (Development only)
2. SuperAdmin creates clinics via `/api/admin/clinics`
3. SuperAdmin manually activates subscriptions via `/api/admin/subscriptions`
4. SuperAdmin logs payments via `/api/admin/payments`

### 2. Clinic User Management
1. ClinicAdmin invites users via `/api/invitations`
2. Invited users register and accept invitation via `/api/invitations/accept`
3. ClinicAdmin manages users via `/api/admin/users` endpoints

### 3. Patient Management
1. Clinic users create patients via `/api/patients`
2. Clinic users view and update patient information

### 4. Hair Analysis Workflow
1. Clinic users create calibration profiles via `/api/calibration`
2. Clinic users create analysis sessions via `/api/analysis/session`
3. Clinic users upload images for analysis via `/api/analysis/job`
4. System processes images through AI worker (asynchronous)
5. Clinic users check job status via `/api/analysis/job/{id}/status`
6. Clinic users view results via `/api/analysis/job/{id}/result`
7. Clinic users add notes via `/api/analysis/job/{id}/notes`
8. Clinic users generate final reports via `/api/analysis/session/{id}/report`

### 5. Subscription Management
1. ClinicAdmin views subscription plans via `/api/subscriptions/plans`
2. ClinicAdmin views clinic subscription via `/api/subscriptions/clinic/{id}`
3. Payments are processed through the payment gateway and webhook via `/api/payments/webhook`

## Postman Collection Verification

The HairAI-Postman-Collection-Full.json file matches all the implemented API endpoints:

✅ Authentication endpoints (register, login, bootstrap-admin)
✅ Admin functions (clinics, subscriptions, payments, user management)
✅ Clinics endpoints (CRUD operations)
✅ Patients endpoints (CRUD operations)
✅ Calibration profiles endpoints (CRUD operations + deactivate)
✅ Analysis endpoints (sessions, jobs, results, reports)
✅ Subscriptions endpoints (plans, clinic subscriptions, create, cancel)
✅ Invitations endpoints (get by token, create, accept)
✅ Payments endpoints (webhook)
✅ Health check endpoint

All endpoints in the Postman collection have:
- Correct HTTP methods
- Proper URL paths
- Example request bodies where applicable
- Descriptive documentation

## Running the Complete Platform

To run the entire HairAI platform:

```bash
# Start all services
docker-compose up -d

# Access the applications:
# - Main Application: http://localhost
# - Admin Dashboard: http://localhost/admin
# - API Documentation: http://localhost/api/docs
```

To run only the backend API and database (for API testing):

```bash
# Windows
start-api-db.bat

# Linux/Mac
./start-api-db.sh
```

The API will be available at: http://localhost:5000
The database will be available at: localhost:5432