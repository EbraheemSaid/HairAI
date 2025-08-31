import React, { useState, useRef } from "react";
import { toast } from "react-hot-toast";

interface FileUploadProps {
  label?: string;
  onFileSelect?: (file: File | null) => void;
  onFileRemove?: () => void;
  error?: string;
  helperText?: string;
  accept?: string;
  maxSize?: number; // in bytes
  className?: string;
  allowedTypes?: string[]; // More specific type validation
  showImagePreview?: boolean;
}

export const FileUpload: React.FC<FileUploadProps> = ({
  label,
  onFileSelect,
  onFileRemove,
  error,
  helperText,
  accept = "image/*",
  maxSize = 10 * 1024 * 1024, // 10MB default
  className = "",
  allowedTypes = ["image/jpeg", "image/jpg", "image/png", "image/gif"],
  showImagePreview = true,
}) => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [dragActive, setDragActive] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFiles = (files: FileList) => {
    if (files.length === 0) return;

    const file = files[0];

    // Check file size
    if (file.size > maxSize) {
      toast.error(
        `File size exceeds ${(maxSize / (1024 * 1024)).toFixed(1)}MB limit`
      );
      return;
    }

    // More specific file type validation
    if (allowedTypes.length > 0 && !allowedTypes.includes(file.type)) {
      toast.error(
        `Invalid file type. Please upload: ${allowedTypes
          .map((type) => type.split("/")[1].toUpperCase())
          .join(", ")}`
      );
      return;
    }

    // Additional image validation for analysis
    if (file.type.startsWith("image/")) {
      const img = new Image();
      img.onload = () => {
        // Check minimum dimensions for analysis quality
        if (img.width < 300 || img.height < 300) {
          toast.error(
            "Image must be at least 300x300 pixels for accurate analysis"
          );
          return;
        }

        // Check aspect ratio (should be reasonably square for head shots)
        const aspectRatio = img.width / img.height;
        if (aspectRatio < 0.5 || aspectRatio > 2) {
          toast.warning(
            "For best results, use images with a square aspect ratio"
          );
        }
      };
      img.src = URL.createObjectURL(file);
    }

    setSelectedFile(file);
    onFileSelect?.(file);

    // Create preview for images
    if (showImagePreview && file.type.startsWith("image/")) {
      const reader = new FileReader();
      reader.onload = () => {
        setPreviewUrl(reader.result as string);
      };
      reader.readAsDataURL(file);
    } else {
      setPreviewUrl(null);
    }
  };

  const handleDrag = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (e.type === "dragenter" || e.type === "dragover") {
      setDragActive(true);
    } else if (e.type === "dragleave") {
      setDragActive(false);
    }
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(false);

    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      handleFiles(e.dataTransfer.files);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      handleFiles(e.target.files);
    }
  };

  const removeFile = () => {
    setSelectedFile(null);
    setPreviewUrl(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
    onFileSelect?.(null);
    onFileRemove?.();
  };

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return "0 Bytes";
    const k = 1024;
    const sizes = ["Bytes", "KB", "MB", "GB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + " " + sizes[i];
  };

  return (
    <div className={`w-full ${className}`}>
      {label && (
        <label className="block text-sm font-medium text-gray-700 mb-1">
          {label}
        </label>
      )}

      <div
        className={`relative border-2 border-dashed rounded-lg p-6 text-center cursor-pointer transition-colors ${
          dragActive
            ? "border-indigo-500 bg-indigo-50"
            : error
            ? "border-red-300 bg-red-50"
            : "border-gray-300 hover:border-gray-400"
        }`}
        onDragEnter={handleDrag}
        onDragOver={handleDrag}
        onDragLeave={handleDrag}
        onDrop={handleDrop}
        onClick={() => fileInputRef.current?.click()}
      >
        <input
          ref={fileInputRef}
          type="file"
          className="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
          onChange={handleChange}
          accept={accept}
        />

        {selectedFile ? (
          <div className="flex flex-col items-center">
            {previewUrl && showImagePreview && (
              <div className="relative mb-4">
                <img
                  src={previewUrl}
                  alt="Preview"
                  className="h-32 w-32 object-contain rounded-lg border"
                />
                <div className="absolute top-0 right-0 bg-green-500 text-white rounded-full w-6 h-6 flex items-center justify-center text-xs">
                  ✓
                </div>
              </div>
            )}
            {!previewUrl &&
              selectedFile.type.startsWith("image/") &&
              !showImagePreview && (
                <div className="mb-4">
                  <svg
                    className="h-12 w-12 text-green-500"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"
                    />
                  </svg>
                </div>
              )}
            <div className="text-sm text-gray-600 text-center">
              <p className="font-medium text-green-700">{selectedFile.name}</p>
              <p className="text-gray-500">
                {formatFileSize(selectedFile.size)}
              </p>
              {selectedFile.type.startsWith("image/") && (
                <p className="text-xs text-gray-400 mt-1">Ready for analysis</p>
              )}
            </div>
            <button
              type="button"
              className="mt-3 inline-flex items-center px-3 py-1 border border-red-300 text-xs font-medium rounded text-red-700 bg-red-50 hover:bg-red-100 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
              onClick={(e) => {
                e.stopPropagation();
                removeFile();
              }}
            >
              <svg
                className="h-3 w-3 mr-1"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
              Remove
            </button>
          </div>
        ) : (
          <div className="space-y-2">
            <svg
              className="mx-auto h-12 w-12 text-gray-400"
              stroke="currentColor"
              fill="none"
              viewBox="0 0 48 48"
              aria-hidden="true"
            >
              <path
                d="M28 8H12a4 4 0 00-4 4v20m32-12v8m0 0v8a4 4 0 01-4 4H12a4 4 0 01-4-4v-4m32-4l-3.172-3.172a4 4 0 00-5.656 0L28 28M8 32l9.172-9.172a4 4 0 015.656 0L28 28m0 0l4 4m4-24h8m-4-4v8m-12 4h.02"
                strokeWidth={2}
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
            <div className="flex text-sm text-gray-600">
              <span className="font-medium text-indigo-600 hover:text-indigo-500">
                Upload a file
              </span>
              <p className="pl-1">or drag and drop</p>
            </div>
            <p className="text-xs text-gray-500">
              {accept.includes("image") ? "PNG, JPG, GIF" : "Any file type"} up
              to {maxSize / (1024 * 1024)}MB
            </p>
          </div>
        )}
      </div>

      {error && <p className="mt-1 text-sm text-red-600">{error}</p>}
      {helperText && !error && (
        <p className="mt-1 text-sm text-gray-500">{helperText}</p>
      )}
    </div>
  );
};
