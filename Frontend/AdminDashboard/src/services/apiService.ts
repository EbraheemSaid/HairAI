import axios, { AxiosInstance, AxiosRequestConfig } from "axios";

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

    // Request interceptor to add auth token
    this.axiosInstance.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem("adminToken");
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor for error handling
    this.axiosInstance.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          localStorage.removeItem("adminToken");
          localStorage.removeItem("adminUser");
          window.location.href = "/login";
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

