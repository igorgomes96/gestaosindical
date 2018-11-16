import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';

import { UsuarioResolverService } from './usuario-resolver.service';
import { UsuarioFormComponent } from './usuario-form/usuario-form.component';
import { UsuarioListComponent } from './usuario-list/usuario-list.component';

const routes: Routes = [
  {
    path: '',
    data: {
      breadcrumb: 'Usuários',
      title: 'Usuários'
    },
    children: [
      {
        path: '',
        component: UsuarioListComponent
      },
      {
        path: 'novo',
        component: UsuarioFormComponent,
      },
      {
        path: ':id',
        component: UsuarioFormComponent,
        resolve: {
          usuario: UsuarioResolverService
        }
      }
    ]
  }
];

@NgModule({
  imports: [CommonModule, RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsuariosRoutingModule { }

