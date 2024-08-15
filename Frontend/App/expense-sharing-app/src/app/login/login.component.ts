import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../api.service';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  serverErrorMessage: string = '';


  constructor(private fb: FormBuilder, private apiService: ApiService, private router: Router) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.pattern('^[a-zA-Z0-9@#]*$')]]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.apiService.login(this.loginForm.value).subscribe(data => {
        localStorage.setItem('token', data.token);
        const token = localStorage.getItem('token');
        if (token) {
          const helper = new JwtHelperService();
          const decodedToken: any = helper.decodeToken(token);
        
        if (decodedToken.role === 'Admin') {
          this.router.navigate(['/admin-home']);
        } else {
          this.router.navigate(['/user-home']);
        }
      }
      },
      error => {
        if (error.status === 400 && error.error && error.error.errors) {
          const validationErrors = error.error.errors;
          this.serverErrorMessage = 'The entered details are not correct.';
        } else {
          this.serverErrorMessage = 'The entered details are not correct..';
        }
      }
    );
    }
  }
}

