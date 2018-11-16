import { endpoints } from 'src/environments/endpoints';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { take } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChartsApiService {

  url = environment.api + endpoints.dashboard;
  constructor(private httpClient: HttpClient) { }

  getPlrMassaSalarial(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}plr/massasalarial/${ano}`).pipe(take(1));
  }

  getPlrTrabalhadores(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}plr/trabalhadores/${ano}`).pipe(take(1));
  }

  getActMassaSalarial(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}act/massasalarial/${ano}`).pipe(take(1));
  }

  getActTrabalhadores(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}act/trabalhadores/${ano}`).pipe(take(1));
  }
}
