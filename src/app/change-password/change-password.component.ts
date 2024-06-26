import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../services/user/user.service';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';


@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule,MatCardModule, MatCardModule],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.css'
})
export class ChangePasswordComponent {

  changePasswordForm: FormGroup;

  constructor(private userService: UserService, private router:Router, private snackBar: MatSnackBar) {
    this.changePasswordForm = new FormGroup({
      currentPassword: new FormControl('', [Validators.required]),
      newPassword: new FormControl('', [Validators.required,Validators.minLength(6)]),
      confirmPassword: new FormControl('', [Validators.required])
  },this.passwordMatchValidator);
}

  passwordMatchValidator(form: FormGroup) {
    return form.get('newPassword')!.value === form.get('confirmPassword')!.value
      ? null : { mismatch: true };
  }

  onSubmit() {
    if (this.changePasswordForm.valid) {
      const { currentPassword, newPassword, confirmPassword } = this.changePasswordForm.value;
      this.userService.changePassword(currentPassword, newPassword, confirmPassword).subscribe(
        response => {
          this.snackBar.open('Password changed successfully', 'Close', {
            duration: 3000,
          });
          this.router.navigate(['/article-a']);
        },
        error => {
          this.snackBar.open('Failed to change password', 'Close', {
            duration: 3000,
          });
          console.error('Failed to change password', error);
        }
      );
    }
  }
}
