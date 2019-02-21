import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

// import { PlanoAcaoFormComponent } from './plano-acao/plano-acao-form/plano-acao-form.component';
// import { PlanoAcaoListComponent } from './plano-acao/plano-acao-list/plano-acao-list.component';
import { LitigioFormComponent } from './litigio/litigio-form/litigio-form.component';
import { LitigioListComponent } from './litigio/litigio-list/litigio-list.component';
import { AgendaFormComponent } from './agenda/agenda-form/agenda-form.component';
import { AgendaListComponent } from './agenda/agenda-list/agenda-list.component';
import { LitigioResolverService } from './litigio/litigio-resolver.service';
import { NegociacaoResolverService } from './negociacao-resolver.service';
// import { PlanoAcaoResolverService } from './plano-acao/plano-acao-resolver.service';
import { CalendarComponent } from './calendar/calendar.component';
import { RelatorioComponent } from './relatorio/relatorio.component';

const routes: Routes = [
  {
    path: 'gestao',
    data: {
      breadcrumb: 'Gestão',
      title: 'Gestão'
    },
    children: [
      {
        path: '',
        component: AgendaListComponent,
      },
      {
        path: 'nova',
        component: AgendaFormComponent,
      },
      {
        path: ':id',
        component: AgendaFormComponent,
        resolve: {
          negociacao: NegociacaoResolverService
        }
      },
      {
        path: ':id/relatorio',
        component: RelatorioComponent,
        resolve: {
          negociacao: NegociacaoResolverService
        },
        data: {
          breadcrumb: 'Relatório',
          title: 'Relatório'
        }
      }
    ]
  },
  {
    path: 'agenda',
    component: CalendarComponent,
    data: {
      breadcrumb: 'Agenda',
      title: 'Agenda'
    },
  },
  {
    path: 'litigios',
    data: {
      breadcrumb: 'Litígios',
      title: 'Litígios'
    },
    children: [
      {
        path: '',
        component: LitigioListComponent,
      },
      {
        path: 'novo',
        component: LitigioFormComponent,
      },
      {
        path: ':id',
        component: LitigioFormComponent,
        resolve: {
          litigio: LitigioResolverService
        }
      }
    ]
  }/*,
  {
    path: 'planosacao',
    data: {
      breadcrumb: 'Planos de Ação',
      title: 'Planos de Ação'
    },
    children: [
      {
        path: '',
        component: PlanoAcaoListComponent,
      },
      {
        path: 'novo',
        component: PlanoAcaoFormComponent,
      },
      {
        path: ':id',
        component: PlanoAcaoFormComponent,
        resolve: {
          planoAcao: PlanoAcaoResolverService
        }
      }
    ]
  }*/
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class NegociacoesRoutingModule { }
