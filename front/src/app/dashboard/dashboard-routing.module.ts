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
          breadcrumb: 'Negociações'
        },
        path: 'negociacoes',
        component: DashboardNegociacoesComponent,
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
