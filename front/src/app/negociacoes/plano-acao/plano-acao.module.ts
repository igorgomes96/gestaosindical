import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlanoAcaoListComponent } from './plano-acao-list/plano-acao-list.component';
import { PlanoAcaoFormComponent } from './plano-acao-form/plano-acao-form.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/shared/shared.module';
import { RouterModule } from '@angular/router';
import { Ng5SliderModule } from 'ng5-slider';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule,
    Ng5SliderModule
  ],
  declarations: [PlanoAcaoListComponent, PlanoAcaoFormComponent]
})
export class PlanoAcaoModule { }
