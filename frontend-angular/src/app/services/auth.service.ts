import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject, tap, catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { 
  User, 
  LoginDto, 
  RegisterUserDto, 
  AuthResponse, 
  ChangePasswordDto, 
  UpdateProfileDto,
  CepResponse,
  ApiResponse 
} from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = environment.apiUrl;
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  private tokenSubject = new BehaviorSubject<string | null>(null);

  public currentUser$ = this.currentUserSubject.asObservable();
  public token$ = this.tokenSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    // Carregar dados do localStorage na inicialização
    this.loadFromStorage();
  }

  private loadFromStorage(): void {
    const token = localStorage.getItem('authToken');
    const user = localStorage.getItem('user');
    
    if (token && user) {
      this.tokenSubject.next(token);
      this.currentUserSubject.next(JSON.parse(user));
    }
  }

  private getHttpOptions(): { headers: HttpHeaders } {
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
  }

  private getAuthenticatedHttpOptions(): { headers: HttpHeaders } {
    const token = this.tokenSubject.value;
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      })
    };
  }

  // Autenticação
  login(credentials: LoginDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, credentials, this.getHttpOptions())
      .pipe(
        tap(response => {
          this.setSession(response);
        }),
        catchError(this.handleError)
      );
  }

  register(userData: RegisterUserDto): Observable<ApiResponse<User>> {
    return this.http.post<ApiResponse<User>>(`${this.apiUrl}/registration`, userData, this.getHttpOptions())
      .pipe(
        catchError(this.handleError)
      );
  }

  logout(): void {
    // Chamar API de logout se necessário
    this.http.post(`${this.apiUrl}/auth/logout`, {}, this.getAuthenticatedHttpOptions())
      .subscribe({
        complete: () => this.clearSession()
      });
    
    this.clearSession();
  }

  refreshToken(): Observable<AuthResponse> {
    const refreshToken = localStorage.getItem('refreshToken');
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/refresh`, { refreshToken }, this.getHttpOptions())
      .pipe(
        tap(response => {
          this.setSession(response);
        }),
        catchError(error => {
          this.clearSession();
          return throwError(() => error);
        })
      );
  }

  // Gerenciamento de sessão
  private setSession(authResponse: AuthResponse): void {
    localStorage.setItem('authToken', authResponse.token);
    localStorage.setItem('refreshToken', authResponse.refreshToken);
    localStorage.setItem('user', JSON.stringify(authResponse.user));
    localStorage.setItem('expiresAt', authResponse.expiresAt.toString());
    
    this.tokenSubject.next(authResponse.token);
    this.currentUserSubject.next(authResponse.user);
  }

  private clearSession(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    localStorage.removeItem('expiresAt');
    
    this.tokenSubject.next(null);
    this.currentUserSubject.next(null);
    
    this.router.navigate(['/login']);
  }

  // Verificações de estado
  isAuthenticated(): boolean {
    const token = this.tokenSubject.value;
    const expiresAt = localStorage.getItem('expiresAt');
    
    if (!token || !expiresAt) {
      return false;
    }
    
    return new Date() < new Date(expiresAt);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    return this.tokenSubject.value;
  }

  // Perfil do usuário
  getProfile(): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/registration/profile`, this.getAuthenticatedHttpOptions())
      .pipe(
        tap(user => {
          this.currentUserSubject.next(user);
          localStorage.setItem('user', JSON.stringify(user));
        }),
        catchError(this.handleError)
      );
  }

  updateProfile(profileData: UpdateProfileDto): Observable<ApiResponse<User>> {
    return this.http.put<ApiResponse<User>>(`${this.apiUrl}/registration/profile`, profileData, this.getAuthenticatedHttpOptions())
      .pipe(
        tap(response => {
          if (response.data) {
            this.currentUserSubject.next(response.data);
            localStorage.setItem('user', JSON.stringify(response.data));
          }
        }),
        catchError(this.handleError)
      );
  }

  uploadProfilePicture(file: File): Observable<ApiResponse<string>> {
    const formData = new FormData();
    formData.append('file', file);
    
    const token = this.tokenSubject.value;
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
    
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/registration/profile/picture`, formData, { headers })
      .pipe(
        catchError(this.handleError)
      );
  }

  changePassword(passwordData: ChangePasswordDto): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(`${this.apiUrl}/auth/change-password`, passwordData, this.getAuthenticatedHttpOptions())
      .pipe(
        catchError(this.handleError)
      );
  }

  // Validações
  checkEmailExists(email: string): Observable<{ exists: boolean }> {
    return this.http.get<{ exists: boolean }>(`${this.apiUrl}/registration/check-email/${email}`, this.getHttpOptions())
      .pipe(
        catchError(this.handleError)
      );
  }

  // CEP
  lookupCep(cep: string): Observable<CepResponse> {
    return this.http.get<CepResponse>(`${this.apiUrl}/registration/cep/${cep}`, this.getHttpOptions())
      .pipe(
        catchError(this.handleError)
      );
  }

  // Tratamento de erros
  private handleError(error: any): Observable<never> {
    console.error('API Error:', error);
    
    let errorMessage = 'Erro desconhecido';
    
    if (error.error?.message) {
      errorMessage = error.error.message;
    } else if (error.error?.errors && Array.isArray(error.error.errors)) {
      errorMessage = error.error.errors.join(', ');
    } else if (error.message) {
      errorMessage = error.message;
    }
    
    return throwError(() => new Error(errorMessage));
  }
}
