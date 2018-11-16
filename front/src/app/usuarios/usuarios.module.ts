import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from './../shared/shared.module';
import { UsuariosRoutingModule } from './usuarios.routing.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsuarioListComponent } from './usuario-list/usuario-list.component';
import { UsuarioFormComponent } from './usuario-form/usuario-form.component';
import { NgxMaskModule } from 'ngx-mask';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    UsuariosRoutingModule,
    SharedModule,
    NgxMaskModule.forRoot()
  ],
  declarations: [UsuarioListComponent, UsuarioFormComponent]
})
export class UsuariosModule { }
