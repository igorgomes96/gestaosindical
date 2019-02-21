import { AuthService } from './../auth.service';
import { Router } from '@angular/router';
import { ToastsService } from 'src/app/shared/toasts.service';
import { FormValidators } from './../../shared/form-validators';
import { FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { switchMap, tap, finalize } from 'rxjs/operators';
import { ToastType } from 'src/app/shared/toasts/toasts.component';

import * as Ladda from 'ladda';
import { AuthApiService } from 'src/app/shared/api/auth-api.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  form: FormGroup;
  buttonLoad: Ladda.LaddaButton;
  constructor(private formBuilder: FormBuilder,
    private api: AuthApiService, private toasts: ToastsService,
    private router: Router, private authService: AuthService) { }

  ngOnInit() {
    this.buttonLoad = Ladda.create(document.querySelector('.ladda-button'));
    this.form = this.formBuilder.group({
      login: ['', [Validators.required, Validators.email]],
      nome: ['', [Validators.required]],
      senha: ['', [Validators.required, Validators.minLength(8)]],
      confirmaSenha: ['', [Validators.required, FormValidators.equalsTo('senha')]]
    });
  }

  hasError(formControl: AbstractControl) {
    return formControl.dirty && formControl.invalid;
  }

  registro() {
    this.buttonLoad.start();
    const user = this.form.value;
    this.api.postUser(this.form.value)
      .pipe(
        tap(_ => this.toasts.showMessage({ message: 'Usuário registrado com sucesso!', title: 'Sucesso!', type: ToastType.success })),
        switchMap(_ => this.api.postLogin(user)),
        finalize(() => this.buttonLoad.stop())
      ).subscribe(d => {
        this.authService.authInfo = d;
        if (d['authenticated']) {
          this.router.navigate(['home']);
        } else {
          this.toasts.showMessage({
            message: d['message'],
            title: 'Erro!',
            type: ToastType.error
          });
        }
      });
  }

}
