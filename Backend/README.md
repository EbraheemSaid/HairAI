# HairAI Backend

This is the backend API for the HairAI SaaS platform, built with .NET 8 and following Clean Architecture principles.

## Project Structure

- **HairAI.Domain**: Contains the domain entities and enums
- **HairAI.Application**: Contains the business logic, CQRS features, DTOs, and interfaces
- **HairAI.Infrastructure**: Contains the implementations of the interfaces defined in the Application layer
- **HairAI.Api**: The ASP.NET Core Web API project

## Prerequisites

- .NET 8 SDK
- Docker (for containerized deployment)

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

## Usage

Run the API locally:
```
cd HairAI.Api
dotnet run
```

## Docker

To build and run the API in Docker:
```
docker build -t hairai-api -f HairAI.Api/Dockerfile .
docker run -p 8080:80 hairai-api
```

## Completed Features

The backend has been fully implemented with all layers complete:

### Domain Layer (✅ Complete)
- All entity classes and enums based on the database schema

### Application Layer (✅ Complete)
- All commands and queries for every feature (Auth, Clinics, Patients, Calibration, Analysis, Subscriptions, Payments, Invitations, Admin)
- All command/query handlers and validators
- All necessary DTOs for data transfer
- Proper interfaces and dependency injection setup
- Validation for all inputs using FluentValidation

### Infrastructure Layer (✅ Complete)
- Entity Framework Core DbContext with all entity configurations
- Database migrations for all entities
- Identity service implementation using ASP.NET Core Identity
- External service implementations:
  - RabbitMQ service for message queuing
  - SendGrid service for email sending
  - Paymob service for payment processing
  - Current user service for tracking the current user

### API Layer (✅ Complete)
- Controllers for all features with proper routing and HTTP methods
- JWT authentication and role-based authorization
- Global exception handling middleware
- Standardized API response formatting
- Swagger documentation
- Health check endpoint
- CORS configuration

The backend is now complete and ready for integration with the frontend and AI worker components.