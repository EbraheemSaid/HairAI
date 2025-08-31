import React, { useState, useEffect } from "react";
import { DataTable } from "../components/ui/DataTable";
import { Button } from "../components/ui/Button";
import { Modal } from "../components/ui/Modal";
import { UserCreateForm } from "../components/forms/UserCreateForm";
import { PlusIcon, PencilIcon, TrashIcon } from "@heroicons/react/24/outline";
import { adminService, AdminUserResponse } from "../services/adminService";
import { toast } from "react-hot-toast";

export const UsersPage: React.FC = () => {
  const [users, setUsers] = useState<AdminUserResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

  useEffect(() => {
    loadUsers();
  }, []);

  const loadUsers = async () => {
    try {
      setIsLoading(true);
      const usersData = await adminService.getAllUsers();
      setUsers(usersData);
    } catch (error) {
      toast.error("Failed to load users");
      console.error("Error loading users:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreateUser = () => {
    setIsCreateModalOpen(false);
    loadUsers(); // Refresh the users list
  };

  const handleDeleteUser = async (userId: string) => {
    if (!confirm("Are you sure you want to delete this user?")) {
      return;
    }

    try {
      await adminService.deleteUser(userId);
      toast.success("User deleted successfully");
      loadUsers();
    } catch (error) {
      toast.error("Failed to delete user");
      console.error("Error deleting user:", error);
    }
  };

  const handleToggleUserStatus = async (userId: string, isActive: boolean) => {
    try {
      if (isActive) {
        await adminService.deactivateUser(userId);
        toast.success("User deactivated successfully");
      } else {
        await adminService.activateUser(userId);
        toast.success("User activated successfully");
      }
      loadUsers();
    } catch (error) {
      toast.error(`Failed to ${isActive ? "deactivate" : "activate"} user`);
      console.error("Error toggling user status:", error);
    }
  };

  const columns = [
    {
      key: "firstName",
      title: "First Name",
      sortable: true,
    },
    {
      key: "lastName",
      title: "Last Name",
      sortable: true,
    },
    {
      key: "email",
      title: "Email",
    },
    {
      key: "role",
      title: "Role",
      render: (value: string) => (
        <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-indigo-100 text-indigo-800">
          {value}
        </span>
      ),
    },
    {
      key: "clinic",
      title: "Clinic",
      render: (value: any, row: AdminUserResponse) => row.clinic?.name || "N/A",
    },
    {
      key: "lastLoginAt",
      title: "Last Login",
      render: (value: string) => {
        if (!value) return "Never";
        return new Date(value).toLocaleDateString();
      },
    },
    {
      key: "isActive",
      title: "Status",
      render: (value: boolean) => (
        <span
          className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
            value ? "bg-green-100 text-green-800" : "bg-gray-100 text-gray-800"
          }`}
        >
          {value ? "Active" : "Inactive"}
        </span>
      ),
    },
    {
      key: "actions",
      title: "Actions",
      render: (_: any, row: AdminUserResponse) => (
        <div className="flex space-x-2">
          <button
            className={`${
              row.isActive
                ? "text-orange-600 hover:text-orange-900"
                : "text-green-600 hover:text-green-900"
            }`}
            onClick={() => handleToggleUserStatus(row.id, row.isActive)}
            title={row.isActive ? "Deactivate user" : "Activate user"}
          >
            {row.isActive ? "Deactivate" : "Activate"}
          </button>
          <button
            className="text-red-600 hover:text-red-900"
            onClick={() => handleDeleteUser(row.id)}
            title="Delete user"
          >
            <TrashIcon className="h-5 w-5" />
          </button>
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Users</h1>
          <p className="mt-1 text-sm text-gray-500">
            Manage all platform users
          </p>
        </div>
        <div className="mt-4 md:mt-0">
          <Button onClick={() => setIsCreateModalOpen(true)}>
            <PlusIcon className="-ml-1 mr-2 h-5 w-5" aria-hidden="true" />
            Add User
          </Button>
        </div>
      </div>

      <div className="bg-white shadow sm:rounded-lg">
        <div className="px-4 py-5 sm:p-6">
          <DataTable
            data={users}
            columns={columns}
            pagination={true}
            itemsPerPage={10}
            loading={isLoading}
          />
        </div>
      </div>

      <Modal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        title="Create New User"
        maxWidth="lg"
      >
        <UserCreateForm
          onSuccess={handleCreateUser}
          onCancel={() => setIsCreateModalOpen(false)}
        />
      </Modal>
    </div>
  );
};
