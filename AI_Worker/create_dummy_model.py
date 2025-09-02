import numpy as np
import cv2
import os

def create_dummy_model():
    """
    Create a simple dummy model for testing purposes
    """
    try:
        import onnx
        from onnx import helper, TensorProto, numpy_helper
        
        # Create a simple model that just passes through the input
        # Input: [1, 3, 640, 640] -> Output: [1, 5, 100] (dummy detections)
        
        # Create input and output info
        X = helper.make_tensor_value_info('input', TensorProto.FLOAT, [1, 3, 640, 640])
        Y = helper.make_tensor_value_info('output', TensorProto.FLOAT, [1, 5, 100])
        
        # Create a simple node that just reshapes the input
        node_def = helper.make_node(
            'Reshape',
            inputs=['input', 'shape'],
            outputs=['output']
        )
        
        # Create the shape tensor
        shape_tensor = helper.make_tensor(
            name='shape',
            data_type=TensorProto.INT64,
            dims=[3],
            vals=[1, 5, 100]
        )
        
        # Create the graph
        graph_def = helper.make_graph(
            [node_def],
            'dummy-model',
            [X],
            [Y],
            [shape_tensor]
        )
        
        # Create the model
        model_def = helper.make_model(graph_def, producer_name='dummy-model')
        
        # Save the model
        onnx.save(model_def, 'models/dummy_model.onnx')
        print("Dummy model created successfully!")
        return True
        
    except Exception as e:
        print(f"Error creating dummy model: {e}")
        return False

def test_dummy_model():
    """
    Test the dummy model
    """
    try:
        import onnxruntime as ort
        
        # Check if dummy model exists
        model_path = "models/dummy_model.onnx"
        if not os.path.exists(model_path):
            print(f"Dummy model not found at: {model_path}")
            return False
        
        print(f"Loading dummy model from: {model_path}")
        
        # Load the model
        model = ort.InferenceSession(model_path)
        print("Dummy model loaded successfully!")
        
        # Create dummy input
        dummy_input = np.random.rand(1, 3, 640, 640).astype(np.float32)
        
        # Run inference
        input_name = model.get_inputs()[0].name
        outputs = model.run(None, {input_name: dummy_input})
        
        print(f"Dummy inference successful!")
        print(f"Output shape: {outputs[0].shape}")
        
        return True
        
    except Exception as e:
        print(f"Error testing dummy model: {e}")
        return False

if __name__ == "__main__":
    # Try to create dummy model
    if create_dummy_model():
        # Test dummy model
        test_dummy_model()
    else:
        print("Failed to create dummy model")