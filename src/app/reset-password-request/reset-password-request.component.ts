import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { UserService } from '../../services/user/user.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { RouterModule } from '@angular/router';


@Component({
  selector: 'app-reset-password-request',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatCardModule,RouterModule],
  templateUrl: './reset-password-request.component.html',
  styleUrl: './reset-password-request.component.css'
})
export class ResetPasswordRequestComponent {

  resetPasswordRequestForm: FormGroup;

  constructor(private userService: UserService,private snackBar: MatSnackBar){
    this.resetPasswordRequestForm = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.email])
    })
  }

  onSubmit(){
    if(this.resetPasswordRequestForm.valid){
      const { email } = this.resetPasswordRequestForm.value;
      this.userService.resetPasswordRequest(email).subscribe(
        response => {
          this.snackBar.open('Reset password request sent to your email', 'Close', {
            duration: 3000,
          });
          console.log('Reset password request sent');
        },
        error => {
          this.snackBar.open('Failed to send reset password request', 'Close', {
            duration: 3000,
          });
          console.error('Failed to send reset password request', error);
        }
      );
    }
  }
}
