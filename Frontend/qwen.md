# HairAI SaaS Platform - Complete Project Context

This document provides a comprehensive overview of the HairAI project for AI-assisted development. It consolidates information from the backend definitive project report, database schema, and frontend development context.

## 1. Project Overview

### 1.1 Core Objective

HairAI is a hardware-agnostic, AI-powered hair analysis SaaS platform designed for trichology and hair transplant clinics. The primary goal is to automate the analysis of trichoscope images, providing consistent, data-driven metrics to support doctors in diagnosis and treatment planning. The initial launch targets the Egyptian market, with a foundational design for future global expansion.

### 1.2 System Architecture: A Microservice Approach

The platform is built on a containerized microservice architecture using Docker Compose for orchestration. The core services are:

- **Reverse Proxy (Nginx)**: The single, secure public entry point for all web traffic, handling SSL/TLS termination and routing.
- **Backend API (.NET 8)**: Central to the application, built with ASP.NET Core Web API following Clean Architecture principles. Manages business logic, data orchestration, security, and communication with other services.
- **AI Worker (Python)**: A dedicated background service for AI model inference, consuming analysis jobs from a message queue, performing object detection using a YOLOv8s ONNX model, and writing results back to the database.
- **Database (PostgreSQL)**: The single source of truth for all persistent application data.
- **Message Queue (RabbitMQ)**: The asynchronous communication backbone, providing a durable queue for analysis jobs.

### 1.3 Frontend Technology Stack

The frontend is a React application built with:

- **Framework**: React (Vite)
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **UI Components**: Shadcn/UI (or similar headless component library)
- **State Management**: Zustand (or React Context)
- **Data Fetching**: Axios (or react-query/tanstack-query)
- **Forms**: React Hook Form
- **Charts/Visualizations**: Recharts (or similar library)
- **Notifications**: react-hot-toast
- **Routing**: React Router

## 2. Key Workflows

### 2.1 Core Analysis Workflow

This is the primary user journey for a doctor:

1. **Session Start**: The doctor initiates a new "Analysis Session" for a specific patient.
2. **Image Upload**: The doctor selects a specific location on a 3D head model (e.g., "Crown," "Donor Area") and uploads one or more trichoscope images for that area, selecting a `CalibrationProfile`.
3. **Job Creation & Queuing**: For each uploaded image, the Backend API creates an `analysis_jobs` record and publishes the job ID to the RabbitMQ queue.
4. **AI Processing**: The Python AI Worker picks up the job, performs the AI analysis, saves an annotated image, and updates the job record with results.
5. **Result Display**: The frontend polls the Backend API for job status and displays the annotated image and metrics upon completion.
6. **Report Generation**: After all images for the session are analyzed, the doctor can generate a final report aggregating data from all jobs.

### 2.2 User & Subscription Management

Supports two onboarding flows:

- **Automated Flow**: Clinics can sign up, choose a subscription plan, and pay directly through Paymob.
- **Manual Flow**: A SuperAdmin can manually create clinics, assign subscription plans, and log payments via the Admin Dashboard.

### 2.3 Clinic User Invitation

ClinicAdmins can invite new users. The system generates a secure, token-based invitation link sent via email. New users signing up through this link are automatically associated with the correct clinic and role.

## 3. Database Schema

The PostgreSQL database schema is designed to be robust and normalized, integrated with ASP.NET Core Identity.

### 3.1 Identity & Access

- `AspNetRoles`, `AspNetUsers` (extended with `FirstName`, `LastName`, `ClinicId`), `AspNetUserRoles`

### 3.2 Core Application Entities

- `clinics`: Top-level entity for each client organization.
- `patients`: Linked to a clinic.
- `calibration_profiles`: Stores camera settings with versioning.
- `analysis_sessions`: Groups multiple analyses into a single patient visit/report.
- `analysis_jobs`: Represents a single image analysis, containing detailed status tracking and links to its parent session and calibration profile.

### 3.3 Subscription & Billing

- `subscription_plans`, `subscriptions`, and `payments` tables manage the SaaS billing model.

### 3.4 Operational & Auditing

- `clinic_invitations` and `audit_logs` provide operational and security tracking.

## 4. Backend Development Strategy

### 4.1 Architecture

A strict Clean Architecture pattern is followed:

- **HairAI.Domain**: Pure C# entity classes and enums.
- **HairAI.Application**: Business logic organized by feature using CQRS, defining interfaces for external services.
- **HairAI.Infrastructure**: Concrete implementations of interfaces (EF Core DbContext, RabbitMQ services, Identity services).
- **HairAI.Api**: Thin ASP.NET Core controllers handling HTTP requests and delegating work.

### 4.2 Approach

- **Code-First with Entity Framework Core 8**: The database schema is defined through C# code and migrations.
- **Deployment**: Containerized with Docker and orchestrated using Docker Compose, deployable to a DigitalOcean VPS.

## 5. Frontend Development Context

### 5.1 General Application Layout

- **Persistent Left Sidebar**: Main navigation hub, collapsible, with role-based links.
- **Top Header Bar**: Displays user/clinic info and logout.
- **Main Content Area**: Renders pages and components.

### 5.2 User Roles & Conditional UI

- **Doctor (Base User)**: Manage patients and perform hair analysis.
- **ClinicAdmin**: All Doctor permissions, plus clinic settings, user management, and subscription details.
- **SuperAdmin**: Access to a global admin panel to manage all clinics and subscriptions.

### 5.3 Detailed Page & Feature Breakdown

The frontend includes pages for:

- **Authentication**: Login, Registration, Invitation Acceptance.
- **Dashboard**: Role-specific summaries.
- **Patient Management**: List, create, and view patient details.
- **Calibration Profile Management** (ClinicAdmin): Manage clinic-specific calibration profiles.
- **Core Analysis Workflow**: Interactive head model, image upload, job tracking, result viewing, and report generation.
- **Clinic Management** (ClinicAdmin): Update clinic info, manage users, view subscription.
- **SuperAdmin Panel**: Manage all clinics and subscriptions.

### 5.4 Folder Structure

```
HairAI/
├── .github/
│   └── workflows/
│       └── ci-cd.yml
├── Backend/
│   ├── HairAI.Api/
│   ├── HairAI.Application/
│   ├── HairAI.Domain/
│   ├── HairAI.Infrastructure/
│   ├── HairAI.sln
│   └── Dockerfile
├── Frontend/
│   ├── App/                  # The main clinical application
│   │   └── ... (src, package.json, Dockerfile)
│   └── AdminDashboard/       # The SuperAdmin dashboard
│       └── ... (src, package.json, Dockerfile)
├── AI_Worker/
│   ├── models/
│   ├── worker.py
│   ├── requirements.txt
│   └── Dockerfile
├── nginx/                  # <-- HERE IT IS
│   └── nginx.conf          # The configuration file for routing
├── scripts/
│   └── ...
├── .dockerignore
├── .gitignore
├── docker-compose.yml
├── qwen.md
└── README.md

This consolidated context provides a complete picture of the HairAI SaaS platform, covering both the backend architecture and the frontend implementation plan.
```
