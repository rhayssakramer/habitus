export interface User {
  id?: number;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  dateOfBirth?: Date;
  profilePicture?: string;
  
  // Endereço
  street?: string;
  number?: string;
  complement?: string;
  neighborhood?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  
  // Status da conta
  isActive?: boolean;
  isEmailConfirmed?: boolean;
  role?: UserRole;
  createdAt?: Date;
  lastLoginAt?: Date;
}

export interface RegisterUserDto {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  phone?: string;
  dateOfBirth?: Date;
  
  // Endereço
  street?: string;
  number?: string;
  complement?: string;
  neighborhood?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  
  profilePicture?: string;
}

export interface UpdateProfileDto {
  firstName: string;
  lastName: string;
  phone?: string;
  dateOfBirth?: Date;
  
  // Endereço
  street?: string;
  number?: string;
  complement?: string;
  neighborhood?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
}

export interface LoginDto {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  user: User;
  expiresAt: Date;
}

export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
}

export interface CepResponse {
  zipCode: string;
  street: string;
  neighborhood: string;
  city: string;
  state: string;
  country?: string;
}

export enum UserRole {
  User = 'User',
  Admin = 'Admin'
}

export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}
