import { NgxMaskModule } from 'ngx-mask';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { PatronalListComponent } from './patronal-list/patronal-list.component';
import { PatronalFormComponent } from './patronal-form/patronal-form.component';
import { SharedModule } from 'src/app/shared/shared.module';


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
  declarations: [PatronalListComponent, PatronalFormComponent]
})
export class PatronaisModule { }
