import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { endpoints } from 'src/environments/endpoints';
import { HttpClient } from '@angular/common/http';
import { take } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthApiService {

  url: string;
  constructor(private httpClient: HttpClient) {
    this.url = environment.api + endpoints.login;
  }

  postLogin(user: any): Observable<any> {
    return this.httpClient.post(this.url, user).pipe(take(1));
  }

  postUser(user: any): Observable<any> {
    return this.httpClient.post(`${this.url}registro`, user).pipe(take(1));
  }

  postSendRecoveryCode(email: string): Observable<any> {
    return this.httpClient.post(`${this.url}${email}/sendcode`, null).pipe(take(1));
  }

  postChangePassword(usuario: any): Observable<any> {
    return this.httpClient.post(`${this.url}changepassword`, usuario).pipe(take(1));
  }

}
