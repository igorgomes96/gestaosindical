import { StatusPlanoAcao } from './../../../model/plano-acao';
import { Litigio, ProcedimentoLitigio, Referente } from './../../../model/litigio';
import { Component, OnInit, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { tap, switchMap } from 'rxjs/operators';
import { Options } from 'ng5-slider/options';
import { IntervalFilterService } from 'src/app/shared/interval-filter.service';
import { ChangeContext } from 'ng5-slider';
import { ToastsService } from 'src/app/shared/toasts.service';
import { ToastType } from 'src/app/shared/toasts/toasts.component';
import { LitigiosApiService } from 'src/app/shared/api/litigios-api.service';

declare var $: any;

@Component({
  selector: 'app-litigio-list',
  templateUrl: './litigio-list.component.html',
  styleUrls: ['./litigio-list.component.css']
})
export class LitigioListComponent implements OnInit {

  constructor(private api: LitigiosApiService, private route: ActivatedRoute,
    private intervalService: IntervalFilterService,
    private toast: ToastsService) { }

  litigiosFiltrados: Litigio[];
  referent: any;
  value: number;
  options: Options;
  ProcedimentoLitigio: typeof ProcedimentoLitigio = ProcedimentoLitigio;
  StatusPlanoAcao: typeof StatusPlanoAcao = StatusPlanoAcao;
  Referente: typeof Referente = Referente;
  onLoad: EventEmitter<Litigio[]> = new EventEmitter<Litigio[]>();
  filterParams = (v: string) => ({ empresa: { nome: v } });

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
      this.onLoad.emit(d);
    });

  }

  classStatus(status: StatusPlanoAcao) {
    if (!status) { return ''; }
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
    return '';
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

    this.updateReferent(startDate, endDate);

    const self = this;
    $('#dataRange').daterangepicker(opt, function (start, end, _) {
      // tslint:disable-next-line:max-line-length
      self.updateReferent(start.toDate(), end.toDate());
    });

  }

  updateReferent(start, end) {
    this.referent['de'] = new Date(new Date(start).setHours(0, 0, 0, 0)).toLocaleString('en-US');
    this.referent['ate'] = new Date(new Date(end).setHours(23, 59, 59, 99)).toLocaleString('en-US');
    this.api.getAll(this.referent).subscribe(d => {
      this.onLoad.emit(d);
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
    .subscribe((d: Litigio[]) => {
      this.onLoad.emit(d);
    });
  }

  onFilter(litigios: Litigio[]) {
    this.litigiosFiltrados = litigios;
  }

  excluir(litigio: Litigio) {
    this.toast.swalMessage({
      title: 'Confirma exlusão?',
      message: 'Todos os dados e arquivos relacionados a este Litígio serão excluídos!',
      type: ToastType.warning
    }, () => this.api.delete(litigio.id).subscribe(d => {
      this.load();
      this.toast.showMessage({
        message: 'Litígio excluído com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      });
    }));
  }

}
