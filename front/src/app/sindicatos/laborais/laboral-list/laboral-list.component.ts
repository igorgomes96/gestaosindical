import { ToastsService } from './../../../shared/toasts.service';
import { tap, switchMap } from 'rxjs/operators';
import { SindicatoLaboral, Mes } from './../../../model/sindicato-laboral';
import { Component, OnInit, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastType } from 'src/app/shared/toasts/toasts.component';
import { LaboraisApiService } from 'src/app/shared/api/laborais-api.service';

@Component({
  selector: 'app-laboral-list',
  templateUrl: './laboral-list.component.html',
  styleUrls: ['./laboral-list.component.css']
})
export class LaboralListComponent implements OnInit {

  sindicatosFiltrados: SindicatoLaboral[];
  Mes: typeof Mes = Mes;

  onLoad: EventEmitter<SindicatoLaboral[]> = new EventEmitter<SindicatoLaboral[]>();

  constructor(private api: LaboraisApiService, private route: ActivatedRoute, private toast: ToastsService) { }

  ngOnInit() {
    this.load();
  }

  load() {
    this.route.queryParamMap.pipe(
      switchMap(d => this.api.getAll(d))
    ).subscribe(s => {
      this.onLoad.emit(s);
    });
  }

  onFilter(sindicatos: SindicatoLaboral[]) {
    this.sindicatosFiltrados = sindicatos;
  }

  excluir(sindicato: SindicatoLaboral) {
    this.toast.swalMessage({
        title: 'Confirma exlusão?',
        message: 'Todos os dados e arquivos relacionados a este sindicato serão excluídos!',
        type: ToastType.warning
    }, () => this.api.delete(sindicato.id).subscribe(d => {
      this.load();
      this.toast.showMessage({
        message: 'Sindicato Laboral excluído com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      });
    }));
  }

}
