import { ToastsService } from './../../shared/toasts.service';
import { take } from 'rxjs/operators';
import { Component, OnInit, EventEmitter } from '@angular/core';
import { Empresa } from 'src/app/model/empresa';
import { ToastType } from 'src/app/shared/toasts/toasts.component';
import { EmpresasApiService } from 'src/app/shared/api/empresas-api.service';
import { Mes } from 'src/app/model/sindicato-laboral';

@Component({
  selector: 'app-empresa-list',
  templateUrl: './empresa-list.component.html',
  styleUrls: ['./empresa-list.component.css']
})
export class EmpresaListComponent implements OnInit {

  empresasFiltradas: Empresa[];
  onLoad: EventEmitter<Empresa[]> = new EventEmitter<Empresa[]>();
  Mes: typeof Mes = Mes;
  filterParams = (v: string) => {
    return {
      empresa: { nome: v },
      sindicatoLaboral: { nome: v },
      sindicatoPatronal: { nome: v }
    };
  }

  constructor(private api: EmpresasApiService, private toast: ToastsService) { }

  ngOnInit() {
    this.load();
  }

  load() {
    this.api.getAll().pipe(take(1))
      .subscribe(e => {
        this.onLoad.emit(e);
      });
  }

  onFilter(empresas: Empresa[]) {
    this.empresasFiltradas = empresas;
  }

  excluir(empresa: Empresa) {
    this.toast.swalMessage({
      title: 'Confirma exlusão?',
      message: 'Todos os dados e arquivos relacionados a esta empresa serão excluídos!',
      type: ToastType.warning
    }, () => this.api.delete(empresa.id).subscribe(d => {
      this.load();
      this.toast.showMessage({
        message: 'Empresa excluída com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      });
    }));
  }
}
