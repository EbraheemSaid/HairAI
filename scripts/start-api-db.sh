#!/bin/bash
echo "Starting HairAI Backend API and Database containers..."
docker-compose -f docker-compose.api.yml up -d postgres backend-api
echo "Containers started successfully!"
echo ""
echo "Backend API is available at: http://localhost:5000"
echo "Database is available at: localhost:5432"
echo ""
echo "To stop the containers, run: docker-compose -f docker-compose.api.yml down"