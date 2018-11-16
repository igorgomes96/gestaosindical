import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { LaboralFormComponent } from './laboral-form/laboral-form.component';
import { LaboralListComponent } from './laboral-list/laboral-list.component';
import { SharedModule } from '../../shared/shared.module';
import { NgxMaskModule } from 'ngx-mask';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    HttpClientModule,
    RouterModule,
    ReactiveFormsModule,
    FormsModule,
    NgxMaskModule.forRoot()
  ],
  declarations: [
    LaboralFormComponent,
    LaboralListComponent
  ],
  exports: [
    LaboralFormComponent,
    LaboralListComponent
  ]
})
export class LaboraisModule { }
