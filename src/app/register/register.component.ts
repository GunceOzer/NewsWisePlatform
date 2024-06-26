import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { UserService } from '../../services/user/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule,MatFormFieldModule,MatInputModule, MatButtonModule, MatCardModule, MatSnackBarModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {

  registerForm: FormGroup;

  constructor(private userService:UserService, private router:Router, private snackBar: MatSnackBar) {
    this.registerForm = new FormGroup({
      fullName: new FormControl('', [Validators.required]),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required, Validators.minLength(6)]),
      confirmPassword: new FormControl('', [Validators.required])
    }, this.passwordMatchValidator);
  }

  passwordMatchValidator(form: FormGroup) {
    return form.get('password')!.value === form.get('confirmPassword')!.value
      ? null : { mismatch: true };
  }

  onSubmit() {
    if (this.registerForm.valid) {
      const { fullName, email, password, confirmPassword } = this.registerForm.value;
      this.userService.register(fullName, email, password, confirmPassword).subscribe(
        response => {
          this.snackBar.open('Registration successful', 'Close', {
            duration: 3000,
          });
          this.router.navigate(['/login']);
        },
        error => {
          this.snackBar.open('Failed to register', 'Close', {
            duration: 3000,
          });
          console.error('Failed to register', error);
        }
      );
    }
  }
}
