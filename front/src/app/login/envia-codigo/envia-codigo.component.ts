import { Router } from '@angular/router';
import { ToastsService } from 'src/app/shared/toasts.service';
import { Component, OnInit } from '@angular/core';
import { ToastType } from 'src/app/shared/toasts/toasts.component';

import * as Ladda from 'ladda';
import { finalize } from 'rxjs/operators';
import { AuthApiService } from 'src/app/shared/api/auth-api.service';

@Component({
  selector: 'app-envia-codigo',
  templateUrl: './envia-codigo.component.html',
  styleUrls: ['./envia-codigo.component.css']
})
export class EnviaCodigoComponent implements OnInit {

  email: string;
  buttonLoad: Ladda.LaddaButton;
  constructor(private api: AuthApiService, private toasts: ToastsService,
    private router: Router) { }

  ngOnInit() {
    this.buttonLoad = Ladda.create(document.querySelector('.ladda-button'));
  }

  enviarCodigo() {
    this.buttonLoad.start();
    this.api.postSendRecoveryCode(this.email)
    .pipe(finalize(() =>  this.buttonLoad.stop()))
    .subscribe(_ => {
      this.toasts.showMessage({
        message: 'Um código de recuperação foi enviado para seu e-mail!',
        title: 'Sucesso!',
        type: ToastType.success
      });
      this.router.navigate(['/recuperacao']);
    });
  }

}
