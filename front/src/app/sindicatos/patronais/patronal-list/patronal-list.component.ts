import { ToastsService } from './../../../shared/toasts.service';
import { ToastType } from './../../../shared/toasts/toasts.component';
import { Component, OnInit, EventEmitter } from '@angular/core';

import { SindicatoPatronal } from 'src/app/model/sindicato-patronal';
import { take } from 'rxjs/operators';
import { PatronaisApiService } from 'src/app/shared/api/patronais-api.service';

declare var swal: any;

@Component({
  selector: 'app-patronal-list',
  templateUrl: './patronal-list.component.html',
  styleUrls: ['./patronal-list.component.css']
})
export class PatronalListComponent implements OnInit {

  sindicatosFiltrados: SindicatoPatronal[];

  onLoad: EventEmitter<SindicatoPatronal[]> = new EventEmitter<SindicatoPatronal[]>();

  constructor(private api: PatronaisApiService, private toastService: ToastsService) { }

  ngOnInit() {
    this.load();
  }

  load() {
    this.api.getAll()
      .pipe(take(1))
      .subscribe(d => {
        this.onLoad.emit(d);
      });
  }

  onFilter(sindicatos: SindicatoPatronal[]) {
    this.sindicatosFiltrados = sindicatos;
  }

  excluir(sindicato: SindicatoPatronal) {
    swal({
      title: 'Confirma exlusão?',
      text: 'Todos os dados e arquivos relacionados a este sindicato serão excluídos!',
      icon: 'warning',
      buttons: true,
      dangerMode: true,
    }).then((willDelete) => {
      if (willDelete) {
        this.api.delete(sindicato.id)
          .subscribe(d => {
            this.load();
            this.toastService.showMessage({
              message: 'Sindicato Patronal excluído com sucesso!',
              title: 'Sucesso!',
              type: ToastType.success
            });
          });
      }
    });
  }


}
