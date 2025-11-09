import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { ApiResponse, LoginRequest, LoginResponse, User, Company, Project, SaleOrder, CreateSaleOrderDto } from '../types';

// Base API configuration
const API_BASE_URL = 'https://localhost:5001/api'; // Update this to match your backend URL

class ApiService {
  private axiosInstance: AxiosInstance;

  constructor() {
    this.axiosInstance = axios.create({
      baseURL: API_BASE_URL,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    this.setupInterceptors();
  }

  private setupInterceptors() {
    // Request interceptor to add auth token
    this.axiosInstance.interceptors.request.use(
      async (config) => {
        const token = await AsyncStorage.getItem('accessToken');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Response interceptor to handle errors
    this.axiosInstance.interceptors.response.use(
      (response) => response,
      async (error) => {
        if (error.response?.status === 401) {
          // Token expired or invalid
          await AsyncStorage.removeItem('accessToken');
          await AsyncStorage.removeItem('refreshToken');
          await AsyncStorage.removeItem('user');
          // Navigate to login screen - implement based on your navigation structure
        }
        return Promise.reject(error);
      }
    );
  }

  // Authentication endpoints
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    try {
      const response = await this.axiosInstance.post<ApiResponse<LoginResponse>>('/TokenAuth/authenticate', credentials);
      const data = response.data;
      
      if (data.success && data.result) {
        // Store tokens and user data
        await AsyncStorage.setItem('accessToken', data.result.accessToken);
        await AsyncStorage.setItem('refreshToken', data.result.refreshToken);
        await AsyncStorage.setItem('user', JSON.stringify(data.result.user));
        return data.result;
      } else {
        throw new Error(data.error || 'Login failed');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Login failed');
    }
  }

  async register(userData: any): Promise<User> {
    try {
      const response = await this.axiosInstance.post<ApiResponse<User>>('/auth/register', userData);
      const data = response.data;
      
      if (data.success && data.result) {
        return data.result;
      } else {
        throw new Error(data.error || 'Registration failed');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Registration failed');
    }
  }

  async logout(): Promise<void> {
    try {
      await this.axiosInstance.post('/auth/logout');
    } catch (error) {
      // Continue with logout even if API call fails
    } finally {
      await AsyncStorage.removeItem('accessToken');
      await AsyncStorage.removeItem('refreshToken');
      await AsyncStorage.removeItem('user');
    }
  }

  // User endpoints
  async getUsers(): Promise<User[]> {
    try {
      const response = await this.axiosInstance.get<ApiResponse<User[]>>('/users');
      const data = response.data;
      
      if (data.success && data.result) {
        return data.result;
      } else {
        throw new Error(data.error || 'Failed to fetch users');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Failed to fetch users');
    }
  }

  async getUser(id: number): Promise<User> {
    try {
      const response = await this.axiosInstance.get<ApiResponse<User>>(`/users/${id}`);
      const data = response.data;
      
      if (data.success && data.result) {
        return data.result;
      } else {
        throw new Error(data.error || 'Failed to fetch user');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Failed to fetch user');
    }
  }

  // Company endpoints
  async getCompanies(): Promise<Company[]> {
    try {
      const response = await this.axiosInstance.get<ApiResponse<Company[]>>('/companies');
      const data = response.data;
      
      if (data.success && data.result) {
        return data.result;
      } else {
        throw new Error(data.error || 'Failed to fetch companies');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Failed to fetch companies');
    }
  }

  async getCompany(id: number): Promise<Company> {
    try {
      const response = await this.axiosInstance.get<ApiResponse<Company>>(`/companies/${id}`);
      const data = response.data;
      
      if (data.success && data.result) {
        return data.result;
      } else {
        throw new Error(data.error || 'Failed to fetch company');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Failed to fetch company');
    }
  }

  async createCompany(company: Partial<Company>): Promise<Company> {
    try {
      const response = await this.axiosInstance.post<ApiResponse<Company>>('/companies', company);
      const data = response.data;
      
      if (data.success && data.result) {
        return data.result;
      } else {
        throw new Error(data.error || 'Failed to create company');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Failed to create company');
    }
  }

  async updateCompany(id: number, company: Partial<Company>): Promise<Company> {
    try {
      const response = await this.axiosInstance.put<ApiResponse<Company>>(`/companies/${id}`, company);
      const data = response.data;
      
      if (data.success && data.result) {
        return data.result;
      } else {
        throw new Error(data.error || 'Failed to update company');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Failed to update company');
    }
  }

  // Project endpoints
  async getProjects(): Promise<Project[]> {
    try {
      const response = await this.axiosInstance.get<ApiResponse<{ items: Project[] }>>('/projects');
      const data = response.data;
      
      if (data.success && data.result) {
        return data.result.items || [];
      } else {
        throw new Error(data.error || 'Failed to fetch projects');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Failed to fetch projects');
    }
  }

  async createProject(project: Partial<Project>): Promise<Project> {
    try {
      const response = await this.axiosInstance.post<ApiResponse<Project>>('/projects', project);
      const data = response.data;
      
      if (data.success && data.result) {
        return data.result;
      } else {
        throw new Error(data.error || 'Failed to create project');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Failed to create project');
    }
  }

  // Sale Order endpoints
  async getSaleOrders(): Promise<SaleOrder[]> {
    try {
      const response = await this.axiosInstance.get<ApiResponse<{ items: SaleOrder[] }>>('/saleorders');
      const data = response.data;
      
      if (data.success && data.result) {
        return data.result.items || [];
      } else {
        throw new Error(data.error || 'Failed to fetch sale orders');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Failed to fetch sale orders');
    }
  }

  async getSaleOrder(id: number): Promise<SaleOrder> {
    try {
      const response = await this.axiosInstance.get<ApiResponse<SaleOrder>>(`/saleorders/${id}`);
      const data = response.data;
      
      if (data.success && data.result) {
        return data.result;
      } else {
        throw new Error(data.error || 'Failed to fetch sale order');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Failed to fetch sale order');
    }
  }

  async createSaleOrder(saleOrder: CreateSaleOrderDto): Promise<SaleOrder> {
    try {
      const response = await this.axiosInstance.post<ApiResponse<SaleOrder>>('/saleorders', saleOrder);
      const data = response.data;
      
      if (data.success && data.result) {
        return data.result;
      } else {
        throw new Error(data.error || 'Failed to create sale order');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Failed to create sale order');
    }
  }

  async updateSaleOrder(id: number, saleOrder: Partial<CreateSaleOrderDto>): Promise<SaleOrder> {
    try {
      const response = await this.axiosInstance.put<ApiResponse<SaleOrder>>(`/saleorders/${id}`, saleOrder);
      const data = response.data;
      
      if (data.success && data.result) {
        return data.result;
      } else {
        throw new Error(data.error || 'Failed to update sale order');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Failed to update sale order');
    }
  }

  async deleteSaleOrder(id: number): Promise<void> {
    try {
      await this.axiosInstance.delete(`/saleorders/${id}`);
    } catch (error: any) {
      throw new Error(error.response?.data?.error || error.message || 'Failed to delete sale order');
    }
  }

  // Utility methods
  async isAuthenticated(): Promise<boolean> {
    const token = await AsyncStorage.getItem('accessToken');
    return !!token;
  }

  async getCurrentUser(): Promise<User | null> {
    try {
      const userString = await AsyncStorage.getItem('user');
      return userString ? JSON.parse(userString) : null;
    } catch (error) {
      return null;
    }
  }
}

export default new ApiService();