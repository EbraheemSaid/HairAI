import onnxruntime as ort
import numpy as np
import cv2
import os

def test_model_outputs():
    # Check if model files exist
    model_v2_path = "models/model_fp32_v2.onnx"
    
    if not os.path.exists(model_v2_path):
        print(f"Model not found at: {model_v2_path}")
        return
    
    print(f"Loading model from: {model_v2_path}")
    
    try:
        # Load the model
        model = ort.InferenceSession(model_v2_path, providers=['CPUExecutionProvider'])
        print("Model loaded successfully!")
        
        # Print model info
        print("\nModel Inputs:")
        for i, input in enumerate(model.get_inputs()):
            print(f"  Input {i}:")
            print(f"    Name: {input.name}")
            print(f"    Shape: {input.shape}")
            print(f"    Type: {input.type}")
        
        print("\nModel Outputs:")
        for i, output in enumerate(model.get_outputs()):
            print(f"  Output {i}:")
            print(f"    Name: {output.name}")
            print(f"    Shape: {output.shape}")
            print(f"    Type: {output.type}")
        
        # Create a test input tensor
        # Assume the model expects a 4D tensor with shape [batch, channels, height, width]
        # and values normalized to [0, 1]
        input_shape = model.get_inputs()[0].shape
        print(f"\nExpected input shape: {input_shape}")
        
        # Create a dummy input (all zeros)
        dummy_input = np.zeros(input_shape, dtype=np.float32)
        
        # Run inference
        input_name = model.get_inputs()[0].name
        outputs = model.run(None, {input_name: dummy_input})
        
        print(f"\nInference successful!")
        print(f"Number of outputs: {len(outputs)}")
        
        for i, output in enumerate(outputs):
            print(f"  Output {i} shape: {output.shape}")
            print(f"  Output {i} dtype: {output.dtype}")
            print(f"  Output {i} min/max: {output.min():.4f}/{output.max():.4f}")
            
    except Exception as e:
        print(f"Error testing model: {e}")
        import traceback
        traceback.print_exc()

if __name__ == "__main__":
    test_model_outputs()