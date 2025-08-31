#!/bin/bash

# HairAI Development Environment Setup Script

echo "Setting up HairAI development environment..."

# Check if Docker is installed
if ! command -v docker &> /dev/null
then
    echo "Docker is not installed. Please install Docker first."
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null
then
    echo "Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

# Create necessary directories
echo "Creating directories..."
mkdir -p ./Backend/HairAI.Api/wwwroot/images
mkdir -p ./Backend/HairAI.Api/wwwroot/reports
mkdir -p ./AI_Worker/models

# Build Docker images
echo "Building Docker images..."
docker-compose build

# Start services
echo "Starting services..."
docker-compose up -d

# Wait for services to start
echo "Waiting for services to start..."
sleep 30

# Check if services are running
echo "Checking service status..."
docker-compose ps

echo "Development environment setup complete!"
echo ""
echo "Access the applications:"
echo "- Main Application: http://localhost"
echo "- Admin Dashboard: http://localhost/admin"
echo "- API Documentation: http://localhost/api/docs"
echo "- RabbitMQ Management: http://localhost:15672 (guest/guest)"
echo "- PostgreSQL: localhost:5432 (hairai_user/hairai_password)"
echo ""
echo "To view logs: docker-compose logs -f"
echo "To stop services: docker-compose down"