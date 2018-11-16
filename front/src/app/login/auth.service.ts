import { AuthInfo } from './auth-info';
import { Injectable, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  onUserChanges: EventEmitter<AuthInfo> = new EventEmitter<AuthInfo>();

  constructor() { }

  set authInfo(info: AuthInfo) {
    localStorage.setItem('auth', JSON.stringify(info));
    this.onUserChanges.emit(info);
  }

  get authInfo(): AuthInfo {
    return JSON.parse(localStorage.getItem('auth'));
  }

  logout() {
    localStorage.removeItem('auth');
  }

  isAuthenticated() {
    const auth = this.authInfo;
    return auth && auth.authenticated && (new Date(auth.expiration) > new Date());
  }

  isInRole(role: string) {
    return this.authInfo && this.authInfo.roles.some(r => r === role);
  }

}
