import { ToastsService } from './../shared/toasts.service';
import { AuthService } from './auth.service';
import { ToastType } from './../shared/toasts/toasts.component';
import { Router } from '@angular/router';
import { AuthApiService } from './auth-api.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  user: any = {};
  constructor(private api: AuthApiService, private route: Router, private authService: AuthService, private toast: ToastsService) { }

  ngOnInit() {
  }

  login() {
    this.api.postLogin(this.user)
      .subscribe(d => {
        this.authService.authInfo = d;
        if (d['authenticated']) {
          this.route.navigate(['home']);
        } else {
          this.toast.showMessage({
            message: d['message'],
            title: 'Erro!',
            type: ToastType.error
          });
        }
      });
  }

  keypress($event: KeyboardEvent) {
    if ($event.code === 'Enter') {
      this.login();
    }
  }

}
