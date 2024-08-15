import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../api.service';

@Component({
  selector: 'app-expense',
  templateUrl: './expense.component.html',
  styleUrls: ['./expense.component.css']
})
export class ExpenseComponent implements OnInit {
  expense: any;

  constructor(private route: ActivatedRoute, private apiService: ApiService, private router: Router) { }

  ngOnInit(): void {
    const expenseId = this.route.snapshot.params['id'];
    this.apiService.getExpense(expenseId).subscribe(data => {
      this.expense = data;
    });
  }

  payExpense() {
    this.apiService.payExpense(this.expense.id).subscribe(() => {
      this.router.navigate(['/group', this.expense.groupId]);
    });
  }
}

