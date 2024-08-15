import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'https://localhost:7010/api';

  constructor(private http: HttpClient) { }

  login(credentials: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/User/login`, credentials);
  }
  getUserById(userId: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/User/${userId}`,  this.getAuthHeaders());
  }

  getGroups(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Group/GetAllGroups`);
  }

  getUserGroups(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/User/${userId}/groups`, this.getAuthHeaders());
  }

  joinGroup(groupId: number): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/User/groups/${groupId}/join`, {}, this.getAuthHeaders());
  }

  createGroup(group: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/User/CreateGroup`, group, this.getAuthHeaders());
  }

  getGroup(groupId: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Group/${groupId}`, this.getAuthHeaders());
  }

  getExpense(expenseId: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Expense/${expenseId}`, this.getAuthHeaders());
  }

  payExpense(expenseshareId: number): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Expense/${expenseshareId}/pay`, {}, this.getAuthHeaders());
  }

  createExpense(groupId: number, expense: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Expense`, expense, this.getAuthHeaders());
  }
  addExpense(expense: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Expense`, expense,  this.getAuthHeaders());
  }
  GetUnpaidExpenses(userId:number):Observable<any>{
    return this.http.get<any>(`${this.baseUrl}/Expense/unpaid/${userId}`,  this.getAuthHeaders());
  }
  getGroupsUserIsNotIn(userId: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Group/user-not-in/${userId}`,  this.getAuthHeaders());
  }
  exitGroup(groupId: number): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/User/groups/${groupId}/exit`, {}, this.getAuthHeaders());
  }
  deleteGroup(groupId: number): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/Group/${groupId}/delete`,this.getAuthHeaders());
  }
  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
      headers: new HttpHeaders({
        Authorization: `Bearer ${token}`
      })
    };
  }
}

