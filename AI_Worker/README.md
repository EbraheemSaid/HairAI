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

2. Make sure the ONNX model files are placed in the `models/` directory:
   - `model_quantized_v1.onnx`
   - `model_quantized_v2.onnx` (default)

## Configuration

The worker is configured using environment variables:

- `RABBITMQ_CONNECTION_STRING`: Connection string for RabbitMQ
- `DATABASE_CONNECTION_STRING`: Connection string for PostgreSQL database

## Usage

Run the worker locally:
```
python worker.py
```

## Testing

To test the ONNX model loading:
```
python test_model.py
```

To create a test image:
```
python test_worker.py
```

## Docker

To build and run the worker in Docker:
```
docker build -t hairai-ai-worker .
docker run hairai-ai-worker
```

## Features

The AI Worker now includes:

1. **ONNX Model Inference**: Supports both v1 and v2 quantized models
2. **Image Processing**: Loads and processes trichoscope images
3. **Follicle Detection**: Identifies different types of hair follicles
4. **Clustering**: Groups individual hairs into follicular units using DBSCAN
5. **Metrics Calculation**: Computes all required hair analysis metrics:
   - Follicular Unit Density
   - Average Hairs per Follicular Unit
   - Average Hair Shaft Thickness
   - Follicular Unit Breakdown (Single, Double, Triple+)
   - Other Detections (Vellus, Abnormal)
6. **Annotated Image Generation**: Creates visual representations of analysis results
7. **Database Integration**: Updates job status and stores results
8. **Error Handling**: Comprehensive error handling and logging