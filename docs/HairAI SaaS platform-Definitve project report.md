HairAI SaaS Platform: Definitive Project Report
Report Date: Tuesday, August 12, 2025
Status: Final Pre-Development Specification
Version: 1.0

1. Executive Summary
This document outlines the complete technical specification and strategic plan for the HairAI SaaS Platform. The project is currently in the final planning stage, with a robust and detailed architecture, database schema, and development roadmap defined. The system is designed as a modern, scalable, and maintainable cloud service tailored for trichology and hair transplant clinics. The initial launch will be focused on the Egyptian market, with a foundational design that supports future global expansion.

2. Core Objective
To develop and launch a hardware-agnostic, AI-powered hair analysis platform that provides clinics with a powerful, cost-effective tool for clinical analysis and administration. The system will automate the analysis of trichoscope images, providing consistent, data-driven metrics to support doctors in diagnosis and treatment planning.

3. System Architecture: A Microservice Approach
The platform is built on a containerized microservice architecture to ensure scalability, resilience, and separation of concerns. The entire system will be orchestrated using Docker Compose.

There are five core containerized services:

Reverse Proxy (Nginx): The single, secure public entry point for all web traffic. It handles SSL/TLS termination (HTTPS) and routes incoming requests to the appropriate backend or frontend service based on the URL path (e.g., /api, /admin).

Backend API (.NET 8): The central brain of the application, built with ASP.NET Core Web API following Clean Architecture principles. It manages all business logic, data orchestration, security (using ASP.NET Core Identity), and communication with other services.

AI Worker (Python): A dedicated background service for AI model inference. It consumes analysis jobs from the message queue, performs object detection using a YOLOv8s ONNX model optimized for CPU execution, and writes the results back to the database.

Database (PostgreSQL): The single source of truth for all persistent application data. It stores all relational data for clinics, patients, analysis results, subscriptions, and more.

Message Queue (RabbitMQ): The asynchronous communication backbone. It provides a durable queue to hold analysis jobs, ensuring the API remains fast and responsive by offloading time-consuming AI processing to the AI Worker.

4. Key Workflows
4.1 Core Analysis Workflow
This is the primary user journey for a doctor using the platform:

Session Start: The doctor initiates a new "Analysis Session" for a specific patient. This creates a parent analysis_sessions record in the database.

Image Upload: The doctor selects a specific location on a 3D head model (e.g., "Crown," "Donor Area") and uploads one or more trichoscope images for that area. For each image, they must also select a pre-configured CalibrationProfile (e.g., "50x Zoom") to ensure accurate measurements.

Job Creation & Queuing: For each uploaded image, the Backend API creates a corresponding analysis_jobs record, linking it to the parent session, patient, and calibration profile. It saves the original image to the server's file system and publishes the analysis_job_id to the RabbitMQ queue.

AI Processing: The Python AI Worker, listening to the queue, picks up the job. It retrieves the job details from the database, loads the original image, performs the AI analysis, saves a new annotated version of the image, and updates the analysis_jobs record with the results, annotated image path, and a "completed" status.

Result Display: The frontend application polls the Backend API for the job status. Once completed, it retrieves and displays the annotated image and the individual analysis metrics.

Report Generation: After all images for the session have been analyzed, the doctor can generate a final report. The backend aggregates the data from all analysis_jobs within the analysis_sessions to calculate overall metrics (e.g., total estimated grafts, average density across all areas).

4.2 User & Subscription Management
The system supports two parallel onboarding flows:

Automated Flow: A clinic can sign up, choose a subscription plan, and pay directly through the integrated Paymob payment gateway.

Manual Flow: For initial clients or offline payments (cash/bank transfer), a SuperAdmin (the project owner) can use a dedicated Admin Dashboard to manually create a clinic, assign a subscription plan, set the active period, and log the payment.

4.3 Clinic User Invitation
A user with a "ClinicAdmin" role can invite new users (e.g., doctors, technicians) to their clinic. The system generates a secure, token-based invitation link and sends it via email. When the new user signs up using this link, their account is automatically associated with the correct clinic and role.

5. Database Schema
The finalized PostgreSQL schema is designed to be robust and normalized. It is fully integrated with ASP.NET Core Identity.

Identity & Access: Standard AspNetUsers, AspNetRoles, etc., with AspNetUsers extended to include FirstName, LastName, and a nullable ClinicId.

Core Application:

clinics: Top-level entity for each client organization.

patients: Linked to a clinic.

calibration_profiles: Stores camera settings with versioning (version, is_active) to ensure historical data integrity.

analysis_sessions: The parent record for a patient's visit, grouping multiple analyses.

analysis_jobs: The child record for a single image analysis, containing detailed status tracking (started_at, completed_at, error_message) and links to its parent session and calibration profile.

Subscription & Billing: subscription_plans, subscriptions, and payments tables manage the SaaS billing model.

Operational & Auditing: clinic_invitations and audit_logs provide operational and security tracking.

6. Backend Development Strategy
Approach: Code-First with Entity Framework Core 8.

Architecture: A strict Clean Architecture pattern is followed to ensure maintainability and testability.

HairAI.Domain: Contains only the C# entity classes and enums. No external dependencies.

HairAI.Application: Contains all business logic, organized by feature using the CQRS pattern. It defines interfaces for all external services. Depends only on Domain.

HairAI.Infrastructure: Contains the concrete implementations of the interfaces defined in the Application layer, including the EF Core DbContext, RabbitMQ services, and Identity services. Depends on Application.

HairAI.Api: The presentation layer containing "thin" ASP.NET Core controllers that handle HTTP requests and delegate work to the Application layer.

7. Deployment & Project Structure
The entire application will be deployed to a DigitalOcean VPS using Docker.

Top-Level Structure: The project is organized into top-level folders: Backend/, Frontend/, AI_Worker/, and nginx/.

Containerization: docker-compose.yml orchestrates the five required containers: backend-api, ai-worker, postgres-db, rabbitmq, and nginx-proxy.

Performance: The initial deployment on a standard $24/month DigitalOcean droplet (2 vCPU, 4GB RAM) is estimated to comfortably handle 2-4 concurrent doctors. The architecture is designed for easy vertical scaling (upgrading to a CPU-Optimized Droplet) as user load increases.

8. Conclusion
The HairAI platform has been meticulously planned with a modern, robust, and scalable architecture. The detailed database schema, well-defined workflows, and clear development strategy provide a solid foundation for the development phase. The project is ready to move from planning to implementation.