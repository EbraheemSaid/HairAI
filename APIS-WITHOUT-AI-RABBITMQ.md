# HairAI API Endpoints That Don't Require AI Processing or RabbitMQ

## Authentication & User Management
These endpoints work independently and don't require AI or RabbitMQ:

1. **POST `/api/auth/bootstrap-admin`** - Bootstrap first SuperAdmin user (Development only)
2. **POST `/api/auth/register`** - Register a new user account
3. **POST `/api/auth/login`** - Login and obtain JWT token

## Admin Functions (SuperAdmin Only)
These endpoints work independently and don't require AI or RabbitMQ:

1. **POST `/api/admin/clinics`** - Create a new clinic
2. **POST `/api/admin/subscriptions`** - Manually activate a subscription for a clinic
3. **POST `/api/admin/payments`** - Manually log a payment for a subscription
4. **GET `/api/admin/users`** - Retrieve a list of all users in the system
5. **POST `/api/admin/users`** - Create a new user account
6. **DELETE `/api/admin/users/{id}`** - Delete a user account
7. **PATCH `/api/admin/users/{id}/activate`** - Activate a user account
8. **PATCH `/api/admin/users/{id}/deactivate`** - Deactivate a user account

## Clinic Management
These endpoints work independently and don't require AI or RabbitMQ:

1. **GET `/api/clinics`** - Retrieve a list of all clinics
2. **GET `/api/clinics/{id}`** - Retrieve details of a specific clinic
3. **POST `/api/clinics`** - Create a new clinic
4. **PUT `/api/clinics/{id}`** - Update an existing clinic

## Patient Management
These endpoints work independently and don't require AI or RabbitMQ:

1. **GET `/api/patients`** - Retrieve a list of patients for a specific clinic
2. **GET `/api/patients/{id}`** - Retrieve details of a specific patient
3. **POST `/api/patients`** - Create a new patient
4. **PUT `/api/patients/{id}`** - Update an existing patient

## Calibration Profile Management
These endpoints work independently and don't require AI or RabbitMQ:

1. **GET `/api/calibration/active`** - Retrieve active calibration profiles for a specific clinic
2. **POST `/api/calibration`** - Create a new calibration profile
3. **PUT `/api/calibration/{id}`** - Update an existing calibration profile
4. **DELETE `/api/calibration/{id}`** - Deactivate a calibration profile

## Subscription Management
These endpoints work independently and don't require AI or RabbitMQ:

1. **GET `/api/subscriptions/plans`** - Retrieve a list of all subscription plans
2. **GET `/api/subscriptions/clinic/{id}`** - Retrieve subscription details for a specific clinic
3. **POST `/api/subscriptions`** - Create a new subscription
4. **DELETE `/api/subscriptions/{id}`** - Cancel an existing subscription

## Invitation Management
These endpoints work independently and don't require AI or RabbitMQ:

1. **GET `/api/invitations/{token}`** - Retrieve invitation details by token
2. **POST `/api/invitations`** - Create a new invitation for a user to join a clinic
3. **POST `/api/invitations/accept`** - Accept an invitation and create a user account

## Health Check
This endpoint works independently and doesn't require AI or RabbitMQ:

1. **GET `/health`** - Check the health status of the API service

## Endpoints That DO Require AI Processing or RabbitMQ
The following endpoints require the AI worker and RabbitMQ to function properly:

1. **GET `/api/analysis/sessions`** - Get paginated list of analysis sessions
2. **GET `/api/analysis/session/{id}`** - Get analysis session details
3. **GET `/api/analysis/job/{id}/status`** - Get the status of a specific analysis job
4. **GET `/api/analysis/job/{id}/result`** - Get the result of a completed analysis job
5. **POST `/api/analysis/session`** - Create a new analysis session
6. **POST `/api/analysis/job`** - Upload an image for analysis (This creates a job that gets queued)
7. **POST `/api/analysis/job/{id}/notes`** - Add doctor notes to an analysis job
8. **POST `/api/analysis/session/{id}/report`** - Generate a final report for an analysis session
9. **POST `/api/payments/webhook`** - Handle payment webhook from payment gateway

## Summary
Most of the platform's functionality is available without the AI worker or RabbitMQ running. The only features that require these components are related to the actual image analysis workflow and payment processing.