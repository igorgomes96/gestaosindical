import { Litigio, ProcedimentoLitigio, Referente } from './../../../model/litigio';
import { Component, OnInit } from '@angular/core';
import { LitigiosApiService } from '../litigios-api.service';
import { ActivatedRoute } from '@angular/router';
import { tap, switchMap } from 'rxjs/operators';
import { Options } from 'ng5-slider/options';
import { IntervalFilterService } from 'src/app/shared/interval-filter.service';
import { ChangeContext } from 'ng5-slider';
import { ToastsService } from 'src/app/shared/toasts.service';
import { ToastType } from 'src/app/shared/toasts/toasts.component';

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

  litigios: Litigio[];
  litigiosFiltrados: Litigio[];
  referent: any;
  value: number;
  options: Options;
  ProcedimentoLitigio: typeof ProcedimentoLitigio = ProcedimentoLitigio;
  Referente: typeof Referente = Referente;
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
      }),
      switchMap(_ => this.api.getAll(this.referent))
    ).subscribe(d => {
      this.litigios = d;
      this.litigiosFiltrados = d;
    });

  }

  onUserChangeEnd(changeContext: ChangeContext): void {
    this.referent['ano'] = changeContext.value;
    this.intervalService.value = changeContext.value;

    this.load();
  }

  load() {
    this.api.getAll(this.referent)
    .subscribe((d: Litigio[]) => {
      this.litigios = d;
      this.litigiosFiltrados = this.litigios;
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
