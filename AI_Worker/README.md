# HairAI AI Worker

This is the AI Worker component of the HairAI SaaS platform. It's responsible for processing analysis jobs from the message queue using a YOLOv8s ONNX model.

## Prerequisites

- Python 3.9+
- Docker (for containerized deployment)

## Installation

1. Install the required Python packages:
   ```
   pip install -r requirements.txt
   ```

2. Make sure the ONNX model file is placed in the `models/` directory.

## Configuration

The worker is configured using environment variables:

- `RABBITMQ_CONNECTION_STRING`: Connection string for RabbitMQ
- `DATABASE_CONNECTION_STRING`: Connection string for PostgreSQL database

## Usage

Run the worker locally:
```
python worker.py
```

## Docker

To build and run the worker in Docker:
```
docker build -t hairai-ai-worker .
docker run hairai-ai-worker
```