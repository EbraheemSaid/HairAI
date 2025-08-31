# HairAI SaaS Platform

A hardware-agnostic, AI-powered hair analysis SaaS platform for trichology and hair transplant clinics.

## Project Status: ✅ MOSTLY COMPLETE

The HairAI platform has been successfully architected and largely implemented as a comprehensive SaaS solution for hair analysis in clinical settings.

## Overview

HairAI is a sophisticated SaaS platform designed for trichology and hair transplant clinics. It leverages AI-powered analysis of trichoscope images to provide consistent, data-driven metrics to support doctors in diagnosis and treatment planning.

The platform automates the analysis of trichoscope images, providing key metrics such as hair count, density, follicular units, and more. It's designed to be hardware-agnostic, working with any trichoscope camera that can capture digital images.

## Architecture

The platform follows a microservices architecture with the following key components:

### 1. Backend API (.NET 8) ✅ COMPLETE
Central to the application, built with ASP.NET Core Web API following Clean Architecture principles. Manages business logic, data orchestration, security, and communication with other services.

### 2. Frontend Applications ✅ MOSTLY COMPLETE
- **Clinical Application**: Main application for doctors and clinic admins
- **Admin Dashboard**: SuperAdmin dashboard for platform management

Built with React, TypeScript, and Tailwind CSS with a comprehensive component library.

### 3. AI Worker (Python) 🚧 READY FOR IMPLEMENTATION
A dedicated background service for AI model inference, consuming analysis jobs from a message queue, performing object detection using a YOLOv8s ONNX model, and writing results back to the database.

### 4. Infrastructure ✅ COMPLETE
Containerized with Docker and orchestrated using Docker Compose:
- **Reverse Proxy (Nginx)**: Single, secure public entry point
- **Database (PostgreSQL)**: Single source of truth for all persistent data
- **Message Queue (RabbitMQ)**: Asynchronous communication backbone

## Key Features

### Authentication & Authorization ✅
- User registration and login with JWT tokens
- Role-based access control (Doctor, ClinicAdmin, SuperAdmin)
- User invitation system

### Clinic Management ✅
- Clinic creation and management
- Clinic settings and configuration
- User management within clinics

### Patient Management ✅
- Patient registration and profile management
- Patient search and filtering
- Patient detail views with analysis history

### Calibration Profiles ✅
- Camera calibration profile management
- Profile versioning and activation

### Analysis Workflow ✅
- Trichoscope image upload and processing
- AI-powered hair analysis with key metrics
- Analysis session management
- Result visualization and reporting

### Subscription Management ✅
- Subscription plan management
- Clinic subscription tracking
- Integration with payment gateway

### Admin Functions ✅
- Platform-wide clinic management
- Subscription plan administration
- Manual subscription activation
- Payment logging and tracking

## Current Implementation Status

### Backend ✅ 100% Complete
- Clean Architecture implementation with Domain, Application, Infrastructure, and API layers
- Complete CQRS with Commands/Queries for all features
- Entity Framework Core with all configurations
- REST API with comprehensive endpoints
- External service integrations (RabbitMQ, SendGrid, Paymob)
- JWT authentication and role-based authorization
- Swagger/OpenAPI documentation

### Frontend ✅ 90% Complete
- Complete project structure for both applications
- Core infrastructure (state management, API service, routing)
- Comprehensive UI component library
- All required page components created
- Form validation and submission handling
- Responsive design with Tailwind CSS
- Admin Dashboard implementation

### Infrastructure ✅ 100% Complete
- Docker configuration for all services
- Nginx reverse proxy setup
- Docker Compose orchestration
- CI/CD pipeline configuration

### AI Worker 🚧 70% Complete
- Project structure and dependencies
- Framework for AI model integration
- Ready for Python implementation

## Getting Started

### Prerequisites
- Docker
- Docker Compose

### Quick Start
```bash
# Clone the repository
git clone <repository-url>
cd HairAI

# Start all services
docker-compose up -d

# Access the applications:
# - Main Application: http://localhost
# - Admin Dashboard: http://localhost/admin
# - API Documentation: http://localhost/api/docs

# Alternatively, start only the backend API and database (for API testing):
# - Windows: start-api-db.bat
# - Linux/Mac: ./start-api-db.sh

# To stop the services:
# - All services: docker-compose down
# - Backend API and database only: stop-api-db.bat (Windows) or ./stop-api-db.sh (Linux/Mac)
```

## Starting the Backend API and Database Only

For development and testing purposes, you can start just the backend API and database containers without the other services:

### Windows
```cmd
start-api-db.bat
```

### Linux/Mac
```bash
./start-api-db.sh
```

### Manual Docker Commands
You can also start the services manually using Docker Compose:

```bash
# Start the services
docker-compose -f docker-compose.api.yml up -d

# Check the status of the containers
docker-compose -f docker-compose.api.yml ps

# View logs
docker-compose -f docker-compose.api.yml logs

# Stop the services
docker-compose -f docker-compose.api.yml down
```

Once started, the services will be available at:
- **Backend API**: http://localhost:5000
- **Database**: localhost:5432 (PostgreSQL)

You can test the API endpoints using the provided Postman collection or curl commands.

## Development

### Project Structure
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

### Development Scripts
```bash
# Start development environment
npm run dev

# Build all services
npm run build

# Run tests
npm run test

# Start Docker services
npm run docker:up

# View Docker logs
npm run docker:logs

# Start only Backend API and Database containers
./start-api-db.sh       # On Linux/Mac
start-api-db.bat        # On Windows

# Stop Backend API and Database containers
./stop-api-db.sh        # On Linux/Mac
stop-api-db.bat         # On Windows

# Test bootstrap-admin endpoint (for initial SuperAdmin creation)
./test-bootstrap-admin.sh  # On Linux/Mac
test-bootstrap-admin.bat   # On Windows
```

### API Documentation

Detailed API documentation is available in the following files:
- `API-DOCUMENTATION.md` - Complete API documentation with all endpoints and workflows
- `ADMIN-GUIDE.md` - Guide for administrators on setting up and managing the platform
- `HairAI-Postman-Collection-Full.json` - Postman collection with all API endpoints for testing

### API Documentation

Detailed API documentation is available in the following files:
- `API-DOCUMENTATION.md` - Complete API documentation with all endpoints
- `ADMIN-GUIDE.md` - Guide for administrators on setting up and managing the platform
- `HairAI-Postman-Collection-Full.json` - Postman collection with all API endpoints for testing

## Deployment Architecture

The platform is designed for containerized deployment using Docker and Docker Compose:

1. **Reverse Proxy (Nginx)**: Single entry point handling SSL termination and routing
2. **Backend API**: ASP.NET Core Web API with Clean Architecture
3. **Frontend Applications**: React applications for clinical and admin use
4. **AI Worker**: Python service for AI model inference
5. **Database**: PostgreSQL for data persistence
6. **Message Queue**: RabbitMQ for asynchronous job processing

## Roadmap to Production

### Phase 1: Integration (2-3 weeks)
1. Connect frontend to backend API
2. Implement AI worker functionality
3. Test end-to-end workflows

### Phase 2: Quality Assurance (2-3 weeks)
1. Implement comprehensive testing
2. Conduct security audit
3. Optimize performance

### Phase 3: Documentation (1 week)
1. Create user documentation
2. Develop administrator guides
3. Prepare training materials

### Phase 4: Production Deployment (1 week)
1. Configure production environment
2. Set up monitoring and alerting
3. Deploy to production

## Next Steps

1. **Integrate backend and frontend APIs**
2. **Implement AI worker functionality**
3. **Conduct comprehensive testing**
4. **Create detailed documentation**
5. **Deploy to production environment**

## License

This project is licensed under the MIT License.