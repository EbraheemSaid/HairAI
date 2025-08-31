import axios, { type AxiosInstance, type AxiosRequestConfig } from "axios";
import { useAuthStore } from "../store/authStore";

class ApiService {
  private axiosInstance: AxiosInstance;

  constructor() {
    this.axiosInstance = axios.create({
      baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:5000/api",
      timeout: 10000,
      headers: {
        "Content-Type": "application/json",
      },
    });

    // Add a request interceptor to include the auth token
    this.axiosInstance.interceptors.request.use(
      (config) => {
        const token = useAuthStore.getState().token;
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Add a response interceptor to handle token expiration
    this.axiosInstance.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          useAuthStore.getState().logout();
        }
        return Promise.reject(error);
      }
    );
  }

  public get<T = any>(url: string, config?: AxiosRequestConfig): Promise<T> {
    return this.axiosInstance.get(url, config).then((response) => {
      // Handle the standard backend response format
      if (
        response.data &&
        typeof response.data === "object" &&
        "success" in response.data
      ) {
        return response.data.data;
      }
      return response.data;
    });
  }

  public post<T = any>(
    url: string,
    data?: any,
    config?: AxiosRequestConfig
  ): Promise<T> {
    return this.axiosInstance.post(url, data, config).then((response) => {
      // Handle the standard backend response format
      if (
        response.data &&
        typeof response.data === "object" &&
        "success" in response.data
      ) {
        return response.data.data;
      }
      return response.data;
    });
  }

  public put<T = any>(
    url: string,
    data?: any,
    config?: AxiosRequestConfig
  ): Promise<T> {
    return this.axiosInstance.put(url, data, config).then((response) => {
      // Handle the standard backend response format
      if (
        response.data &&
        typeof response.data === "object" &&
        "success" in response.data
      ) {
        return response.data.data;
      }
      return response.data;
    });
  }

  public delete<T = any>(url: string, config?: AxiosRequestConfig): Promise<T> {
    return this.axiosInstance.delete(url, config).then((response) => {
      // Handle the standard backend response format
      if (
        response.data &&
        typeof response.data === "object" &&
        "success" in response.data
      ) {
        return response.data.data;
      }
      return response.data;
    });
  }

  public patch<T = any>(
    url: string,
    data?: any,
    config?: AxiosRequestConfig
  ): Promise<T> {
    return this.axiosInstance.patch(url, data, config).then((response) => {
      // Handle the standard backend response format
      if (
        response.data &&
        typeof response.data === "object" &&
        "success" in response.data
      ) {
        return response.data.data;
      }
      return response.data;
    });
  }
}

export const apiService = new ApiService();
