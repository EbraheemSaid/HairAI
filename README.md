# HairAI SaaS Platform

A hardware-agnostic, AI-powered hair analysis SaaS platform for trichology and hair transplant clinics.

## Overview

HairAI is a sophisticated SaaS platform designed for trichology and hair transplant clinics. It leverages AI-powered analysis of trichoscope images to provide consistent, data-driven metrics to support doctors in diagnosis and treatment planning.

The platform automates the analysis of trichoscope images, providing key metrics such as hair count, density, follicular units, and more. It's designed to be hardware-agnostic, working with any trichoscope camera that can capture digital images.

## Architecture

The platform follows a microservices architecture with the following key components:

### 1. Backend API (.NET 8)
Central to the application, built with ASP.NET Core Web API following Clean Architecture principles. Manages business logic, data orchestration, security, and communication with other services.

### 2. Frontend Applications
- **Clinical Application**: Main application for doctors and clinic admins
- **Admin Dashboard**: SuperAdmin dashboard for platform management

Built with React, TypeScript, and Tailwind CSS with a comprehensive component library.

### 3. AI Worker (Python)
A dedicated background service for AI model inference, consuming analysis jobs from a message queue, performing object detection using a YOLOv8s ONNX model, and writing results back to the database.

### 4. Infrastructure
Containerized with Docker and orchestrated using Docker Compose:
- **Reverse Proxy (Nginx)**: Single, secure public entry point
- **Database (PostgreSQL)**: Single source of truth for all persistent data
- **Message Queue (RabbitMQ)**: Asynchronous communication backbone

## Key Features

### Authentication & Authorization
- User registration and login with JWT tokens
- Role-based access control (Doctor, ClinicAdmin, SuperAdmin)
- User invitation system

### Clinic Management
- Clinic creation and management
- Clinic settings and configuration
- User management within clinics

### Patient Management
- Patient registration and profile management
- Patient search and filtering
- Patient detail views with analysis history

### Calibration Profiles
- Camera calibration profile management
- Profile versioning and activation

### Analysis Workflow
- Trichoscope image upload and processing
- AI-powered hair analysis with key metrics
- Analysis session management
- Result visualization and reporting

### Subscription Management
- Subscription plan management
- Clinic subscription tracking
- Integration with payment gateway

### Admin Functions
- Platform-wide clinic management
- Subscription plan administration
- Manual subscription activation
- Payment logging and tracking

## Technology Stack

### Backend
- **Framework**: .NET 8 with ASP.NET Core Web API
- **Architecture**: Clean Architecture with CQRS pattern
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: JWT Bearer tokens with ASP.NET Core Identity
- **Messaging**: RabbitMQ for asynchronous job processing
- **External Services**: SendGrid (email), Paymob (payments)

### Frontend
- **Framework**: React with TypeScript
- **Styling**: Tailwind CSS
- **State Management**: Zustand
- **Routing**: React Router
- **API Communication**: Axios
- **Form Handling**: React Hook Form with Zod validation
- **UI Components**: Custom component library
- **Charts**: Recharts

### AI Worker
- **Language**: Python
- **AI Framework**: ONNX Runtime
- **Model**: YOLOv8s for object detection
- **Image Processing**: OpenCV
- **Clustering**: DBSCAN for follicular unit grouping

### Infrastructure
- **Containerization**: Docker
- **Orchestration**: Docker Compose
- **Reverse Proxy**: Nginx
- **Database**: PostgreSQL
- **Message Queue**: RabbitMQ

## Project Structure

```
HairAI/
├── Backend/              # .NET 8 Clean Architecture backend
│   ├── HairAI.Api/       # REST API controllers
│   ├── HairAI.Application/  # Business logic, CQRS, DTOs
│   ├── HairAI.Domain/     # Domain entities and enums
│   ├── HairAI.Infrastructure/  # EF Core, external services
│   └── HairAI.sln        # Solution file
├── Frontend/
│   ├── App/              # Main clinical application (React)
│   └── AdminDashboard/   # SuperAdmin dashboard (React)
├── AI_Worker/            # Python AI model inference
├── nginx/                # Nginx reverse proxy configuration
├── scripts/              # Helper scripts
└── docker-compose.yml    # Orchestration file
```

## Core Workflows

### 1. Clinic Onboarding
- SuperAdmin creates clinic for new clients
- Manual subscription activation for initial clients
- ClinicAdmin invites doctors and staff

### 2. Patient Analysis
- Doctor creates patient profile
- Sets up camera calibration profiles
- Creates analysis session for patient visit
- Uploads trichoscope images for each scalp region
- System processes images through AI worker
- Doctor reviews results and adds notes
- Final report generation for patient

### 3. Subscription Management
- Clinics can choose from multiple subscription plans
- Automated payment processing through Paymob
- Manual payment logging for cash transactions
- Subscription status tracking and renewal

## Database Schema

The PostgreSQL database is designed with the following key entities:

- **Clinics**: Top-level organization entity
- **Patients**: Linked to clinics with analysis history
- **Users**: Doctors, ClinicAdmins, and SuperAdmins
- **Calibration Profiles**: Camera settings for accurate measurements
- **Analysis Sessions**: Group multiple analyses for a patient visit
- **Analysis Jobs**: Individual image analysis with results
- **Subscription Plans**: Pricing tiers with feature limits
- **Subscriptions**: Clinic subscription status and billing
- **Payments**: Payment records for subscriptions
- **Invitations**: User onboarding system
- **Audit Logs**: System activity tracking

## API Documentation

The platform provides a comprehensive REST API with:

- Standardized response formats
- JWT-based authentication
- Role-based access control
- Swagger/OpenAPI documentation
- Complete endpoint coverage for all features

## Deployment

The platform is designed for containerized deployment using Docker and Docker Compose:

1. **Reverse Proxy (Nginx)**: Handles SSL termination and routing
2. **Backend API**: ASP.NET Core Web API
3. **Frontend Applications**: React applications
4. **AI Worker**: Python service for AI inference
5. **Database**: PostgreSQL for data persistence
6. **Message Queue**: RabbitMQ for job processing

## Security

The platform implements enterprise-grade security features:

- Multi-tenant data isolation
- JWT-based authentication with role-based access control
- Input validation and sanitization
- Secure password handling with ASP.NET Core Identity
- Audit logging for all system activities
- Rate limiting to prevent abuse
- Secure configuration management

## License

This project is licensed under the MIT License.