import { CommonModule } from '@angular/common';
import { UserService } from './../../services/user/user.service';
import { Component, EventEmitter, Output } from '@angular/core';
import { FormGroup,FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormField } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule, MatCardTitle } from '@angular/material/card';
import { Router, RouterModule } from '@angular/router';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule,MatInputModule,MatButtonModule,MatFormField,MatCardModule,MatCardTitle,RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

  //Keep it simple - avoid overly complicated hierarchies
  //Avoid circular dependencies (try to avoid bidirectional communciation => ngmodel can be better scenario)
  //User observable services to communicate between components for advanced scenarios
 // @Output() userEmit: EventEmitter<string> = new EventEmitter<string>();



  loginForm :FormGroup= new FormGroup({
    //we can use untyped form control but it is better to use typed form control
    //we can also do email:new FormControl<string>('',{Validators.required, Validators.minLength(5)})
    //password:new FormControl<string>('',{Validators.required, Validators.minLength(5),Validators.pattern('^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$')})
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
    rememberMe: new FormControl(false)
  });


  constructor(private userService:UserService,private router:Router,private snackBar:MatSnackBar){

  }

  get loginFormControl(){
    return this.loginForm.controls;
  }

  loginBtnClick(){

    if(this.loginForm.valid){
      const email = this.loginForm.value.email;
      const password = this.loginForm.value.password;
      const rememberMe = this.loginForm.value.rememberMe;

      this.userService.login(email, password, rememberMe).subscribe(
        (response:any )=> {
          this.snackBar.open('Login successful', 'Close', {
            duration: 3000,
          });
          console.log('Login successful', response);
          this.router.navigate(['/article-a']);
        },
        (error:any) => {
          this.snackBar.open('Your email or password is incorrect', 'Close', {
            duration: 3000,
          });
          console.error('Login failed', error);
        }
      );
    }else{
      console.log('Invalid form');
    }

  }


}
