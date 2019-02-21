import { ToastsService } from './../shared/toasts.service';
import { AuthService } from './auth.service';
import { ToastType } from './../shared/toasts/toasts.component';
import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';

import * as Ladda from 'ladda';
import { finalize } from 'rxjs/operators';
import { AuthApiService } from '../shared/api/auth-api.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  user: any = {};
  buttonLoad: Ladda.LaddaButton;
  constructor(private api: AuthApiService, private route: Router, private authService: AuthService, private toast: ToastsService) { }

  ngOnInit() {
    this.buttonLoad = Ladda.create(document.querySelector('.ladda-button'));
  }

  login() {
    this.buttonLoad.start();
    this.api.postLogin(this.user)
    .pipe(
      finalize(() => this.buttonLoad.stop())
    ).subscribe(d => {
        this.buttonLoad.stop();
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
