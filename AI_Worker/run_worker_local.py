import os
import sys

# Add the current directory to Python path
sys.path.append(os.path.dirname(os.path.abspath(__file__)))

# Set environment variables for local testing
os.environ['RABBITMQ_CONNECTION_STRING'] = 'amqp://hairai_user:hairai_password@localhost:5672'
os.environ['DATABASE_CONNECTION_STRING'] = 'postgresql://hairai_user:hairai_password@localhost:5432/hairai_db'

# Import and run the worker
from worker import AIWorker

if __name__ == "__main__":
    worker = AIWorker()
    try:
        worker.start()
    except KeyboardInterrupt:
        print("Stopping AI Worker...")
        worker.stop()