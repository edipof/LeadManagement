import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class LeadService {

  private apiUrl = 'https://localhost:7263/api/Leads'; 

  constructor(private http: HttpClient) {}

  getLeads(status: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/${status}`);
  }
  
  acceptLead(id: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/accept/${id}`, {});
  }

  declineLead(id: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/decline/${id}`, {});
  }
}
