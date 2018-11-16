import { Ng5SliderModule } from 'ng5-slider';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LitigioListComponent } from './litigio-list/litigio-list.component';
import { LitigioFormComponent } from './litigio-form/litigio-form.component';
import { SharedModule } from './../../shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule,
    Ng5SliderModule
  ],
  declarations: [LitigioListComponent, LitigioFormComponent]
})
export class LitigioModule { }
