HairAI SaaS Platform - Project Context
This document provides a comprehensive overview of the HairAI project for AI-assisted development. It outlines the project's goals, technology stack, architecture, database schema, and key workflows.

1. Project Overview
Project: A hardware-agnostic, AI-powered hair analysis SaaS platform.

Target Audience: Trichology and hair transplant clinics, initially launching in the Egyptian market.

Core Functionality: The platform allows doctors to upload trichoscope images of a patient's scalp, which are then analyzed by a YOLOv8s AI model to provide key metrics. The system is designed to be a robust, scalable, and maintainable cloud service.

2. Technology Stack
Backend Framework: .NET 8 with ASP.NET Core Web API

Architecture: Clean Architecture with the CQRS (Command Query Responsibility Segregation) pattern.

Database: PostgreSQL

ORM: Entity Framework Core 8 (Code-First approach)

Asynchronous Processing: RabbitMQ message queue for decoupling the API from the AI worker.

AI Worker: A separate Python application responsible for model inference using ONNX Runtime on a CPU.

Containerization: The entire application stack (API, Worker, Database, Queue, Reverse Proxy) will be containerized using Docker and orchestrated with Docker Compose.

Authentication: ASP.NET Core Identity for user and role management.

3. Clean Architecture Principles
The project strictly follows Clean Architecture to ensure separation of concerns and maintainability. The dependency rule is key: dependencies only point inwards.

HairAI.Domain (Core): Contains only the C# entity classes (POCOs) and domain-specific enums. It has zero dependencies on any other project.

HairAI.Application (Business Logic): Contains all business logic, use cases (organized as CQRS features), DTOs, and interfaces for external services (e.g., IApplicationDbContext, IQueueService, IEmailService). It depends only on HairAI.Domain.

HairAI.Infrastructure (Implementation): Contains the concrete implementations of the interfaces defined in the Application layer. This includes the ApplicationDbContext (using EF Core), services for RabbitMQ, email, payments, and the implementation of ASP.NET Core Identity. It depends on HairAI.Application.

HairAI.Api (Presentation): The ASP.NET Core Web API project. It contains "thin" controllers whose only job is to handle HTTP requests, send commands/queries to the Application layer (via MediatR), and return responses. It depends on HairAI.Application and HairAI.Infrastructure.

4. Complete Project Structure (File Blueprint)
HairAI.Domain
/Entities
    - Clinic.cs, Patient.cs, CalibrationProfile.cs, AnalysisSession.cs, AnalysisJob.cs, SubscriptionPlan.cs, Subscription.cs, Payment.cs, ClinicInvitation.cs, AuditLog.cs
/Enums
    - JobStatus.cs, SubscriptionStatus.cs, PaymentStatus.cs, InvitationStatus.cs

HairAI.Application
/Common/Interfaces
    - IApplicationDbContext.cs, IIdentityService.cs, IQueueService.cs, IPaymentGateway.cs, IEmailService.cs, ICurrentUserService.cs
/DTOs
    - ClinicDto.cs, PatientDto.cs, AnalysisSessionDto.cs, etc.
/Features
    /Auth
        /Commands/Register: RegisterCommand.cs, RegisterCommandHandler.cs, RegisterCommandValidator.cs
        /Queries/Login: LoginQuery.cs, LoginQueryHandler.cs, LoginQueryValidator.cs
    /Clinics
        /Commands/CreateClinic: (Command, Handler, Validator)
        /Commands/UpdateClinic: (Command, Handler, Validator)
        /Queries/GetClinicById: (Query, Handler)
        /Queries/GetAllClinics: (Query, Handler)
    /Patients
        /Commands/CreatePatient: (Command, Handler, Validator)
        /Commands/UpdatePatient: (Command, Handler, Validator)
        /Queries/GetPatientById: (Query, Handler)
        /Queries/GetPatientsForClinic: (Query, Handler)
    /Calibration
        /Commands/CreateCalibrationProfile: (Command, Handler, Validator)
        /Commands/UpdateCalibrationProfile: (Command, Handler, Validator)
        /Commands/DeactivateCalibrationProfile: (Command, Handler)
        /Queries/GetActiveProfilesForClinic: (Query, Handler)
    /Analysis
        /Commands/CreateAnalysisSession: (Command, Handler)
        /Commands/UploadAnalysisImage: (Command, Handler)
        /Commands/AddDoctorNotes: (Command, Handler)
        /Commands/GenerateFinalReport: (Command, Handler)
        /Queries/GetAnalysisJobStatus: (Query, Handler)
        /Queries/GetAnalysisSessionDetails: (Query, Handler)
        /Queries/GetAnalysisJobResult: (Query, Handler)
    /Subscriptions
        /Commands/CreateSubscription: (Command, Handler)
        /Commands/CancelSubscription: (Command, Handler)
        /Queries/GetSubscriptionForClinic: (Query, Handler)
        /Queries/GetAllPlans: (Query, Handler)
    /Payments
        /Commands/HandlePaymentWebhook: (Command, Handler)
    /Invitations
        /Commands/CreateInvitation: (Command, Handler)
        /Commands/AcceptInvitation: (Command, Handler)
        /Queries/GetInvitationByToken: (Query, Handler)
    /Admin
        /Commands/ManuallyCreateClinic: (Command, Handler)
        /Commands/ManuallyActivateSubscription: (Command, Handler)
        /Commands/ManuallyLogPayment: (Command, Handler)

HairAI.Infrastructure
/Persistence
    - ApplicationDbContext.cs
    /Configurations: (A configuration class for each entity)
    /Migrations: (Auto-generated by EF Core)
/Services
    - RabbitMqService.cs, SendGridEmailService.cs, PaymobService.cs
/Identity
    - IdentityService.cs
    - ApplicationUser.cs

HairAI.Api
/Controllers
    - AuthController.cs, ClinicsController.cs, PatientsController.cs, CalibrationController.cs, AnalysisController.cs, SubscriptionsController.cs, PaymentsController.cs, InvitationsController.cs, AdminController.cs
/Services
    - CurrentUserService.cs
- Program.cs (Handles Dependency Injection)

5. Database Schema Overview
Identity: Standard ASP.NET Core Identity tables (AspNetUsers, AspNetRoles, etc.) manage users. AspNetUsers is extended with FirstName, LastName, and a nullable ClinicId.

Core Entities: Clinics is the parent for Patients and CalibrationProfiles.

Analysis Workflow: An AnalysisSession is the parent record for a patient's visit. It groups multiple AnalysisJobs. Each AnalysisJob represents a single image analysis, has a location_tag (e.g., "Crown"), and is linked to a specific CalibrationProfile.

Subscriptions: Clinics have a Subscription, which is linked to a SubscriptionPlan. Payments are recorded against a Subscription.

Operational: ClinicInvitations manages adding new users to a clinic. AuditLogs track all significant actions in the system.

6. Key Workflows
Core Analysis Loop: A doctor logs in and starts a new AnalysisSession for a patient. They select a location on a 3D head model (e.g., "Donor Area") and upload one or more images, selecting the appropriate CalibrationProfile for each. The API creates an AnalysisJob record for each image and publishes its ID to RabbitMQ. The Python worker consumes the job, performs the analysis, saves the annotated image, and updates the job status and results in the database. The frontend polls for the status and displays the annotated image and data upon completion. Finally, the doctor can generate a report which aggregates data from all jobs within the session.

Manual Admin Onboarding: A SuperAdmin (me) can use the AdminController endpoints to manually create a Clinic, create and activate a Subscription for that clinic, and log a Payment for offline transactions (cash or bank transfer). This bypasses the need for the clinic to use the automated payment gateway initially.