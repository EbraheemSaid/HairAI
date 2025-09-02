@echo off
REM Script to test the AI worker with proper environment variables

echo Starting HairAI services...
docker-compose up -d postgres rabbitmq

echo Waiting for services to be ready...
timeout /t 10 /nobreak >nul

echo Testing AI Worker with proper environment variables...
docker run --rm ^
  --network hairai_hairai-network ^
  -e RABBITMQ_CONNECTION_STRING="amqp://hairai_user:hairai_password@rabbitmq:5672" ^
  -e DATABASE_CONNECTION_STRING="Server=postgres;Port=5432;Database=hairai_db;User Id=hairai_user;Password=hairai_password;" ^
  hairai-ai-worker python worker.py