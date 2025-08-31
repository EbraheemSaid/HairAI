import os
import pika
import json
import psycopg2
from PIL import Image
import numpy as np
import onnxruntime as ort

class AIWorker:
    def __init__(self):
        # Get configuration from environment variables
        self.rabbitmq_connection_string = os.getenv('RABBITMQ_CONNECTION_STRING')
        self.database_connection_string = os.getenv('DATABASE_CONNECTION_STRING')
        
        # Initialize ONNX model
        self.model = ort.InferenceSession('models/hair_model.onnx')
        
        # Connect to RabbitMQ
        self.connection = pika.BlockingConnection(
            pika.URLParameters(self.rabbitmq_connection_string)
        )
        self.channel = self.connection.channel()
        self.channel.queue_declare(queue='analysis_jobs', durable=True)
        
    def process_job(self, job_id):
        """
        Process a single analysis job
        """
        # In a real implementation, we would:
        # 1. Retrieve job details from the database
        # 2. Load the image
        # 3. Run the AI model on the image
        # 4. Save the annotated image
        # 5. Update the job status and results in the database
        
        print(f"Processing job {job_id}")
        
        # This is a simplified implementation for demonstration
        # In reality, you would implement the actual AI processing logic here
        
        return {
            "hair_count": 1500,
            "density": 85.5,
            "diameter": 0.075
        }
    
    def callback(self, ch, method, properties, body):
        """
        Callback function for RabbitMQ messages
        """
        try:
            message = json.loads(body)
            job_id = message.get('JobId')
            
            if job_id:
                # Process the job
                results = self.process_job(job_id)
                
                # Update database with results
                self.update_job_results(job_id, results)
                
                # Acknowledge the message
                ch.basic_ack(delivery_tag=method.delivery_tag)
                print(f"Processed job {job_id}")
            else:
                print("Invalid message format")
                ch.basic_nack(delivery_tag=method.delivery_tag, requeue=False)
                
        except Exception as e:
            print(f"Error processing job: {str(e)}")
            ch.basic_nack(delivery_tag=method.delivery_tag, requeue=False)
    
    def update_job_results(self, job_id, results):
        """
        Update job results in the database
        """
        # In a real implementation, we would connect to the database
        # and update the analysis_jobs table with the results
        
        print(f"Updating job {job_id} with results: {results}")
    
    def start(self):
        """
        Start listening for messages
        """
        self.channel.basic_qos(prefetch_count=1)
        self.channel.basic_consume(queue='analysis_jobs', on_message_callback=self.callback)
        print("AI Worker is waiting for messages. To exit press CTRL+C")
        self.channel.start_consuming()
    
    def stop(self):
        """
        Stop the worker
        """
        self.connection.close()

if __name__ == "__main__":
    worker = AIWorker()
    try:
        worker.start()
    except KeyboardInterrupt:
        print("Stopping AI Worker...")
        worker.stop()