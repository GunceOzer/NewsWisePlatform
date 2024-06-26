import { Injectable } from '@angular/core';
import { ApiService } from '../api/api.service';
import { map, Observable, tap } from 'rxjs';
import { User } from '../../app/models/user';
import { HttpHeaders } from '@angular/common/http';
import { Guid } from 'guid-typescript';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = 'Accounts';

  constructor(private apiService:ApiService) { }

  private getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  login(email: string, password: string, rememberMe: boolean): Observable<User> {
    const loginData = { email, password, rememberMe };

    return this.apiService.post(`${this.apiUrl}/login`, loginData).pipe(
      map((response: any) => {
        console.log('Login response:', response); // Log the entire response
        if (response && response.token && response.refreshToken) {
          const decodedToken = this.decodeToken(response.token);
          console.log('Decoded token:', decodedToken); // Log the decoded token

          const roles = decodedToken.role || decodedToken.roles;
          const userId = decodedToken.nameid || decodedToken.sub; // Ensure your token contains the userId

          if (userId) {
            response.id = userId; // Add the userId to the response object
            localStorage.setItem('currentUser', JSON.stringify({ ...response, id: userId }));
          } else {
            console.error('User ID not found in the token');
          }

          localStorage.setItem('token', response.token);
          localStorage.setItem('refreshToken', response.refreshToken);
          localStorage.setItem('roles', JSON.stringify(roles));

          if (roles.includes('Admin')) {
            console.log('Admin logged in');
          } else {
            console.log('User logged in');
          }
        }
        return response;
      })
    );
  }


  logout(): Observable<any> {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${this.getToken()}`);
    return this.apiService.post(`${this.apiUrl}/logout`, {}, { headers }).pipe(
      map((response: any) => {
        // Remove user from local storage to log user out
        localStorage.removeItem('currentUser');
        //added to remove roles from local storage
        localStorage.removeItem('roles');
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        console.log('User logged out');
        return response;
      }));
  }
  getToken(): string | null {
    const currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
    return currentUser?.token;
  }

  getRoles(): string[] {
    return JSON.parse(localStorage.getItem('roles') || '[]');
  }
  getRefreshToken(): string | null {
    const currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
    return currentUser?.refreshToken;
  }
  private decodeToken(token: string): any {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
      return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
  }
  getCurrentUserId(): Guid | null {
    const currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
    if (currentUser && currentUser.id) {
      return Guid.parse(currentUser.id);
    } else {
      console.error('User ID is missing in the currentUser object');
      return null;
    }
  }
  changePassword(currentPassword: string, newPassword: string, confirmPassword: string): Observable<any> {
    const url = `${this.apiUrl}/change-password`;
    const body = { currentPassword, newPassword, confirmPassword };
    return this.apiService.post(url, body);
  }

  register(fullName: string, email:string, password:string, confirmPassword:string):Observable<any>{
    const registrationData ={fullName, email, password, confirmPassword};
    return this.apiService.post(`${this.apiUrl}/register`, registrationData);

  }

  resetPasswordRequest(email: string): Observable<any> {
    const resetPasswordRequestData = { email };
    return this.apiService.post(`${this.apiUrl}/reset-password-request`, resetPasswordRequestData);
  }

  resetPassword(email:string, token:string, password:string , confirmPassword:string):Observable<any>{
    const resetPasswordData = {email, token, password, confirmPassword};
    return this.apiService.post(`${this.apiUrl}/reset-password`, resetPasswordData);

  }

}


