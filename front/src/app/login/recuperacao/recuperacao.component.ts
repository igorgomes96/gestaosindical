import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { ToastsService } from 'src/app/shared/toasts.service';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { FormValidators } from 'src/app/shared/form-validators';
import { switchMap } from 'rxjs/operators';
import { ToastType } from 'src/app/shared/toasts/toasts.component';

import * as Ladda from 'ladda';
import { finalize } from 'rxjs/operators';
import { AuthApiService } from 'src/app/shared/api/auth-api.service';

@Component({
  selector: 'app-recuperacao',
  templateUrl: './recuperacao.component.html',
  styleUrls: ['./recuperacao.component.css']
})
export class RecuperacaoComponent implements OnInit {

  form: FormGroup;
  buttonLoad: Ladda.LaddaButton;
  // codigoRecuperacao: string;
  constructor(private formBuilder: FormBuilder,
    private api: AuthApiService, private toasts: ToastsService,
    private router: Router, private authService: AuthService) { }

  ngOnInit() {

    this.buttonLoad = Ladda.create(document.querySelector('.ladda-button'));

    this.form = this.formBuilder.group({
      login: ['', [Validators.required, Validators.email]],
      codigoRecuperacao: ['', [Validators.required]],
      senha: ['', [Validators.required, Validators.minLength(8)]],
      confirmaSenha: ['', [Validators.required, FormValidators.equalsTo('senha')]]
    });

    // this.route.queryParamMap
    // .subscribe(q => {
    //   if (q.has('code')) {
    //     this.codigoRecuperacao = q.get('code');
    //   }
    // });
  }

  hasError(formControl: AbstractControl) {
    return formControl.dirty && formControl.invalid;
  }

  changePassword() {
    this.buttonLoad.start();
    const user = this.form.getRawValue();
    this.api.postChangePassword(user)
      .pipe(
        switchMap(_ => this.api.postLogin(user)),
        finalize(() =>  this.buttonLoad.stop())
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
