import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AgendaListComponent } from './agenda-list/agenda-list.component';
import { AgendaFormComponent } from './agenda-form/agenda-form.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { RodadaComponent } from './rodada/rodada.component';
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
  declarations: [AgendaListComponent, AgendaFormComponent, RodadaComponent]
})
export class AgendaModule { }
