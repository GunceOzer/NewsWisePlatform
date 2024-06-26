import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
} from '@angular/router';
import { UserService } from '../../services/user/user.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private userService: UserService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    const roles = this.userService.getRoles();
    const expectedRoles = route.data['role'];

    if (this.hasRequiredRole(roles, expectedRoles)) {
      return true;
    } else {
      this.router.navigate(['/unauthorized']);
      return false;
    }
  }
  isAuthenticated(): boolean {
    // Check if the user is authenticated (e.g., by checking for a valid token in local storage)
    const token = localStorage.getItem('token');
    return !!token; // This will return true if token exists, false otherwise
  }
  isAdmin(): boolean {
    const roles = this.userService.getRoles();
    return roles.includes('Admin');
  }

  private hasRequiredRole(
    userRoles: string[],
    expectedRoles: string | string[]
  ): boolean {
    if (Array.isArray(expectedRoles)) {
      return expectedRoles.some((role) => userRoles.includes(role));
    }
    return userRoles.includes(expectedRoles);
  }
}
