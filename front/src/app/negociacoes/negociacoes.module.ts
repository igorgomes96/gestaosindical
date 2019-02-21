import { ReactiveFormsModule, FormsModule } from '@angular/forms';
// import { PlanoAcaoModule } from './plano-acao/plano-acao.module';
import { LitigioModule } from './litigio/litigio.module';
import { CustosModule } from './custos/custos.module';
import { AgendaModule } from './agenda/agenda.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NegociacoesRoutingModule } from './negociacoes-routing.module';
import { ConcorrenteComponent } from './concorrente/concorrente.component';
import { SharedModule } from '../shared/shared.module';
import { CalendarComponent } from './calendar/calendar.component';
import { RelatorioComponent } from './relatorio/relatorio.component';

@NgModule({
  imports: [
    CommonModule,
    AgendaModule,
    CustosModule,
    LitigioModule,
    NegociacoesRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule
  ],
  declarations: [ConcorrenteComponent, CalendarComponent, RelatorioComponent]
})
export class NegociacoesModule { }
