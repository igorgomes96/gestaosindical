import { FormGroup } from '@angular/forms';
import { Referente } from './../../../model/litigio';
import { Component, OnInit } from '@angular/core';

import { PlanoAcao, StatusPlanoAcao } from './../../../model/plano-acao';
import { PlanoAcaoApiService } from './../plano-acao-api.service';
import { Options } from 'ng5-slider/options';
import { IntervalFilterService } from 'src/app/shared/interval-filter.service';
import { ActivatedRoute } from '@angular/router';
import { tap, switchMap } from 'rxjs/operators';
import { ChangeContext } from 'ng5-slider';
import { ToastsService } from 'src/app/shared/toasts.service';
import { ToastType } from 'src/app/shared/toasts/toasts.component';

declare var $: any;

@Component({
  selector: 'app-plano-acao-list',
  templateUrl: './plano-acao-list.component.html',
  styleUrls: ['./plano-acao-list.component.css']
})
export class PlanoAcaoListComponent implements OnInit {

  constructor(private api: PlanoAcaoApiService, private route: ActivatedRoute,
    private intervalService: IntervalFilterService,
    private toast: ToastsService) { }

  planosAcao: PlanoAcao[];
  planosAcaoFiltrados: PlanoAcao[];
  referent: any;
  value: number;
  options: Options;
  StatusPlanoAcao: typeof StatusPlanoAcao = StatusPlanoAcao;
  Referente: typeof Referente = Referente;
  formRangeDate: FormGroup;

  filterParams = (v: string) => ({ laboral: { nome: v }, patronal: { nome: v }, referente: v });

  ngOnInit() {


    this.intervalService.options
      .subscribe(opt => this.options = opt);

    this.route.queryParamMap.pipe(
      tap(d => {
        this.referent = {};
        Object.assign(this.referent, d['params']);
        if (this.referent.hasOwnProperty('ano')) {
          this.value = this.intervalService.value = this.referent['ano'];
        } else {
          this.referent['ano'] = this.value = this.intervalService.value;
        }
        this.updateDateRange();
      }),
      switchMap(_ => this.api.getAll(this.referent))
    ).subscribe(d => {
      this.planosAcao = d;
      this.planosAcaoFiltrados = d;
    });
  }

  classStatus(status: StatusPlanoAcao) {
    switch (StatusPlanoAcao[status]) {
      case StatusPlanoAcao.Vencido:
        return 'label-danger';
      case StatusPlanoAcao.AVencer:
        return 'label-warning';
      case StatusPlanoAcao.Solucionado:
        return 'label-primary';
      case StatusPlanoAcao.NoPrazo:
        return 'label-success';
    }
  }

  datestring(d: Date) {
    return ('0' + d.getDate()).slice(-2) + '/' + ('0' + (d.getMonth() + 1)).slice(-2) + '/' +
      d.getFullYear();
  }

  updateDateRange(options: any = null) {
    const startDate = new Date(this.referent ? this.referent['ano'] : new Date().getFullYear(), 0, 1);
    const endDate = new Date(this.referent ? this.referent['ano'] : new Date().getFullYear(), 11, 31);

    const opt = {
      'locale': {
        'format': 'DD/MM/YYYY',
        'separator': ' - ',
        'applyLabel': 'Aplicar',
        'cancelLabel': 'Cancelar',
        'fromLabel': 'De',
        'toLabel': 'Até',
        'customRangeLabel': 'Custom',
        'weekLabel': 'S',
        'daysOfWeek': [
          'Dom',
          'Seg',
          'Ter',
          'Qua',
          'Qui',
          'Sex',
          'Sab'
        ],
        'monthNames': [
          'Janeiro',
          'Fevereiro',
          'Março',
          'Abril',
          'Maio',
          'Junho',
          'Julho',
          'Agosto',
          'Setembro',
          'Outrubro',
          'Novembro',
          'Dezembro'
        ],
        'firstDay': 1
      },
      'showDropdowns': true,
      'minYear': 2018,
      'maxYear': 2018,
      'startDate': startDate,
      'endDate': endDate,
      'minDate': this.datestring(startDate),
      'maxDate': this.datestring(endDate),
      'buttonClasses': 'btn btn-success'
    };
    if (options) {
      Object.assign(opt, options);
    }

    $('#dataRange').daterangepicker(opt, function (start, end, label) {
      // tslint:disable-next-line:max-line-length
      console.log(start.toDate(), end, label);
    });

  }

  onUserChangeEnd(changeContext: ChangeContext): void {
    this.referent['ano'] = changeContext.value;
    this.updateDateRange();
    this.intervalService.value = changeContext.value;
    this.load();
  }

  load() {
    this.api.getAll(this.referent)
      .subscribe((d: PlanoAcao[]) => {
        this.planosAcao = d;
        this.planosAcaoFiltrados = this.planosAcao;
      });
  }

  onFilter(planosAcao: PlanoAcao[]) {
    this.planosAcaoFiltrados = planosAcao;
  }

  excluir(plano: PlanoAcao) {
    this.toast.swalMessage({
      title: 'Confirma exlusão?',
      message: 'Todos os dados e arquivos relacionados a este plano de ação serão excluídos!',
      type: ToastType.warning
    }, () => this.api.delete(plano.id).subscribe(d => {
      this.load();
      this.toast.showMessage({
        message: 'Plano de Ação excluído com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      });
    }));
  }

}
