import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../api.service';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-group',
  templateUrl: './group.component.html',
  styleUrls: ['./group.component.css']
})
export class GroupComponent implements OnInit {
  group: any = {
    members: [],
    expenses: []
  };
  user:number=0;

  constructor(private route: ActivatedRoute, private apiService: ApiService,private router: Router) { }

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      const helper = new JwtHelperService();
      const decodedToken: any = helper.decodeToken(token);
      const userId= decodedToken.UserId;
      if(userId){
        this.user=userId;
      }
    }
    this.route.params.subscribe(params => {
      const groupId = params['id'];
      if (groupId) {
        this.apiService.getGroup(groupId).subscribe(data => {
          this.group = data;
        }, error => {
          console.error('Error fetching group details:', error);
        });
      } else {
        console.error('Invalid group ID');
      }
    });
  }
  navigateToCreateExpense(groupId: number): void {
    if (groupId) {
      console.log('Navigating to group with ID:', groupId);
      this.router.navigate(['/create-expense', groupId]);
    } else {
      console.error('Invalid group ID:', groupId);
    }
  }
  exitGroup(groupId: number): void {
    this.apiService.exitGroup(groupId).subscribe(() => {
      this.router.navigate(['/user-home']);
    });
  }
  deleteGroup(groupId: number) {
    this.apiService.deleteGroup(groupId).subscribe(() => {
      this.router.navigate(['/user-home']);
    });
  }

}
