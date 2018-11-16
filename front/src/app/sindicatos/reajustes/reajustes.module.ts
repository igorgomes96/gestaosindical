import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReajustesListComponent } from './reajustes-list/reajustes-list.component';
import { ReajustesFormComponent } from './reajustes-form/reajustes-form.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [ReajustesListComponent, ReajustesFormComponent]
})
export class ReajustesModule { }
