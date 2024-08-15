import { Component, OnInit } from '@angular/core';
import { ApiService } from '../api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-home',
  templateUrl: './admin-home.component.html',
  styleUrls: ['./admin-home.component.css']
})
export class AdminHomeComponent implements OnInit {
  groups: any[] = [];
  expenses: any[] = [];

  constructor(private apiService: ApiService, private router: Router) { }

  ngOnInit(): void {
    this.loadGroups();
    
  }

  loadGroups(): void {
    this.apiService.getGroups().subscribe(data => {
      this.groups = data;
    });
  }
  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/']);
  }
 
}
