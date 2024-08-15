import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../api.service';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-create-group',
  templateUrl: './create-group.component.html',
  styleUrls: ['./create-group.component.css']
})
export class CreateGroupComponent {
  groupForm: FormGroup;

  constructor(private fb: FormBuilder, private apiService: ApiService, private router: Router, private jwtHelper: JwtHelperService) {
    this.groupForm = this.fb.group({
      name: ['',Validators.required],
      description: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.groupForm.valid) {
      const token = localStorage.getItem('token');
      if (token) {
        const decodedToken: any = this.jwtHelper.decodeToken(token);
        const userId = decodedToken.UserId;

        
          const groupData = {
            ...this.groupForm.value,
            CreatedById: userId,
            CreatedDate: new Date()
          };

          this.apiService.createGroup(groupData).subscribe(() => {
            this.router.navigate(['/user-home']);
          
        });
      } else {
        console.error('No token found in local storage');
      }
    }
  }
}
