import { Router, ActivatedRoute } from '@angular/router';
import { ToastsService } from 'src/app/shared/toasts.service';
import { RodadaNegociacao } from './../../../model/negociacao';
import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FormGroup} from '@angular/forms';
import { Arquivo } from 'src/app/model/arquivo';
import { switchMap, finalize } from 'rxjs/operators';
import { ToastType } from 'src/app/shared/toasts/toasts.component';
import { RodadasApiService } from 'src/app/shared/api/rodadas-api.service';
import { NegociacoesApiService } from 'src/app/shared/api/negociacoes-api.service';

@Component({
  selector: 'app-rodada',
  templateUrl: './rodada.component.html',
  styleUrls: ['./rodada.component.css']
})
export class RodadaComponent implements OnInit {

  @Input() rodada: RodadaNegociacao;
  @Input() final: boolean;
  @Output() deleteRodada: EventEmitter<boolean> = new EventEmitter<boolean>();

  arquivos: Arquivo[];
  form: FormGroup;
  data: Date;
  spinnerArquivos = false;

  constructor(private service: RodadasApiService, private toast: ToastsService,
    private router: Router, private negociacoesApi: NegociacoesApiService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.service.getArquivos(this.rodada.id)
      .subscribe(a => this.arquivos = a);
  }

  setData($event) {
    if (!$event) {
      this.rodada.data = null;
    }
    this.rodada.data = new Date(`${$event} 00:00:00`);
  }

  upload(files: FileList) {
    this.spinnerArquivos = true;
    this.service.uploadFiles(this.rodada.id, files)
    .pipe(switchMap(_ => this.service.getArquivos(this.rodada.id)),
      finalize(() => this.spinnerArquivos = false))
    .subscribe(d => this.arquivos = d);
  }

  deleteFile(event: any) {
    this.service.getArquivos(this.rodada.id).subscribe(d => this.arquivos = d);
  }

  salvar() {
    this.service.put(this.rodada.id, this.rodada)
      .subscribe(_ => {
        this.toast.showMessage({ message: 'Rodada atualizada com sucesso!', title: 'Sucesso!', type: ToastType.success });
      });
  }

  referenteValue(valor: any) {
    return valor;
  }

  navigatePlanosAcao() {
    const de = new Date(new Date(this.rodada.data).setHours(0, 0, 0, 0)).toLocaleString('en-US');
    const ate = new Date(new Date(this.rodada.data).setHours(23, 59, 59, 99)).toLocaleString('en-US');

    // this.negociacoesApi.get(this.rodada.negociacaoId)
    //  .subscribe((neg: Negociacao) => {
    this.router.navigate(['/negociacoes/planosacao'],
      {
        queryParams: {
          de: de,
          ate: ate,
          // empresaId: neg.empresaId
        }
      });
    // });
  }

  excluir() {
    this.toast.swalMessage({
      title: 'Confirma exlusão?',
      message: 'Todos os dados e arquivos relacionados a esta rodada de negociação serão excluídos!',
      type: ToastType.warning
    }, () => this.service.delete(this.rodada.id).subscribe(_ => {
      this.deleteRodada.emit(true);
      this.toast.showMessage({
        message: 'Rodada excluída com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      });
    }));
  }

  relatorioFinal() {
    this.negociacoesApi.postRelatorio(this.rodada.negociacaoId)
    .subscribe(() => {
      this.toast.showMessage({
        message: 'Relatório gerado com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      });
      this.router.navigate(['./relatorio'], {relativeTo: this.route});
    });
  }

}
