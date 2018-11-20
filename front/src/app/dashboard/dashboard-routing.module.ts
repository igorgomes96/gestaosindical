import { DashboardEmpresasComponent } from './dashboard-empresas/dashboard-empresas.component';
import { DashboardNegociacoesComponent } from './dashboard-negociacoes/dashboard-negociacoes.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Dashboard'
    },
    children: [
      {
        data: {
          breadcrumb: 'Anual'
        },
        path: 'anual',
        component: DashboardNegociacoesComponent,
      },
      {
        data: {
          breadcrumb: 'Por Empresa'
        },
        path: 'empresa',
        component: DashboardEmpresasComponent,
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
