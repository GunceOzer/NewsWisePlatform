import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { UserService } from '../../services/user/user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatCardModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent {
  resetPasswordForm: FormGroup;
  email: string;
  token: string;

  constructor(private route: ActivatedRoute, private userService: UserService, private router: Router, private snackBar: MatSnackBar) {
    this.resetPasswordForm = new FormGroup({
      password: new FormControl('', [Validators.required, Validators.minLength(6)]),
      confirmPassword: new FormControl('', [Validators.required])
    }, this.passwordMatchValidator);
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.token = decodeURIComponent(params['token']);
      this.email = params['email'];
    });
  }

  passwordMatchValidator(form: FormGroup) {
    return form.get('password')!.value === form.get('confirmPassword')!.value ? null : { mismatch: true };
  }

  onSubmit() {
    if (this.resetPasswordForm.valid) {
      const { password } = this.resetPasswordForm.value;
      this.userService.resetPassword(this.email, this.token, password, password).subscribe(
        response => {
          this.snackBar.open('Password has been reset successfully.', 'Close', {
            duration: 5000
          });
          this.router.navigate(['/login']);
        },
        error => {
          this.snackBar.open('Failed to reset password.', 'Close', {
            duration: 5000
          });
        }
      );
    }
  }
}
