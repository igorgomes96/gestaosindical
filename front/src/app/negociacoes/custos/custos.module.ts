import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustosListComponent } from './custos-list/custos-list.component';
import { CustosFormComponent } from './custos-form/custos-form.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [CustosListComponent, CustosFormComponent]
})
export class CustosModule { }
