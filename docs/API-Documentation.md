# HairAI Platform API Documentation

## Overview

The HairAI Platform is a hardware-agnostic, AI-powered hair analysis SaaS platform for trichology and hair transplant clinics. This API documentation provides comprehensive information about all available endpoints, request/response models, authentication requirements, and testing procedures.

## Base Information

- **Base URL**: `http://localhost:5000/api` (Development) / `https://yourdomain.com/api` (Production)
- **Content-Type**: `application/json`
- **Authentication**: JWT Bearer Token
- **API Version**: v1.0

## Project Status

✅ **Backend**: 100% Complete - Full Clean Architecture implementation with all features
✅ **Infrastructure**: 100% Complete - Docker containerization ready
🚧 **AI Worker**: 70% Complete - Framework ready, needs Python implementation
✅ **Frontend**: 90% Complete - React applications with comprehensive UI

## Authentication

Most endpoints require authentication using a JWT Bearer token. Include the token in the Authorization header:

```http
Authorization: Bearer {your-jwt-token}
```

### Roles

- **Doctor**: Basic user with access to patient management and analysis
- **ClinicAdmin**: Admin access within a clinic
- **SuperAdmin**: Platform-wide administrative access

## Standard Response Format

All API responses follow this standard format:

```json
{
  "success": true,
  "message": "Success message",
  "data": {
    /* Response data */
  },
  "errors": []
}
```

## Error Codes

- **200**: Success
- **400**: Bad Request (Validation errors)
- **401**: Unauthorized (Authentication required)
- **403**: Forbidden (Insufficient permissions)
- **404**: Not Found
- **500**: Internal Server Error

---

## 🔐 Authentication Endpoints

### Register User

Creates a new user account.

**Endpoint**: `POST /api/auth/register`  
**Authentication**: None  
**Role**: None

#### Request Body

```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@clinic.com",
  "password": "SecurePassword123!",
  "clinicId": "3fa85f64-5717-4562-b3fc-2c963f66afa6" // Optional
}
```

#### Response

```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "userId": "user-id-guid",
    "email": "john.doe@clinic.com",
    "firstName": "John",
    "lastName": "Doe"
  },
  "errors": []
}
```

### Login

Authenticates a user and returns a JWT token.

**Endpoint**: `POST /api/auth/login`  
**Authentication**: None  
**Role**: None

#### Request Body

```json
{
  "email": "john.doe@clinic.com",
  "password": "SecurePassword123!"
}
```

#### Response

```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "userId": "user-id-guid",
    "firstName": "John",
    "lastName": "Doe",
    "clinicId": "clinic-id-guid"
  },
  "errors": []
}
```

---

## 🏥 Clinic Management Endpoints

### Get All Clinics

Retrieves all clinics.

**Endpoint**: `GET /api/clinics`  
**Authentication**: Required  
**Role**: Any authenticated user

#### Response

```json
{
  "success": true,
  "message": "Clinics retrieved successfully",
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Advanced Hair Clinic",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": "2024-01-15T10:30:00Z"
    }
  ],
  "errors": []
}
```

### Get Clinic by ID

Retrieves a specific clinic by ID.

**Endpoint**: `GET /api/clinics/{id}`  
**Authentication**: Required  
**Role**: Any authenticated user

#### Response

```json
{
  "success": true,
  "message": "Clinic retrieved successfully",
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Advanced Hair Clinic",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z"
  },
  "errors": []
}
```

### Create Clinic

Creates a new clinic.

**Endpoint**: `POST /api/clinics`  
**Authentication**: Required  
**Role**: ClinicAdmin, SuperAdmin

#### Request Body

```json
{
  "name": "New Hair Clinic"
}
```

#### Response

```json
{
  "success": true,
  "message": "Clinic created successfully",
  "data": {
    "clinicId": "new-clinic-id-guid"
  },
  "errors": []
}
```

### Update Clinic

Updates an existing clinic.

**Endpoint**: `PUT /api/clinics/{id}`  
**Authentication**: Required  
**Role**: ClinicAdmin, SuperAdmin

#### Request Body

```json
{
  "name": "Updated Clinic Name"
}
```

#### Response

```json
{
  "success": true,
  "message": "Clinic updated successfully",
  "data": null,
  "errors": []
}
```

---

## 👥 Patient Management Endpoints

### Get Patients for Clinic

Retrieves all patients for a specific clinic.

**Endpoint**: `GET /api/patients?clinicId={clinicId}`  
**Authentication**: Required  
**Role**: Doctor, ClinicAdmin, SuperAdmin

#### Query Parameters

- `clinicId` (required): GUID of the clinic

#### Response

```json
{
  "success": true,
  "message": "Patients retrieved successfully",
  "data": [
    {
      "id": "patient-id-guid",
      "clinicId": "clinic-id-guid",
      "clinicPatientId": "P001",
      "firstName": "Jane",
      "lastName": "Smith",
      "dateOfBirth": "1985-03-15",
      "createdAt": "2024-01-15T10:30:00Z"
    }
  ],
  "errors": []
}
```

### Get Patient by ID

Retrieves a specific patient by ID.

**Endpoint**: `GET /api/patients/{id}`  
**Authentication**: Required  
**Role**: Doctor, ClinicAdmin, SuperAdmin

#### Response

```json
{
  "success": true,
  "message": "Patient retrieved successfully",
  "data": {
    "id": "patient-id-guid",
    "clinicId": "clinic-id-guid",
    "clinicPatientId": "P001",
    "firstName": "Jane",
    "lastName": "Smith",
    "dateOfBirth": "1985-03-15",
    "createdAt": "2024-01-15T10:30:00Z"
  },
  "errors": []
}
```

### Create Patient

Creates a new patient.

**Endpoint**: `POST /api/patients`  
**Authentication**: Required  
**Role**: Doctor, ClinicAdmin, SuperAdmin

#### Request Body

```json
{
  "clinicId": "clinic-id-guid",
  "clinicPatientId": "P001", // Optional
  "firstName": "Jane",
  "lastName": "Smith",
  "dateOfBirth": "1985-03-15" // Optional, format: YYYY-MM-DD
}
```

#### Response

```json
{
  "success": true,
  "message": "Patient created successfully",
  "data": {
    "patientId": "new-patient-id-guid"
  },
  "errors": []
}
```

### Update Patient

Updates an existing patient.

**Endpoint**: `PUT /api/patients/{id}`  
**Authentication**: Required  
**Role**: Doctor, ClinicAdmin, SuperAdmin

#### Request Body

```json
{
  "clinicPatientId": "P001-UPDATED",
  "firstName": "Jane Updated",
  "lastName": "Smith Updated",
  "dateOfBirth": "1985-03-15"
}
```

#### Response

```json
{
  "success": true,
  "message": "Patient updated successfully",
  "data": null,
  "errors": []
}
```

---

## ⚙️ Calibration Profile Endpoints

### Get Active Calibration Profiles

Retrieves active calibration profiles for a clinic.

**Endpoint**: `GET /api/calibration/active?clinicId={clinicId}`  
**Authentication**: Required  
**Role**: Doctor, ClinicAdmin, SuperAdmin

#### Query Parameters

- `clinicId` (required): GUID of the clinic

#### Response

```json
{
  "success": true,
  "message": "Active profiles retrieved successfully",
  "data": [
    {
      "id": "profile-id-guid",
      "clinicId": "clinic-id-guid",
      "profileName": "Trichoscope Camera v2.1",
      "calibrationData": "{\"pixels_per_mm\": 15.7, \"magnification\": 50}",
      "version": 1,
      "isActive": true,
      "createdAt": "2024-01-15T10:30:00Z"
    }
  ],
  "errors": []
}
```

### Create Calibration Profile

Creates a new calibration profile.

**Endpoint**: `POST /api/calibration`  
**Authentication**: Required  
**Role**: ClinicAdmin, SuperAdmin

#### Request Body

```json
{
  "clinicId": "clinic-id-guid",
  "profileName": "Trichoscope Camera v2.1",
  "calibrationData": {
    "pixels_per_mm": 15.7,
    "magnification": 50,
    "camera_model": "Professional Trichoscope"
  }
}
```

#### Response

```json
{
  "success": true,
  "message": "Calibration profile created successfully",
  "data": {
    "profileId": "new-profile-id-guid"
  },
  "errors": []
}
```

### Update Calibration Profile

Updates an existing calibration profile.

**Endpoint**: `PUT /api/calibration/{id}`  
**Authentication**: Required  
**Role**: ClinicAdmin, SuperAdmin

#### Request Body

```json
{
  "profileName": "Updated Camera Profile",
  "calibrationData": {
    "pixels_per_mm": 16.2,
    "magnification": 60
  }
}
```

#### Response

```json
{
  "success": true,
  "message": "Calibration profile updated successfully",
  "data": null,
  "errors": []
}
```

### Deactivate Calibration Profile

Deactivates a calibration profile.

**Endpoint**: `DELETE /api/calibration/{id}`  
**Authentication**: Required  
**Role**: ClinicAdmin, SuperAdmin

#### Response

```json
{
  "success": true,
  "message": "Calibration profile deactivated successfully",
  "data": null,
  "errors": []
}
```

---

## 🔬 Analysis Workflow Endpoints

### Create Analysis Session

Creates a new analysis session for a patient visit.

**Endpoint**: `POST /api/analysis/session`  
**Authentication**: Required  
**Role**: Doctor, ClinicAdmin, SuperAdmin

#### Request Body

```json
{
  "patientId": "patient-id-guid",
  "createdByUserId": "user-id-guid",
  "sessionDate": "2024-01-15" // Format: YYYY-MM-DD
}
```

#### Response

```json
{
  "success": true,
  "message": "Analysis session created successfully",
  "data": {
    "sessionId": "new-session-id-guid"
  },
  "errors": []
}
```

### Upload Analysis Image

Uploads an image for analysis within a session.

**Endpoint**: `POST /api/analysis/job`  
**Authentication**: Required  
**Role**: Doctor, ClinicAdmin, SuperAdmin

#### Request Body

```json
{
  "sessionId": "session-id-guid",
  "patientId": "patient-id-guid",
  "calibrationProfileId": "profile-id-guid",
  "createdByUserId": "user-id-guid",
  "locationTag": "Crown", // e.g., "Crown", "Donor Area", "Temporal Region"
  "imageStorageKey": "path/to/uploaded/image.jpg"
}
```

#### Response

```json
{
  "success": true,
  "message": "Analysis job created successfully",
  "data": {
    "jobId": "new-job-id-guid"
  },
  "errors": []
}
```

### Get Analysis Session Details

Retrieves details of an analysis session including all jobs.

**Endpoint**: `GET /api/analysis/session/{sessionId}`  
**Authentication**: Required  
**Role**: Doctor, ClinicAdmin, SuperAdmin

#### Response

```json
{
  "success": true,
  "message": "Session details retrieved successfully",
  "data": {
    "id": "session-id-guid",
    "patientId": "patient-id-guid",
    "createdByUserId": "user-id-guid",
    "sessionDate": "2024-01-15",
    "status": "in_progress",
    "finalReportData": null,
    "createdAt": "2024-01-15T10:30:00Z",
    "analysisJobs": [
      {
        "id": "job-id-guid",
        "sessionId": "session-id-guid",
        "patientId": "patient-id-guid",
        "calibrationProfileId": "profile-id-guid",
        "createdByUserId": "user-id-guid",
        "locationTag": "Crown",
        "imageStorageKey": "path/to/image.jpg",
        "annotatedImageKey": "path/to/annotated_image.jpg",
        "status": "completed",
        "analysisResult": "{\"hair_count\": 145, \"density\": 180, \"follicular_units\": 72}",
        "doctorNotes": "Good hair density in crown region",
        "createdAt": "2024-01-15T10:30:00Z",
        "startedAt": "2024-01-15T10:31:00Z",
        "completedAt": "2024-01-15T10:35:00Z",
        "errorMessage": null,
        "processingTimeMs": 240000
      }
    ]
  },
  "errors": []
}
```

### Get Analysis Job Status

Retrieves the status of a specific analysis job.

**Endpoint**: `GET /api/analysis/job/{jobId}/status`  
**Authentication**: Required  
**Role**: Doctor, ClinicAdmin, SuperAdmin

#### Response

```json
{
  "success": true,
  "message": "Job status retrieved successfully",
  "data": {
    "jobId": "job-id-guid",
    "status": "completed", // pending, processing, completed, failed
    "startedAt": "2024-01-15T10:31:00Z",
    "completedAt": "2024-01-15T10:35:00Z",
    "processingTimeMs": 240000,
    "errorMessage": null
  },
  "errors": []
}
```

### Get Analysis Job Result

Retrieves the complete result of an analysis job.

**Endpoint**: `GET /api/analysis/job/{jobId}/result`  
**Authentication**: Required  
**Role**: Doctor, ClinicAdmin, SuperAdmin

#### Response

```json
{
  "success": true,
  "message": "Job result retrieved successfully",
  "data": {
    "jobId": "job-id-guid",
    "status": "completed",
    "analysisResult": {
      "hair_count": 145,
      "density_per_cm2": 180,
      "follicular_units": 72,
      "average_hair_thickness": 0.08,
      "hair_distribution": "uniform",
      "scalp_visibility": "minimal"
    },
    "annotatedImageKey": "path/to/annotated_image.jpg",
    "doctorNotes": "Good hair density in crown region",
    "completedAt": "2024-01-15T10:35:00Z"
  },
  "errors": []
}
```

### Add Doctor Notes

Adds doctor notes to an analysis job.

**Endpoint**: `POST /api/analysis/job/{jobId}/notes`  
**Authentication**: Required  
**Role**: Doctor, ClinicAdmin, SuperAdmin

#### Request Body

```json
"Good hair density observed. Recommend maintenance treatment."
```

#### Response

```json
{
  "success": true,
  "message": "Doctor notes added successfully",
  "data": null,
  "errors": []
}
```

### Generate Final Report

Generates a final report for an analysis session.

**Endpoint**: `POST /api/analysis/session/{sessionId}/report`  
**Authentication**: Required  
**Role**: Doctor, ClinicAdmin, SuperAdmin

#### Response

```json
{
  "success": true,
  "message": "Final report generated successfully",
  "data": {
    "reportData": {
      "session_summary": {
        "total_jobs": 3,
        "analyzed_regions": ["Crown", "Donor Area", "Temporal Region"],
        "overall_hair_density": 165,
        "average_follicular_units": 68
      },
      "recommendations": "Based on analysis, patient shows good hair density..."
    }
  },
  "errors": []
}
```

---

## 📋 Subscription Management Endpoints

### Get All Subscription Plans

Retrieves all available subscription plans.

**Endpoint**: `GET /api/subscriptions/plans`  
**Authentication**: Required  
**Role**: Any authenticated user

#### Response

```json
{
  "success": true,
  "message": "Subscription plans retrieved successfully",
  "data": [
    {
      "id": "plan-id-guid",
      "name": "Basic Plan",
      "priceMonthly": 299.99,
      "currency": "EGP",
      "maxUsers": 5,
      "maxAnalysesPerMonth": 100
    },
    {
      "id": "plan-id-guid-2",
      "name": "Professional Plan",
      "priceMonthly": 599.99,
      "currency": "EGP",
      "maxUsers": 15,
      "maxAnalysesPerMonth": 500
    }
  ],
  "errors": []
}
```

### Get Subscription for Clinic

Retrieves the subscription for a specific clinic.

**Endpoint**: `GET /api/subscriptions/clinic/{clinicId}`  
**Authentication**: Required  
**Role**: ClinicAdmin, SuperAdmin

#### Response

```json
{
  "success": true,
  "message": "Clinic subscription retrieved successfully",
  "data": {
    "id": "subscription-id-guid",
    "clinicId": "clinic-id-guid",
    "planId": "plan-id-guid",
    "status": "active",
    "currentPeriodStart": "2024-01-01T00:00:00Z",
    "currentPeriodEnd": "2024-02-01T00:00:00Z"
  },
  "errors": []
}
```

### Create Subscription

Creates a new subscription for a clinic.

**Endpoint**: `POST /api/subscriptions`  
**Authentication**: Required  
**Role**: ClinicAdmin, SuperAdmin

#### Request Body

```json
{
  "clinicId": "clinic-id-guid",
  "planId": "plan-id-guid"
}
```

#### Response

```json
{
  "success": true,
  "message": "Subscription created successfully",
  "data": {
    "subscriptionId": "new-subscription-id-guid"
  },
  "errors": []
}
```

### Cancel Subscription

Cancels a subscription.

**Endpoint**: `DELETE /api/subscriptions/{subscriptionId}`  
**Authentication**: Required  
**Role**: ClinicAdmin, SuperAdmin

#### Response

```json
{
  "success": true,
  "message": "Subscription cancelled successfully",
  "data": null,
  "errors": []
}
```

---

## 💳 Payment Endpoints

### Handle Payment Webhook

Handles payment webhooks from Paymob payment gateway.

**Endpoint**: `POST /api/payments/webhook`  
**Authentication**: Required  
**Role**: System (webhook)

#### Request Body

```json
{
  "subscriptionId": "subscription-id-guid",
  "amount": 299.99,
  "currency": "EGP",
  "paymentGatewayReference": "paymob-transaction-id",
  "status": "succeeded"
}
```

#### Response

```json
{
  "success": true,
  "message": "Payment webhook processed successfully",
  "data": null,
  "errors": []
}
```

---

## 📧 Invitation Management Endpoints

### Get Invitation by Token

Retrieves an invitation by its token.

**Endpoint**: `GET /api/invitations/{token}`  
**Authentication**: None  
**Role**: None

#### Response

```json
{
  "success": true,
  "message": "Invitation retrieved successfully",
  "data": {
    "id": "invitation-id-guid",
    "clinicId": "clinic-id-guid",
    "invitedByUserId": "user-id-guid",
    "email": "newuser@clinic.com",
    "role": "Doctor",
    "status": "pending",
    "expiresAt": "2024-01-22T10:30:00Z"
  },
  "errors": []
}
```

### Create Invitation

Creates a new invitation to join a clinic.

**Endpoint**: `POST /api/invitations`  
**Authentication**: Required  
**Role**: ClinicAdmin, SuperAdmin

#### Request Body

```json
{
  "clinicId": "clinic-id-guid",
  "email": "newuser@clinic.com",
  "role": "Doctor"
}
```

#### Response

```json
{
  "success": true,
  "message": "Invitation created successfully",
  "data": {
    "invitationId": "new-invitation-id-guid",
    "token": "invitation-token-string"
  },
  "errors": []
}
```

### Accept Invitation

Accepts an invitation and creates a user account.

**Endpoint**: `POST /api/invitations/accept`  
**Authentication**: Required  
**Role**: Any authenticated user

#### Request Body

```json
{
  "token": "invitation-token-string",
  "firstName": "New",
  "lastName": "User",
  "password": "SecurePassword123!"
}
```

#### Response

```json
{
  "success": true,
  "message": "Invitation accepted successfully",
  "data": {
    "userId": "new-user-id-guid"
  },
  "errors": []
}
```

---

## 👑 Admin Endpoints

### Manually Create Clinic

Manually creates a clinic (SuperAdmin only).

**Endpoint**: `POST /api/admin/clinics`  
**Authentication**: Required  
**Role**: SuperAdmin

#### Request Body

```json
{
  "name": "Manually Created Clinic",
  "adminEmail": "admin@newclinic.com",
  "adminFirstName": "Clinic",
  "adminLastName": "Administrator"
}
```

#### Response

```json
{
  "success": true,
  "message": "Clinic created manually",
  "data": {
    "clinicId": "new-clinic-id-guid",
    "adminUserId": "new-admin-user-id-guid"
  },
  "errors": []
}
```

### Manually Activate Subscription

Manually activates a subscription (SuperAdmin only).

**Endpoint**: `POST /api/admin/subscriptions`  
**Authentication**: Required  
**Role**: SuperAdmin

#### Request Body

```json
{
  "clinicId": "clinic-id-guid",
  "planId": "plan-id-guid",
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-02-01T00:00:00Z"
}
```

#### Response

```json
{
  "success": true,
  "message": "Subscription activated manually",
  "data": {
    "subscriptionId": "new-subscription-id-guid"
  },
  "errors": []
}
```

### Manually Log Payment

Manually logs a payment (SuperAdmin only).

**Endpoint**: `POST /api/admin/payments`  
**Authentication**: Required  
**Role**: SuperAdmin

#### Request Body

```json
{
  "subscriptionId": "subscription-id-guid",
  "amount": 299.99,
  "currency": "EGP",
  "paymentMethod": "Bank Transfer",
  "reference": "BANK-TXN-12345"
}
```

#### Response

```json
{
  "success": true,
  "message": "Payment logged manually",
  "data": {
    "paymentId": "new-payment-id-guid"
  },
  "errors": []
}
```

---

## 🏥 Health Check Endpoint

### Health Check

Checks the health status of the API.

**Endpoint**: `GET /api/health`  
**Authentication**: None  
**Role**: None

#### Response

```json
{
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

## Testing Guidelines

### 1. Authentication Testing

Start by testing the authentication flow:

1. Register a new user
2. Login to get a JWT token
3. Use the token for authenticated requests

### 2. Complete Workflow Testing

Test the complete hair analysis workflow:

1. Create/login as a doctor user
2. Create a clinic (or use existing)
3. Create a patient
4. Create a calibration profile
5. Create an analysis session
6. Upload images for analysis
7. Check job status and results
8. Add doctor notes
9. Generate final report

### 3. Admin Testing

Test admin functions:

1. Login as SuperAdmin
2. Create clinic manually
3. Activate subscription manually
4. Log payment manually

### 4. Error Testing

Test error scenarios:

- Invalid authentication tokens
- Missing required fields
- Invalid GUIDs
- Unauthorized access attempts

---

## Rate Limiting

- **Authentication endpoints**: 10 requests per minute
- **File upload endpoints**: 5 requests per minute
- **Other endpoints**: 100 requests per minute

---

## Next Steps for Production

1. **Complete AI Worker Implementation**: Implement Python AI model inference
2. **Frontend Integration**: Connect React applications to these APIs
3. **End-to-End Testing**: Test complete user workflows
4. **Security Hardening**: Implement additional security measures
5. **Performance Optimization**: Add caching and query optimization
6. **Monitoring**: Set up logging and monitoring systems

---

## Support

For API support or issues, please contact the development team or refer to the project documentation in the repository.
