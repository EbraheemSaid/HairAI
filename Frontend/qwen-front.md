Prompt for AI Frontend Developer: Build the HairAI SaaS Platform
1. Your Role and Primary Goal
You are an expert senior frontend developer tasked with building a complete, production-ready, and user-friendly web application for the "HairAI" SaaS platform. Your primary goal is to create a responsive, robust, and intuitive user interface based on the detailed specifications and backend API structure provided below. The application will be used by clinic staff with varying technical skills, so clarity and ease of use are paramount.

2. Project Overview
Project: HairAI - A hardware-agnostic, AI-powered hair analysis SaaS platform.

Target Audience: Trichology and hair transplant clinics in Egypt.

Core Functionality: The platform enables doctors to upload trichoscope images of a patient's scalp. These images are processed by a backend AI model to provide critical analysis metrics (e.g., hair density, graft estimation). The frontend's job is to facilitate this entire workflow, from patient management to displaying the final analysis results in a clear and actionable format.

3. Frontend Technology Stack & General Requirements
Framework: React (Vite) for a fast development environment and optimized builds.

Language: TypeScript for type safety and maintainability.

Styling: Tailwind CSS for a utility-first, responsive design.

UI Components: Shadcn/UI (or a similar headless component library like Radix) to ensure consistent, accessible, and high-quality UI elements (buttons, forms, modals, tables, etc.).

State Management: Zustand (or React Context) for managing global state, such as authentication tokens and user information.

Data Fetching: Axios (or react-query/tanstack-query) for handling API requests to the backend. Create a centralized API client that automatically attaches the JWT token to authorized requests.

Forms: React Hook Form for robust and performant form handling with validation.

Charts/Visualizations: Recharts (or a similar library) for displaying analysis data and dashboard metrics.

Notifications: Use a library like react-hot-toast to provide non-intrusive feedback to the user (e.g., "Patient created successfully," "Error," "Analysis complete").

Localization: The initial UI must be in English. However, structure the code (e.g., using JSON files for strings) to easily support Arabic translation in the future.

4. General Application Layout
The application should have a modern dashboard layout:

Persistent Left Sidebar: This will be the main navigation hub. It should be collapsible. The links displayed will change based on the user's role (SuperAdmin, ClinicAdmin, Doctor).

Top Header Bar: Displays the name of the logged-in user, their clinic, and a dropdown menu with a "Logout" button.

Main Content Area: This is where the different pages and components will be rendered.

5. User Roles & Conditional UI
The frontend must dynamically adapt its UI based on the logged-in user's role.

Doctor (Base User): Can manage patients and perform hair analysis.

ClinicAdmin: Has all Doctor permissions, plus the ability to manage clinic settings, invite new users, and view subscription details.

SuperAdmin: Has access to a separate, global admin panel to manage all clinics and subscriptions across the platform.

6. Detailed Page & Feature Breakdown
6.1 Authentication
/login: A clean, centered form with "Email" and "Password" fields. On successful login, store the returned JWT token securely (e.g., in an HttpOnly cookie or local storage) and redirect the user to their dashboard.

/register: A multi-step form for new clinics to sign up. It should include fields for clinic name, contact info, and the primary user's details (First Name, Last Name, Email, Password).

/invitations/accept?token=[TOKEN]: A dedicated page for invited users. It should validate the token from the URL parameter. The user will be asked to set their FirstName, LastName, and Password to complete their account creation, which will be automatically linked to the inviting clinic.

6.2 Main Dashboard (/dashboard)
This is the landing page after login.

It should display a summary of recent activity.

For Doctors/Admins: Show recent patients, upcoming appointments (if applicable), and a shortcut to start a new analysis.

For SuperAdmins: Show platform-wide statistics (total clinics, active subscriptions, total revenue).

6.3 Patient Management
/patients:

View: A data table listing all patients for the clinic. Columns: Patient Name, ID/Code, Date of Birth, Last Visit. The table must be searchable and sortable.

Action: A prominent "Add New Patient" button that opens a modal or navigates to a new page.

Form (/patients/new): A form to create a new patient with fields for name, contact details, DOB, etc.

/patients/[patientId]: A detailed view for a single patient, showing their personal information and a history of all their analysis sessions.

6.4 Calibration Profile Management (ClinicAdmin only)
/settings/calibration:

View: A table listing all calibration profiles for the clinic (e.g., "50x Zoom," "100x Zoom"). Columns: Profile Name, Version, Active Status.

Action: A button to "Add New Profile."

Form: A modal form to create a new profile, requiring a Profile Name.

6.5 The Core Analysis Workflow
This is the most critical user journey. It must be seamless and intuitive.

Step 1: Start Session (/analysis/new)

The user selects a patient from a searchable dropdown list.

Upon selection, a new "Analysis Session" is created in the backend, and the user is navigated to the main analysis page: /analysis/session/[sessionId].

Step 2: The Analysis Workspace (/analysis/session/[sessionId])

The page is divided into two main sections:

Left Panel: Interactive Head Model & Job List

Display a simple, interactive 3D head model (you can use a library like react-three-fiber or even a static 2D image with clickable hotspots for the initial version). The key areas are "Crown," "Donor Area," etc.

Below the model, show a list of all analysis jobs for this session, grouped by area. Each job entry should display its status: Pending, Processing, Completed, or Error.

Right Panel: Uploader & Results Viewer

When a user clicks an area on the head model, this panel updates.

It shows a large file upload zone.

A dropdown menu to select the CalibrationProfile is mandatory for each upload.

The user can drag-and-drop or select one or more trichoscope images.

On upload, each image immediately appears in the "Job List" on the left with a Processing status. Implement polling (setInterval) on a GET /api/Analysis/GetAnalysisJobStatus endpoint for each processing job.

When a job status changes to Completed, the entry in the job list should become clickable.

Step 3: Viewing Results

When the user clicks a Completed job, the right panel switches to a result view.

It must display the annotated image returned from the backend.

Below the image, display a clean table of the analysis metrics (e.g., "Total Hairs," "Density," "Follicular Units").

Step 4: Report Generation

Once all jobs are completed, a "Generate Final Report" button at the top of the page becomes active.

Clicking this will trigger the backend to aggregate all data. The frontend should then display a printable, well-formatted report page (/analysis/session/[sessionId]/report) summarizing the results from all analyzed areas and providing overall metrics (like total estimated grafts).

6.6 Clinic Management (ClinicAdmin only)
/settings/clinic: A form to update the clinic's details (name, address, etc.).

/settings/users: A table of all users in the clinic. It should have a prominent "Invite New User" button that opens a modal to send an invitation via email.

/settings/subscription: A page showing the clinic's current subscription plan, status (Active, Expired), and billing history.

6.7 SuperAdmin Panel (/admin)
This section should have its own distinct navigation within the sidebar, visible only to the SuperAdmin.

/admin/clinics: A data table of all clinics on the platform. Allows the admin to view details of any clinic.

/admin/clinics/new: A form for the SuperAdmin to manually create a new clinic.

/admin/subscriptions: A dashboard to manage subscriptions. The admin should be able to select a clinic and manually activate a subscription plan for them and log offline payments (cash/bank transfer).

7. Final Deliverables
The complete, well-organized React source code.

A README.md file with clear instructions on how to install dependencies (npm install) and run the project locally (npm run dev).

The code must be clean, commented where necessary, and follow modern React best practices.