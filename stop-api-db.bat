@echo off
echo Stopping HairAI Backend API and Database containers...
docker-compose -f docker-compose.api.yml down
echo Containers stopped successfully!