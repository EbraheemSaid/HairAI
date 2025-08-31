# HairAI API

This is the ASP.NET Core Web API project for the HairAI SaaS platform.

## Project Structure

- **Controllers**: API controllers for all features
- **Middleware**: Custom middleware components
- **Filters**: Action filters for request/response processing
- **Services**: API-specific services
- **Common**: Shared classes and utilities

## Prerequisites

- .NET 8 SDK
- PostgreSQL database
- RabbitMQ message queue
- SendGrid account (for email)
- Paymob account (for payments)

## Configuration

The API is configured using appsettings.json and environment variables:

- `ConnectionStrings:DefaultConnection`: PostgreSQL database connection string
- `ConnectionStrings:RabbitMQConnection`: RabbitMQ connection string
- `Paymob:ApiKey`: Paymob API key
- `Paymob:IntegrationId`: Paymob integration ID
- `SendGrid:ApiKey`: SendGrid API key
- `Jwt:Key`: JWT secret key
- `Jwt:Issuer`: JWT issuer
- `Jwt:Audience`: JWT audience
- `Jwt:ExpireDays`: JWT expiration in days

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and obtain a JWT token

### Clinics
- `GET /api/clinics` - Get all clinics
- `GET /api/clinics/{id}` - Get a specific clinic
- `POST /api/clinics` - Create a new clinic
- `PUT /api/clinics/{id}` - Update a clinic

### Patients
- `GET /api/patients?clinicId={id}` - Get all patients for a clinic
- `GET /api/patients/{id}` - Get a specific patient
- `POST /api/patients` - Create a new patient
- `PUT /api/patients/{id}` - Update a patient

### Calibration Profiles
- `GET /api/calibration/active?clinicId={id}` - Get active calibration profiles for a clinic
- `POST /api/calibration` - Create a new calibration profile
- `PUT /api/calibration/{id}` - Update a calibration profile
- `DELETE /api/calibration/{id}` - Deactivate a calibration profile

### Analysis
- `GET /api/analysis/session/{id}` - Get analysis session details
- `GET /api/analysis/job/{id}/status` - Get analysis job status
- `GET /api/analysis/job/{id}/result` - Get analysis job result
- `POST /api/analysis/session` - Create a new analysis session
- `POST /api/analysis/job` - Upload an analysis image
- `POST /api/analysis/job/{id}/notes` - Add doctor notes to an analysis job
- `POST /api/analysis/session/{id}/report` - Generate a final report for a session

### Subscriptions
- `GET /api/subscriptions/plans` - Get all subscription plans
- `GET /api/subscriptions/clinic/{id}` - Get subscription for a clinic
- `POST /api/subscriptions` - Create a new subscription
- `DELETE /api/subscriptions/{id}` - Cancel a subscription

### Payments
- `POST /api/payments/webhook` - Handle payment webhook

### Invitations
- `GET /api/invitations/{token}` - Get invitation by token
- `POST /api/invitations` - Create a new invitation
- `POST /api/invitations/accept` - Accept an invitation

### Admin
- `POST /api/admin/clinics` - Manually create a clinic
- `POST /api/admin/subscriptions` - Manually activate a subscription
- `POST /api/admin/payments` - Manually log a payment

## Authentication

Most endpoints require authentication using a JWT token. Include the token in the Authorization header:

```
Authorization: Bearer {token}
```

Admin endpoints require the user to have the "SuperAdmin" role.

## Error Handling

The API uses a global exception handler middleware to catch and format errors consistently:

- Validation errors return 400 Bad Request with details
- Authorization errors return 401 Unauthorized or 403 Forbidden
- Server errors return 500 Internal Server Error

## Response Format

All API responses follow a standard format:

```json
{
  "success": true,
  "message": "Success message",
  "data": { },
  "errors": []
}
```

## Running the API

To run the API locally:

```bash
cd HairAI.Api
dotnet run
```

The API will be available at `https://localhost:5001` or `http://localhost:5000`.

## Swagger Documentation

Swagger documentation is available at `/swagger` when running in development mode.