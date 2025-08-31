import React, { useState, useEffect } from "react";
import { CustomInput } from "./CustomInput";
import { Button } from "../ui/Button";
import {
  adminService,
  CreateUserRequest,
  AdminClinicResponse,
} from "../../services/adminService";
import { toast } from "react-hot-toast";

interface UserCreateFormProps {
  onSuccess: () => void;
  onCancel: () => void;
}

export const UserCreateForm: React.FC<UserCreateFormProps> = ({
  onSuccess,
  onCancel,
}) => {
  const [formData, setFormData] = useState<CreateUserRequest>({
    firstName: "",
    lastName: "",
    email: "",
    role: "Doctor",
    clinicId: undefined,
  });

  const [clinics, setClinics] = useState<AdminClinicResponse[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isLoadingClinics, setIsLoadingClinics] = useState(true);
  const [errors, setErrors] = useState<Partial<CreateUserRequest>>({});

  useEffect(() => {
    loadClinics();
  }, []);

  const loadClinics = async () => {
    try {
      setIsLoadingClinics(true);
      const clinicsData = await adminService.getAllClinics();
      setClinics(clinicsData);
    } catch (error) {
      toast.error("Failed to load clinics");
      console.error("Error loading clinics:", error);
    } finally {
      setIsLoadingClinics(false);
    }
  };

  const validateForm = (): boolean => {
    const newErrors: Partial<CreateUserRequest> = {};

    if (!formData.firstName.trim()) {
      newErrors.firstName = "First name is required";
    }

    if (!formData.lastName.trim()) {
      newErrors.lastName = "Last name is required";
    }

    if (!formData.email.trim()) {
      newErrors.email = "Email is required";
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = "Please enter a valid email address";
    }

    if (!formData.role) {
      newErrors.role = "Role is required";
    }

    if (
      (formData.role === "ClinicAdmin" || formData.role === "Doctor") &&
      !formData.clinicId
    ) {
      newErrors.clinicId = "Clinic is required for this role";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleInputChange = (field: keyof CreateUserRequest, value: string) => {
    setFormData((prev) => ({
      ...prev,
      [field]: value,
    }));

    // Clear error when user starts typing
    if (errors[field]) {
      setErrors((prev) => ({
        ...prev,
        [field]: undefined,
      }));
    }
  };

  const handleRoleChange = (role: CreateUserRequest["role"]) => {
    setFormData((prev) => ({
      ...prev,
      role,
      // Clear clinicId if SuperAdmin is selected
      clinicId: role === "SuperAdmin" ? undefined : prev.clinicId,
    }));

    if (errors.role) {
      setErrors((prev) => ({
        ...prev,
        role: undefined,
      }));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setIsLoading(true);
    try {
      const userData = {
        ...formData,
        clinicId:
          formData.role === "SuperAdmin" ? undefined : formData.clinicId,
      };

      await adminService.createUser(userData);
      toast.success("User created successfully");
      onSuccess();
    } catch (error: any) {
      const errorMessage =
        error.response?.data?.message || "Failed to create user";
      toast.error(errorMessage);
      console.error("Error creating user:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const requiresClinic =
    formData.role === "ClinicAdmin" || formData.role === "Doctor";

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <CustomInput
          id="firstName"
          label="First Name"
          type="text"
          value={formData.firstName}
          onChange={(e) => handleInputChange("firstName", e.target.value)}
          error={errors.firstName}
          required
        />

        <CustomInput
          id="lastName"
          label="Last Name"
          type="text"
          value={formData.lastName}
          onChange={(e) => handleInputChange("lastName", e.target.value)}
          error={errors.lastName}
          required
        />
      </div>

      <CustomInput
        id="email"
        label="Email Address"
        type="email"
        value={formData.email}
        onChange={(e) => handleInputChange("email", e.target.value)}
        error={errors.email}
        required
      />

      <div>
        <label
          htmlFor="role"
          className="block text-sm font-medium text-gray-700 mb-1"
        >
          Role
        </label>
        <select
          id="role"
          value={formData.role}
          onChange={(e) =>
            handleRoleChange(e.target.value as CreateUserRequest["role"])
          }
          className={`block w-full border rounded-md shadow-sm py-2 px-3 focus:outline-none sm:text-sm ${
            errors.role
              ? "border-red-300 focus:ring-red-500 focus:border-red-500"
              : "border-gray-300 focus:ring-indigo-500 focus:border-indigo-500"
          }`}
          required
        >
          <option value="Doctor">Doctor</option>
          <option value="ClinicAdmin">Clinic Admin</option>
          <option value="SuperAdmin">Super Admin</option>
        </select>
        {errors.role && (
          <p className="mt-1 text-sm text-red-600">{errors.role}</p>
        )}
      </div>

      {requiresClinic && (
        <div>
          <label
            htmlFor="clinicId"
            className="block text-sm font-medium text-gray-700 mb-1"
          >
            Clinic
          </label>
          {isLoadingClinics ? (
            <div className="py-2 px-3 border border-gray-300 rounded-md bg-gray-50">
              Loading clinics...
            </div>
          ) : (
            <select
              id="clinicId"
              value={formData.clinicId || ""}
              onChange={(e) => handleInputChange("clinicId", e.target.value)}
              className={`block w-full border rounded-md shadow-sm py-2 px-3 focus:outline-none sm:text-sm ${
                errors.clinicId
                  ? "border-red-300 focus:ring-red-500 focus:border-red-500"
                  : "border-gray-300 focus:ring-indigo-500 focus:border-indigo-500"
              }`}
              required
            >
              <option value="">Select a clinic</option>
              {clinics.map((clinic) => (
                <option key={clinic.id} value={clinic.id}>
                  {clinic.name}
                </option>
              ))}
            </select>
          )}
          {errors.clinicId && (
            <p className="mt-1 text-sm text-red-600">{errors.clinicId}</p>
          )}
        </div>
      )}

      <div className="flex justify-end space-x-3 pt-4">
        <Button
          type="button"
          variant="outline"
          onClick={onCancel}
          disabled={isLoading}
        >
          Cancel
        </Button>
        <Button type="submit" loading={isLoading} disabled={isLoading}>
          Create User
        </Button>
      </div>
    </form>
  );
};

