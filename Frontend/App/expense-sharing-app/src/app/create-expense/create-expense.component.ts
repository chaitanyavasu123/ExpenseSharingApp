import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../api.service';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-create-expense',
  templateUrl: './create-expense.component.html',
  styleUrls: ['./create-expense.component.css']
})
export class CreateExpenseComponent implements OnInit {
  expenseForm: FormGroup;
  groupId: number=0;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.expenseForm = this.fb.group({
      description: ['', Validators.required],
      amount: ['', [Validators.required, Validators.min(0.01)]],
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.groupId = +params['id'];
    });
  }

  onSubmit() {
    if (this.expenseForm.valid) {
      const token = localStorage.getItem('token');
      if (token) {
        const helper = new JwtHelperService();
        const decodedToken: any = helper.decodeToken(token);
        const userId = decodedToken.UserId;
        const expenseData = {
          description: this.expenseForm.value.description,
          amount: this.expenseForm.value.amount,
          date: this.expenseForm.value.date,
          groupId: this.groupId,
          paidById: userId,
          expenseShares: []  // Adjust as needed
        };
        this.apiService.addExpense(expenseData).subscribe(() => {
          this.router.navigate(['/group', this.groupId]);
        }, error => {
          console.error('Error adding expense:', error);
        });
      } else {
        console.error('No JWT token found in local storage');
      }
    }
  }
}
