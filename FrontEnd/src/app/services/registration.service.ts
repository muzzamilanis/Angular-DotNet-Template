import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface UserRegistrationDto {
  name: string;
  email: string;
  password: string;
}

export interface CompanyRegistrationDto {
  companyName: string;
  address?: string;
  phone?: string;
  email?: string;
  contactPerson?: string;
}

export interface RegistrationDto {
  user: UserRegistrationDto;
  company: CompanyRegistrationDto;
}

export interface RegistrationResponseDto {
  success: boolean;
  message: string;
  userId?: number;
  companyId?: number;
}

@Injectable({
  providedIn: 'root'
})
export class RegistrationService {
  private apiUrl = environment.apiUrl || 'https://localhost:7000/api';

  constructor(private http: HttpClient) { }

  register(registrationData: RegistrationDto): Observable<RegistrationResponseDto> {
    return this.http.post<RegistrationResponseDto>(`${this.apiUrl}/registration`, registrationData);
  }
}