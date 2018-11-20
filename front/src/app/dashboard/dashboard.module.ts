import { NgxMaskModule } from 'ngx-mask';
import { Ng5SliderModule } from 'ng5-slider';
import { SharedModule } from './../shared/shared.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardNegociacoesComponent } from './dashboard-negociacoes/dashboard-negociacoes.component';
import { DashboardEmpresasComponent } from './dashboard-empresas/dashboard-empresas.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

@NgModule({
  imports: [
    CommonModule,
    DashboardRoutingModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    NgxMaskModule,
    Ng5SliderModule
  ],
  declarations: [DashboardNegociacoesComponent, DashboardEmpresasComponent]
})
export class DashboardModule { }
