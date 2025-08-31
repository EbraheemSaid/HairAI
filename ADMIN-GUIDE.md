# HairAI Admin Guide

This guide explains how to set up and manage the HairAI platform as an administrator.

## Initial Setup

### 1. Start the Platform

Start all services using Docker Compose:

```bash
docker-compose up -d
```

### 2. Create the SuperAdmin Account

The SuperAdmin account is the highest-level administrator in the system. To create it:

1. Make sure the backend API is running
2. Send a POST request to: `http://localhost:5000/api/auth/bootstrap-admin`

Note: This endpoint requires no body content in the request. It's a simple POST request with no parameters.

This will create a SuperAdmin user with:
- Email: `admin@hairai.com`
- Password: `SuperAdmin123!`

Note: This endpoint is only available in development environments and will:
1. Delete any existing user with the email `admin@hairai.com`
2. Create a new SuperAdmin user with the default password
3. Ensure all required roles exist (SuperAdmin, ClinicAdmin, Doctor)
4. Assign the SuperAdmin role to the new user

You can test this endpoint using the provided test scripts:
- On Linux/Mac: `./test-bootstrap-admin.sh`
- On Windows: `test-bootstrap-admin.bat`

### 3. Login as SuperAdmin

Use the credentials above to login:

POST to `http://localhost/api/auth/login` with:
```json
{
  "email": "admin@hairai.com",
  "password": "SuperAdmin123!"
}
```

Save the JWT token from the response for future requests.

## Managing Clinics

As a SuperAdmin, you can create and manage clinics.

### View Subscription Plans

First, you'll need to know what subscription plans are available:

GET `http://localhost/api/subscriptions/plans`

This will return a list of available plans with their IDs, names, prices, and features.

### Create a New Clinic

POST to `http://localhost/api/admin/clinics` with:
```json
{
  "name": "Clinic Name"
}
```

You'll receive a response with the clinic ID which you'll need for other operations.

### Set Up Subscription for a Clinic

To activate a subscription for a clinic:

POST to `http://localhost/api/admin/subscriptions` with:
```json
{
  "clinicId": "clinic-guid-here",
  "planId": "plan-guid-here",
  "currentPeriodStart": "2023-01-01T00:00:00.000Z",
  "currentPeriodEnd": "2023-12-31T00:00:00.000Z"
}
```

### Log a Payment for a Clinic

To manually log a payment:

POST to `http://localhost/api/admin/payments` with:
```json
{
  "subscriptionId": "subscription-guid-here",
  "amount": 99.99,
  "currency": "USD",
  "paymentGatewayReference": "PAY123456789"
}
```

## Managing Users

### View All Users

GET `http://localhost/api/admin/users`

### Create a User

POST to `http://localhost/api/admin/users` with:
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123!",
  "clinicId": "clinic-guid-here",
  "role": "Doctor"
}
```

Available roles:
- SuperAdmin
- ClinicAdmin
- Doctor

### Delete a User

DELETE `http://localhost/api/admin/users/{user-id}`

### Activate/Deactivate a User

To activate a user:
PATCH `http://localhost/api/admin/users/{user-id}/activate`

To deactivate a user:
PATCH `http://localhost/api/admin/users/{user-id}/deactivate`

## Complete Workflow: Adding a Clinic and Setting Up Subscription

Here's the complete step-by-step process for adding a new clinic and setting up its subscription:

### Step 1: Create the Clinic

POST to `http://localhost/api/admin/clinics` with:
```json
{
  "name": "New Dermatology Clinic"
}
```

Response:
```json
{
  "success": true,
  "message": "Clinic created successfully",
  "data": {
    "id": "clinic-guid-here",
    "name": "New Dermatology Clinic",
    "createdAt": "2023-06-15T10:30:00.000Z"
  }
}
```

Save the clinic ID for the next steps.

### Step 2: Get Subscription Plans

GET `http://localhost/api/subscriptions/plans`

Response:
```json
{
  "success": true,
  "message": "Subscription plans retrieved successfully",
  "data": [
    {
      "id": "plan-guid-basic",
      "name": "Basic Plan",
      "priceMonthly": 99.99,
      "currency": "USD",
      "maxUsers": 5,
      "maxAnalysesPerMonth": 100
    },
    {
      "id": "plan-guid-premium",
      "name": "Premium Plan",
      "priceMonthly": 199.99,
      "currency": "USD",
      "maxUsers": 20,
      "maxAnalysesPerMonth": 500
    }
  ]
}
```

Choose the appropriate plan ID for the clinic.

### Step 3: Activate Subscription

POST to `http://localhost/api/admin/subscriptions` with:
```json
{
  "clinicId": "clinic-guid-here",
  "planId": "plan-guid-premium",
  "currentPeriodStart": "2023-06-15T00:00:00.000Z",
  "currentPeriodEnd": "2024-06-14T00:00:00.000Z"
}
```

Response:
```json
{
  "success": true,
  "message": "Subscription activated successfully",
  "data": {
    "id": "subscription-guid-here",
    "clinicId": "clinic-guid-here",
    "planId": "plan-guid-premium",
    "status": "Active",
    "currentPeriodStart": "2023-06-15T00:00:00.000Z",
    "currentPeriodEnd": "2024-06-14T00:00:00.000Z"
  }
}
```

Save the subscription ID for the next step.

### Step 4: Log Payment

POST to `http://localhost/api/admin/payments` with:
```json
{
  "subscriptionId": "subscription-guid-here",
  "amount": 199.99,
  "currency": "USD",
  "paymentGatewayReference": "PAY123456789"
}
```

Response:
```json
{
  "success": true,
  "message": "Payment logged successfully",
  "data": {
    "id": "payment-guid-here",
    "subscriptionId": "subscription-guid-here",
    "amount": 199.99,
    "currency": "USD",
    "status": "Succeeded",
    "paymentGatewayReference": "PAY123456789",
    "processedAt": "2023-06-15T10:35:00.000Z"
  }
}
```

### Step 5: Create Clinic Admin User (Optional)

You may want to create an admin user for the clinic:

POST to `http://localhost/api/admin/users` with:
```json
{
  "firstName": "Clinic",
  "lastName": "Admin",
  "email": "admin@newdermatologyclinic.com",
  "password": "SecurePassword123!",
  "clinicId": "clinic-guid-here",
  "role": "ClinicAdmin"
}
```

## Using the Postman Collection

To make API testing easier, we've provided a Postman collection:

1. Open Postman
2. Import the `HairAI-Postman-Collection-Full.json` file
3. Update the `baseUrl` variable to match your environment (e.g., `http://localhost`)
4. After logging in, update the `token` variable with your JWT token
5. You can now test all API endpoints

## Security Best Practices

1. Change the default SuperAdmin password immediately after initial setup
2. Use strong, unique passwords for all accounts
3. Regularly review user accounts and permissions
4. Monitor API usage and logs for suspicious activity
5. Keep the platform updated with the latest security patches

## Troubleshooting

### Cannot access API endpoints

- Ensure all Docker services are running: `docker-compose ps`
- Check service logs: `docker-compose logs backend-api`
- Verify the base URL is correct

### Authentication issues

- Ensure you're including the JWT token in the Authorization header as `Bearer {token}`
- Check that the token hasn't expired (default 24 hours)
- Verify user account status is active

### Rate limiting

If you receive 429 errors, you're making too many requests:
- Wait for the rate limit window to reset
- Optimize your client application to make fewer requests
- Consider batching operations where possible