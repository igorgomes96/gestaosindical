import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from './../shared/shared.module';
import { LoginComponent } from './login.component';
import { RegisterComponent } from './register/register.component';
import { EnviaCodigoComponent } from './envia-codigo/envia-codigo.component';
import { RecuperacaoComponent } from './recuperacao/recuperacao.component';


@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule
  ],
  declarations: [LoginComponent, RegisterComponent, EnviaCodigoComponent, RecuperacaoComponent],
  exports: [LoginComponent]
})
export class AuthModule { }
