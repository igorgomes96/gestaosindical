import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LaboraisModule } from './laborais/laborais.module';
import { SindicatosRoutingModule } from './sindicatos.routing.module';
import { PatronaisModule } from './patronais/patronais.module';
import { SharedModule } from './../shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    LaboraisModule,
    PatronaisModule,
    SharedModule,
    SindicatosRoutingModule
  ],
  declarations: []
})
export class SindicatosModule { }
