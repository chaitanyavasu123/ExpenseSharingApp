import { Component, OnInit } from '@angular/core';
import { ApiService } from '../api.service';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-user-home',
  templateUrl: './user-home.component.html',
  styleUrls: ['./user-home.component.css']
})
export class UserHomeComponent implements OnInit {
  groups: any[] = [];
  groupsNotIn: any[] = [];
  user:number=0;
  userName:string="";
  unpaidExpenses: any[] = [];
  member:any;

  constructor(private apiService: ApiService, private router: Router) { }

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      const helper = new JwtHelperService();
      const decodedToken: any = helper.decodeToken(token);
      const username=decodedToken.Name;
      if(username){
        this.userName=username;
      }
      const userId = decodedToken.UserId;
      if (userId) {
        this.user=userId;
        this.apiService.getUserById(userId).subscribe(data => {
          this.member = data;
        });
        this.apiService.getUserGroups(userId).subscribe(data => {
          this.groups = data;
        });
        this.apiService.GetUnpaidExpenses(userId).subscribe(data => {
          this.unpaidExpenses = data;
        });
        this.loadGroupsNotIn();
      } else {
        console.error('UserId not found in token');
      }
    } else {
      console.error('No JWT token found in local storage');
    }
  }
  joinGroup(groupId: number):void {
    this.apiService.joinGroup(groupId).subscribe(() => {
     this.reloadComponent();
    });
  }
  navigateToGroup(groupId: number): void {
    if (groupId) {
      console.log('Navigating to group with ID:', groupId);
      this.router.navigate(['/group', groupId]);
    } else {
      console.error('Invalid group ID:', groupId);
    }
  }
  payExpense(expenseShareId: number):void {
    
    this.apiService.payExpense(expenseShareId)
      .subscribe(() => {
        window.location.reload();
      });
  }
  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/']);
  }
  reloadComponent(): void {
    this.router.navigate([this.router.url]);
  }
  loadGroupsNotIn(): void {
    this.apiService.getGroupsUserIsNotIn(this.user).subscribe(data => {
      this.groupsNotIn = data;
    });
  }
 

}
