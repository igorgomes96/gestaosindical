import { AuthService } from './../auth.service';
import { Router } from '@angular/router';
import { ToastsService } from 'src/app/shared/toasts.service';
import { AuthApiService } from './../auth-api.service';
import { FormValidators } from './../../shared/form-validators';
import { FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { switchMap, tap } from 'rxjs/operators';
import { ToastType } from 'src/app/shared/toasts/toasts.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  form: FormGroup;
  constructor(private formBuilder: FormBuilder,
    private api: AuthApiService, private toasts: ToastsService,
    private router: Router, private authService: AuthService) { }

  ngOnInit() {
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
    const user = this.form.value;
    this.api.postUser(this.form.value)
    .pipe(
      tap(_ => this.toasts.showMessage({message: 'Usuário registrado com sucesso!', title: 'Sucesso!', type: ToastType.success})),
      switchMap(_ => this.api.postLogin(user))
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
