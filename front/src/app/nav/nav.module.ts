import { SharedModule } from './../shared/shared.module';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FooterComponent } from './footer/footer.component';
import { NavigationComponent } from './navigation/navigation.component';
import { TopnavComponent } from './topnav/topnav.component';
import { BreadcrumpComponent } from './breadcrump/breadcrump.component';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    SharedModule
  ],
  exports: [
    NavigationComponent,
    TopnavComponent,
    FooterComponent
  ],
  declarations: [NavigationComponent, TopnavComponent, FooterComponent, NavigationComponent, BreadcrumpComponent]
})
export class NavModule { }
