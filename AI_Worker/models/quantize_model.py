# quantize_model.py
from onnxruntime.quantization import quantize_dynamic, QuantType

# --- Configuration ---
input_model_path = 'model_fp32_v1.onnx'      # 👈 Your ONNX model from the last step
output_model_path = 'model_quantized_v1.onnx'  # 👈 The final, optimized model file to create

print(f"Loading model: {input_model_path}")

# This function performs the optimization
quantize_dynamic(
    model_input=input_model_path,
    model_output=output_model_path,
    weight_type=QuantType.QInt8  # Convert model's weights to efficient 8-bit integers
)

print(f"✅ Quantization complete. Optimized model saved to: {output_model_path}")