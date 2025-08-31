# HairAI Infrastructure

This is the infrastructure layer for the HairAI SaaS platform, implementing the interfaces defined in the Application layer.

## Project Structure

- **Persistence**: Contains the Entity Framework Core DbContext and entity configurations
- **Services**: Contains implementations of external services (RabbitMQ, SendGrid, Paymob)
- **Identity**: Contains the Identity service implementation
- **Migrations**: Contains Entity Framework Core migrations

## Prerequisites

- .NET 8 SDK
- PostgreSQL database
- RabbitMQ message queue
- SendGrid account (for email)
- Paymob account (for payments)

## Configuration

The infrastructure layer is configured through dependency injection in the main API project.

## Completed Features

The Infrastructure layer has been fully implemented with:

- **Entity Framework Core DbContext** with all entity configurations
- **Database migrations** for all entities
- **Identity service** implementation using ASP.NET Core Identity
- **External service implementations**:
  - RabbitMQ service for message queuing
  - SendGrid service for email sending
  - Paymob service for payment processing
  - Current user service for tracking the current user

All services are properly registered with dependency injection and ready for use.