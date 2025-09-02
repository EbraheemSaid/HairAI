import os
import pika
import json
import psycopg2
import logging
import time
from PIL import Image
import numpy as np
import onnxruntime as ort
import cv2
from sklearn.cluster import DBSCAN

# Set up logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

class AIWorker:
    def __init__(self):
        # Get configuration from environment variables
        self.rabbitmq_connection_string = os.getenv('RABBITMQ_CONNECTION_STRING')
        self.database_connection_string = os.getenv('DATABASE_CONNECTION_STRING')
        
        # Configuration for hair analysis
        self.model_path = 'models/model_fp32_v2.onnx'  # Using non-quantized model
        self.image_area_cm2 = 0.25  # Default area, will be updated from job details
        self.distance_threshold = 100
        self.pixels_per_mm = 600
        
        # Initialize ONNX model
        logger.info(f"Loading ONNX model from {self.model_path}")
        self.model = ort.InferenceSession(self.model_path, providers=['CPUExecutionProvider'])
        logger.info("Model loaded successfully")
        
        # Check if environment variables are set
        if not self.rabbitmq_connection_string:
            logger.warning("RABBITMQ_CONNECTION_STRING environment variable not set")
            return
            
        if not self.database_connection_string:
            logger.warning("DATABASE_CONNECTION_STRING environment variable not set")
            return
        
        # Connect to RabbitMQ with retry logic
        logger.info("Connecting to RabbitMQ")
        self.connection = None
        self.channel = None
        max_retries = 5
        retry_delay = 5
        
        for attempt in range(max_retries):
            try:
                self.connection = pika.BlockingConnection(
                    pika.URLParameters(self.rabbitmq_connection_string)
                )
                self.channel = self.connection.channel()
                self.channel.queue_declare(queue='analysis_jobs', durable=True)
                logger.info("Connected to RabbitMQ")
                break
            except Exception as e:
                logger.warning(f"Failed to connect to RabbitMQ (attempt {attempt + 1}/{max_retries}): {e}")
                if attempt < max_retries - 1:
                    logger.info(f"Retrying in {retry_delay} seconds...")
                    time.sleep(retry_delay)
                else:
                    logger.error("Failed to connect to RabbitMQ after all retries")
                    raise
        
        # Connect to database with retry logic
        logger.info("Connecting to database")
        max_retries = 5
        retry_delay = 5
        
        for attempt in range(max_retries):
            try:
                self.db_connection = psycopg2.connect(self.database_connection_string)
                logger.info("Connected to database")
                break
            except Exception as e:
                logger.warning(f"Failed to connect to database (attempt {attempt + 1}/{max_retries}): {e}")
                if attempt < max_retries - 1:
                    logger.info(f"Retrying in {retry_delay} seconds...")
                    time.sleep(retry_delay)
                else:
                    logger.error("Failed to connect to database after all retries")
                    raise

    def measure_thickness_from_bbox(self, coords):
        """
        Calculates thickness based on the minor axis of the bounding box.
        """
        x1, y1, x2, y2 = coords
        width = x2 - x1
        height = y2 - y1
        # The thickness of a long, thin object is its smaller dimension
        return min(width, height)

    def draw_final_label(self, image, label, coords):
        """
        Draws the final interpreted bounding box and label.
        """
        x1, y1, x2, y2 = coords
        
        color_map = {
            'single': (255, 255, 0),      # Yellow
            'double': (0, 255, 0),       # Green
            'triple+': (0, 165, 255),     # Orange for all 3+ FUs
            'abnormal': (255, 0, 0),      # Red
            'undersize': (0, 0, 255),     # Blue
        }
        color_key = label.split(' ')[0].lower()
        color = color_map.get(color_key, (128, 128, 128)) # Default to gray
        cv2.rectangle(image, (x1, y1), (x2, y2), color, 2)
        font = cv2.FONT_HERSHEY_SIMPLEX
        font_scale = 0.7
        font_thickness = 2
        (text_w, text_h), baseline = cv2.getTextSize(label, font, font_scale, font_thickness)
        cv2.rectangle(image, (x1, y1 - text_h - baseline), (x1 + text_w, y1), color, -1)
        cv2.putText(image, label, (x1, y1 - 4), font, font_scale, (0, 0, 0), font_thickness)

    def process_job(self, job_id):
        """
        Process a single analysis job
        """
        logger.info(f"Processing job {job_id}")
        
        try:
            # 1. Retrieve job details from the database
            job_details = self.get_job_details(job_id)
            if not job_details:
                raise Exception(f"Job {job_id} not found")
            
            # 2. Load the image
            # Assuming the ImageStorageKey is a file path
            image_path = job_details['image_path']
            # If it's a relative path, it should be correct as is
            # The backend already saves the full relative path
            if not os.path.isabs(image_path):
                # No need to prepend anything, the path should be correct
                pass
            
            logger.info(f"Loading image from {image_path}")
            original_image = cv2.imread(image_path)
            if original_image is None:
                raise Exception(f"Could not read image at {image_path}")
            
            analysis_image = original_image.copy()
            logger.info(f"Image loaded successfully. Shape: {original_image.shape}")
            
            # 3. Run the AI model on the image
            logger.info("Running model inference")
            detections = self.run_inference(original_image)
            logger.info(f"Found {len(detections)} detections")
            
            # 4. Process detections and calculate metrics
            logger.info("Analyzing detections")
            results = self.analyze_detections(detections, analysis_image, original_image)
            logger.info("Analysis completed")
            
            # 5. Save the annotated image
            annotated_image_path = self.save_annotated_image(analysis_image, job_id)
            logger.info(f"Annotated image saved to {annotated_image_path}")
            
            # 6. Prepare results for database update
            final_results = {
                "metrics": results["metrics"],
                "follicular_breakdown": results["follicular_breakdown"],
                "other_detections": results["other_detections"],
                "annotated_image_path": annotated_image_path
            }
            
            logger.info(f"Job {job_id} processed successfully")
            return final_results
            
        except Exception as e:
            logger.error(f"Error processing job {job_id}: {str(e)}")
            raise

    def get_job_details(self, job_id):
        """
        Retrieve job details from the database
        """
        try:
            cursor = self.db_connection.cursor()
            cursor.execute("""
                SELECT aj."ImageStorageKey", aj."CalibrationProfileId", cp."CalibrationData"
                FROM "AnalysisJobs" aj
                LEFT JOIN "CalibrationProfiles" cp ON aj."CalibrationProfileId" = cp."Id"
                WHERE aj."Id" = %s
            """, (str(job_id),))
            
            row = cursor.fetchone()
            if row:
                return {
                    "image_path": row[0],  # ImageStorageKey
                    "calibration_profile_id": row[1],
                    "calibration_data": row[2]
                }
            return None
        except Exception as e:
            logger.error(f"Error retrieving job details: {str(e)}")
            return None

    def update_job_results(self, job_id, results):
        """
        Update job results in the database
        """
        try:
            cursor = self.db_connection.cursor()
            
            # Convert results to JSON string
            results_json = json.dumps(results)
            
            # Update the AnalysisJobs table
            cursor.execute("""
                UPDATE "AnalysisJobs" 
                SET "Status" = %s, 
                    "AnalysisResult" = %s,
                    "AnnotatedImageKey" = %s,
                    "CompletedAt" = NOW(),
                    "ProcessingTimeMs" = EXTRACT(EPOCH FROM (NOW() - "CreatedAt")) * 1000
                WHERE "Id" = %s
            """, (2, results_json, results.get("annotated_image_path", ""), str(job_id)))  # 2 = Completed
            
            self.db_connection.commit()
            logger.info(f"Successfully updated job {job_id} with results")
        except Exception as e:
            self.db_connection.rollback()
            logger.error(f"Error updating job results: {str(e)}")

    def run_inference(self, image):
        """
        Run ONNX model inference on the image
        """
        # Preprocess the image for YOLOv8
        input_shape = (640, 640)  # Standard YOLOv8 input size
        original_shape = image.shape[:2]
        
        # Resize image while maintaining aspect ratio
        img_resized = self.resize_and_pad(image, input_shape)
        
        # Convert BGR to RGB
        img_rgb = cv2.cvtColor(img_resized, cv2.COLOR_BGR2RGB)
        
        # Normalize pixel values to [0, 1]
        img_normalized = img_rgb.astype(np.float32) / 255.0
        
        # Change to CHW format (Channels, Height, Width)
        img_transposed = np.transpose(img_normalized, (2, 0, 1))
        
        # Add batch dimension
        img_batch = np.expand_dims(img_transposed, axis=0)
        
        # Run inference
        ort_inputs = {self.model.get_inputs()[0].name: img_batch}
        ort_outs = self.model.run(None, ort_inputs)
        
        # Post-process detections
        detections = self.post_process_detections(ort_outs, original_shape, input_shape)
        
        return detections

    def resize_and_pad(self, image, target_shape):
        """
        Resize image while maintaining aspect ratio and pad to target shape
        """
        h, w = image.shape[:2]
        target_h, target_w = target_shape
        
        # Calculate scaling factor
        scale = min(target_w / w, target_h / h)
        
        # Calculate new dimensions
        new_w = int(w * scale)
        new_h = int(h * scale)
        
        # Resize image
        resized = cv2.resize(image, (new_w, new_h), interpolation=cv2.INTER_LINEAR)
        
        # Create padded image
        padded = np.full((target_h, target_w, 3), 114, dtype=np.uint8)
        padded[:new_h, :new_w] = resized
        
        return padded

    def post_process_detections(self, outputs, original_shape, input_shape):
        """
        Post-process model outputs to get detections in the format compatible with YOLOv8
        Based on the old script which used Ultralytics YOLO library
        """
        # YOLOv8 ONNX models typically output a tensor with shape [1, 4 + num_classes, num_boxes]
        # where 4 represents [x_center, y_center, width, height] and num_classes is the number of classes
        
        # Get the output tensor (assuming single output for simplicity)
        output_tensor = outputs[0]
        
        # If the output is 3D, we need to reshape it
        if len(output_tensor.shape) == 3:
            # Reshape from [1, 4 + num_classes, num_boxes] to [num_boxes, 4 + num_classes]
            output_tensor = output_tensor[0].transpose()
        
        # Define confidence threshold and NMS threshold
        conf_threshold = 0.15  # As used in the old script
        nms_threshold = 0.45   # Standard NMS threshold
        
        # Lists to store valid detections
        boxes = []
        scores = []
        class_ids = []
        
        # Process each detection
        for detection in output_tensor:
            # Extract class scores (assuming last 5 values are [x, y, w, h, conf, class_scores...])
            if len(detection) >= 6:
                # Extract bounding box coordinates (x_center, y_center, width, height)
                x_center, y_center, width, height = detection[:4]
                
                # Extract confidence
                confidence = detection[4]
                
                # Extract class scores
                class_scores = detection[5:]
                
                # Apply confidence threshold
                if confidence >= conf_threshold:
                    # Get the class with highest score
                    class_id = np.argmax(class_scores)
                    class_score = class_scores[class_id]
                    
                    # Calculate final confidence (objectness * class confidence)
                    final_conf = confidence * class_score
                    
                    # Apply confidence threshold again
                    if final_conf >= conf_threshold:
                        # Convert from [x_center, y_center, width, height] to [x1, y1, x2, y2]
                        x1 = (x_center - width / 2) * original_shape[1] / input_shape[1]
                        y1 = (y_center - height / 2) * original_shape[0] / input_shape[0]
                        x2 = (x_center + width / 2) * original_shape[1] / input_shape[1]
                        y2 = (y_center + height / 2) * original_shape[0] / input_shape[0]
                        
                        # Add to lists
                        boxes.append([x1, y1, x2, y2])
                        scores.append(final_conf)
                        class_ids.append(class_id)
        
        # Apply Non-Maximum Suppression (NMS)
        if len(boxes) > 0:
            # Convert to numpy arrays
            boxes = np.array(boxes)
            scores = np.array(scores)
            
            # Apply NMS
            indices = cv2.dnn.NMSBoxes(boxes.tolist(), scores.tolist(), conf_threshold, nms_threshold)
            
            # Filter detections based on NMS
            if len(indices) > 0:
                # Flatten indices if needed (cv2.dnn.NMSBoxes can return different shapes)
                if isinstance(indices, np.ndarray) and len(indices.shape) > 1:
                    indices = indices.flatten()
                elif isinstance(indices, tuple):
                    indices = indices[0]
                
                # Keep only the detections after NMS
                filtered_boxes = boxes[indices]
                filtered_scores = scores[indices]
                filtered_class_ids = [class_ids[i] for i in indices]
                
                # Convert to the format expected by the rest of the code
                detections = []
                for i in range(len(filtered_boxes)):
                    detection_dict = {
                        'bbox': filtered_boxes[i].tolist(),  # [x1, y1, x2, y2]
                        'confidence': float(filtered_scores[i]),
                        'class_id': int(filtered_class_ids[i])
                    }
                    detections.append(detection_dict)
                
                return detections
        
        # Return empty list if no valid detections
        return []

    def analyze_detections(self, detections, analysis_image, original_image):
        """
        Analyze detections and calculate all required metrics
        """
        # Define class names based on the old script
        class_names = {
            0: 'single',
            1: 'double',
            2: 'triple+',
            3: 'abnormal',
            4: 'undersize'
        }
        
        # Detections that could be part of a healthy follicular unit
        clusterable_detections = []
        # Detections that are explicitly not healthy (undersize, abnormal)
        non_terminal_detections = []
        
        # Process detections
        for detection in detections:
            class_id = detection['class_id']
            class_name = class_names.get(class_id, 'unknown')
            bbox = detection['bbox']
            
            # Calculate center point
            x1, y1, x2, y2 = map(int, bbox)
            center = ((x1 + x2) // 2, (y1 + y2) // 2)
            
            detection_info = {
                'class_name': class_name,
                'coords': [x1, y1, x2, y2],
                'center': center
            }
            
            # Separate detections
            if class_name in ['single', 'double']:  # 'premium' was in the old script, assuming 'double' is similar
                clusterable_detections.append(detection_info)
            else:
                non_terminal_detections.append(detection_info)
                # Draw non-terminal detections
                self.draw_final_label(analysis_image, class_name.capitalize(), [x1, y1, x2, y2])
        
        # Cluster clusterable detections using DBSCAN
        clusters = []
        if clusterable_detections:
            centers = np.array([d['center'] for d in clusterable_detections])
            db = DBSCAN(eps=self.distance_threshold, min_samples=1).fit(centers)
            cluster_labels = db.labels_
            n_clusters = len(set(cluster_labels))
            
            for i in range(n_clusters):
                indices = np.where(cluster_labels == i)[0]
                current_cluster = [clusterable_detections[idx] for idx in indices]
                clusters.append(current_cluster)
        
        # Initialize counts
        counts = {
            'single': 0, 'double': 0, 'triple+': 0,
            'abnormal': 0, 'undersize': 0
        }
        
        all_terminal_thicknesses_px = []
        total_hairs_in_sample = 0
        
        # Process non-terminal detections (undersize, abnormal)
        for det in non_terminal_detections:
            if det['class_name'] in counts:
                counts[det['class_name']] += 1
        
        # Process clusters and measure thickness of individual hairs
        for cluster in clusters:
            for det in cluster:
                # Measure each individual hair in the cluster
                thickness_px = self.measure_thickness_from_bbox(det['coords'])
                all_terminal_thicknesses_px.append(thickness_px)
            
            total_hairs_in_sample += len(cluster)
            
            # Create a single large bounding box for drawing purposes only
            cluster_coords = (
                min(det['coords'][0] for det in cluster),
                min(det['coords'][1] for det in cluster),
                max(det['coords'][2] for det in cluster),
                max(det['coords'][3] for det in cluster)
            )
            
            num_hairs = len(cluster)
            if num_hairs == 1:
                counts['single'] += 1
                self.draw_final_label(analysis_image, 'Single FU', cluster_coords)
            elif num_hairs == 2:
                counts['double'] += 1
                self.draw_final_label(analysis_image, 'Double FU', cluster_coords)
            elif num_hairs >= 3:
                counts['triple+'] += 1
                self.draw_final_label(analysis_image, 'Triple+ FU', cluster_coords)
        
        # Calculate metrics
        if all_terminal_thicknesses_px:
            avg_thickness_px = np.mean(all_terminal_thicknesses_px)
            avg_thickness_mm = avg_thickness_px / self.pixels_per_mm
            avg_hair_thickness_microns = avg_thickness_mm * 1000
        else:
            avg_hair_thickness_microns = 0
        
        total_fus = counts['single'] + counts['double'] + counts['triple+']
        fu_density = total_fus / self.image_area_cm2 if self.image_area_cm2 > 0 else 0
        avg_hairs_per_fu = total_hairs_in_sample / total_fus if total_fus > 0 else 0
        vellus_density = counts['undersize'] / self.image_area_cm2 if self.image_area_cm2 > 0 else 0
        abnormal_density = counts['abnormal'] / self.image_area_cm2 if self.image_area_cm2 > 0 else 0
        
        # Prepare metrics
        metrics = {
            "follicular_unit_density": round(fu_density, 2),
            "average_hairs_per_fu": round(avg_hairs_per_fu, 2),
            "average_hair_thickness_microns": round(avg_hair_thickness_microns, 2)
        }
        
        # Prepare follicular breakdown
        follicular_breakdown = {
            "single_fu_count": counts['single'],
            "double_fu_count": counts['double'],
            "triple_plus_fu_count": counts['triple+'],
            "single_fu_percentage": round(counts['single'] / total_fus * 100, 1) if total_fus > 0 else 0.0,
            "double_fu_percentage": round(counts['double'] / total_fus * 100, 1) if total_fus > 0 else 0.0,
            "triple_plus_fu_percentage": round(counts['triple+'] / total_fus * 100, 1) if total_fus > 0 else 0.0
        }
        
        # Prepare other detections
        other_detections = {
            "vellus_count": counts['undersize'],
            "abnormal_count": counts['abnormal']
        }
        
        return {
            "metrics": metrics,
            "follicular_breakdown": follicular_breakdown,
            "other_detections": other_detections
        }

    def save_annotated_image(self, analysis_image, job_id):
        """
        Save the annotated image and return its path
        """
        # Create annotated images directory if it doesn't exist
        annotated_dir = os.path.join("output", "annotated_images")
        if not os.path.exists(annotated_dir):
            os.makedirs(annotated_dir)
        
        # Save annotated image
        annotated_path = os.path.join(annotated_dir, f"annotated_{job_id}.jpg")
        cv2.imwrite(annotated_path, analysis_image)
        
        # Return relative path for storage in database
        return os.path.join("output", "annotated_images", f"annotated_{job_id}.jpg")

    def callback(self, ch, method, properties, body):
        """
        Callback function for RabbitMQ messages
        """
        try:
            message = json.loads(body)
            job_id = message.get('JobId')
            
            if job_id:
                logger.info(f"Received job {job_id} from queue")
                # Update job status to Processing
                self.update_job_status(job_id, 'Processing')
                
                # Process the job
                results = self.process_job(job_id)
                
                # Update database with results
                self.update_job_results(job_id, results)
                
                # Acknowledge the message
                ch.basic_ack(delivery_tag=method.delivery_tag)
                logger.info(f"Processed job {job_id}")
            else:
                logger.warning("Invalid message format")
                ch.basic_nack(delivery_tag=method.delivery_tag, requeue=False)
                
        except Exception as e:
            logger.error(f"Error processing job: {str(e)}")
            # Update job status to Failed
            if 'job_id' in locals():
                self.update_job_status(job_id, 'Failed', str(e))
            ch.basic_nack(delivery_tag=method.delivery_tag, requeue=False)

    def update_job_status(self, job_id, status, error_message=None):
        """
        Update job status in the database
        """
        try:
            cursor = self.db_connection.cursor()
            
            # Convert status string to integer
            status_map = {
                'Pending': 0,
                'Processing': 1,
                'Completed': 2,
                'Failed': 3
            }
            status_int = status_map.get(status, 0)  # Default to Pending if not found
            
            if status == 'Processing':
                cursor.execute("""
                    UPDATE "AnalysisJobs" 
                    SET "Status" = %s, 
                        "StartedAt" = NOW()
                    WHERE "Id" = %s
                """, (status_int, str(job_id)))
            elif status == 'Failed':
                cursor.execute("""
                    UPDATE "AnalysisJobs" 
                    SET "Status" = %s, 
                        "ErrorMessage" = %s,
                        "CompletedAt" = NOW()
                    WHERE "Id" = %s
                """, (status_int, error_message, str(job_id)))
            elif status == 'Completed':
                cursor.execute("""
                    UPDATE "AnalysisJobs" 
                    SET "Status" = %s, 
                        "CompletedAt" = NOW()
                    WHERE "Id" = %s
                """, (status_int, str(job_id)))
            
            self.db_connection.commit()
            logger.info(f"Successfully updated job {job_id} status to {status} ({status_int})")
        except Exception as e:
            self.db_connection.rollback()
            logger.error(f"Error updating job status: {str(e)}")

    def start(self):
        """
        Start listening for messages
        """
        # Check if connections were established
        if not hasattr(self, 'connection') or not hasattr(self, 'db_connection'):
            logger.error("Worker not properly initialized. Missing environment variables.")
            return
            
        self.channel.basic_qos(prefetch_count=1)
        self.channel.basic_consume(queue='analysis_jobs', on_message_callback=self.callback)
        logger.info("AI Worker is waiting for messages. To exit press CTRL+C")
        self.channel.start_consuming()
    
    def stop(self):
        """
        Stop the worker
        """
        self.connection.close()
        self.db_connection.close()
        logger.info("AI Worker stopped")

if __name__ == "__main__":
    worker = AIWorker()
    try:
        # Check if worker was properly initialized
        if hasattr(worker, 'connection') and hasattr(worker, 'db_connection'):
            worker.start()
        else:
            logger.info("Worker initialized in test mode (missing environment variables)")
    except KeyboardInterrupt:
        logger.info("Stopping AI Worker...")
        worker.stop()