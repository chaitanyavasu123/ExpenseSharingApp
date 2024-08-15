import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { UserHomeComponent } from './user-home/user-home.component';
import { CreateGroupComponent } from './create-group/create-group.component';
import { GroupComponent } from './group/group.component';
import { ExpenseComponent } from './expense/expense.component';
import { CreateExpenseComponent } from './create-expense/create-expense.component';
import { AdminHomeComponent } from './admin-home/admin-home.component';
import { AuthGuard } from './auth.guard';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'user-home', component: UserHomeComponent, canActivate: [AuthGuard] },
  { path: 'create-group', component: CreateGroupComponent, canActivate: [AuthGuard] },
  { path: 'group/:id', component: GroupComponent, canActivate: [AuthGuard] },
  { path: 'expense/:id', component: ExpenseComponent, canActivate: [AuthGuard] },
  { path: 'create-expense/:id', component: CreateExpenseComponent, canActivate: [AuthGuard] },
  { path: 'admin-home', component: AdminHomeComponent, canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

