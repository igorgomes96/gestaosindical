import { ToastsService } from 'src/app/shared/toasts.service';
import { IntervalFilterService } from './../../../shared/interval-filter.service';
import { Component, OnInit } from '@angular/core';
import { Negociacao, StatusNegociacao } from 'src/app/model/negociacao';
import { ActivatedRoute } from '@angular/router';
import { switchMap, tap } from 'rxjs/operators';
import { Options, ChangeContext } from 'ng5-slider';
import { ToastType } from 'src/app/shared/toasts/toasts.component';
import { NegociacoesService } from '../../negociacoes.service';
import { NegociacoesApiService } from 'src/app/shared/api/negociacoes-api.service';

declare var $: any;

@Component({
  selector: 'app-agenda-list',
  templateUrl: './agenda-list.component.html',
  styleUrls: ['./agenda-list.component.css']
})
export class AgendaListComponent implements OnInit {

  negociacoes: Negociacao[];
  negociacoesFiltradas: Negociacao[];
  referent: any;
  value: number;
  options: Options;
  StatusNegociacao: typeof StatusNegociacao = StatusNegociacao;
  filterParams = (v: string) => { return {
      empresa: { nome: v },
      sindicatoLaboral: { nome: v },
      sindicatoPatronal: { nome: v }
    };
  }

  constructor(private api: NegociacoesApiService, private route: ActivatedRoute,
    private intervalService: IntervalFilterService,
    private toast: ToastsService, private negociacoesService: NegociacoesService) { }

  classStatus(status: StatusNegociacao) {
    return this.negociacoesService.classStatus(status);
  }

  ngOnInit() {
    this.intervalService.options
    .subscribe(opt => {
      this.options = opt;
    });

    this.route.queryParamMap.pipe(
      tap(d => {
        this.referent = {};
        Object.assign(this.referent, d['params']);
        if (this.referent.hasOwnProperty('ano')) {
          this.value = this.intervalService.value = this.referent['ano'];
        } else {
          this.referent['ano'] = this.value = this.intervalService.value;
        }
      }),
      switchMap(_ => this.api.getAll(this.referent))
    ).subscribe((d: Negociacao[]) => {
      this.negociacoes = d;
      this.negociacoesFiltradas = this.negociacoes;
    });

  }

  onUserChangeEnd(changeContext: ChangeContext): void {
    this.referent['ano'] = changeContext.value;
    this.intervalService.value = changeContext.value;

    this.load();
  }

  load() {
    this.api.getAll(this.referent)
    .subscribe((d: Negociacao[]) => {
      this.negociacoes = d;
      this.negociacoesFiltradas = this.negociacoes;
    });
  }

  excluir(negociacao: Negociacao) {
    this.toast.swalMessage({
      title: 'Confirma exlusão?',
      message: 'Todos os dados e arquivos relacionados a esta negociação serão excluídos!',
      type: ToastType.warning
    }, () => this.api.delete(negociacao.id).subscribe(d => {
      this.load();
      this.toast.showMessage({
        message: 'Negociação excluída com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      });
    }));
  }

  onFilter(negociacoes: Negociacao[]) {
    this.negociacoesFiltradas = negociacoes;
  }


}
