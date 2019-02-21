import { Arquivo } from 'src/app/model/arquivo';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ItemLitigio } from 'src/app/model/litigio';
import { switchMap, finalize } from 'rxjs/operators';
import { StatusPlanoAcao } from 'src/app/model/plano-acao';
import { PlanoAcaoApiService } from 'src/app/shared/api/plano-acao-api.service';
import { ItensLitigiosApiService } from 'src/app/shared/api/itens-litigios-api.service';

@Component({
  selector: 'app-litigio-item',
  templateUrl: './litigio-item.component.html',
  styleUrls: ['./litigio-item.component.css']
})
export class LitigioItemComponent implements OnInit {

  @Input() item: ItemLitigio;
  @Input() index = 0;
  @Output() save = new EventEmitter<ItemLitigio>();
  @Output() delete = new EventEmitter<ItemLitigio>();
  arquivos: Arquivo[];
  arquivosPlano: Arquivo[];
  StatusPlanoAcao: typeof StatusPlanoAcao = StatusPlanoAcao;
  spinnerArquivos = false;
  spinnerArquivosPlano = false;


  constructor(private api: ItensLitigiosApiService,
    private planoApi: PlanoAcaoApiService) { }

  ngOnInit() {
    this.item.planoAcao.dataPrevista = new Date(this.item.planoAcao.dataPrevista);
    this.api.getArquivos(this.item.id)
      .subscribe(value => {
        this.arquivos = value;
      });

    if (this.item.planoAcao) {
      this.planoApi.getArquivos(this.item.planoAcao.id)
        .subscribe(value => {
          this.arquivosPlano = value;
        });
    }
  }



  upload(files: FileList) {
    this.spinnerArquivos = true;
    this.api.uploadFiles(this.item.id, files)
      .pipe(switchMap(_ => this.api.getArquivos(this.item.id)),
        finalize(() => this.spinnerArquivos = false))
      .subscribe(d => this.arquivos = d);
  }

  deleteFile(event: any) {
    this.api.getArquivos(this.item.id).subscribe(d => this.arquivos = d);
  }

  uploadPlano(files: FileList) {
    this.spinnerArquivosPlano = true;
    this.planoApi.uploadFiles(this.item.planoAcao.id, files)
      .pipe(switchMap(_ => this.planoApi.getArquivos(this.item.planoAcao.id)),
        finalize(() => this.spinnerArquivosPlano = false))
      .subscribe(d => this.arquivosPlano = d);
  }

  deleteFilePlano(event: any) {
    this.planoApi.getArquivos(this.item.planoAcao.id).subscribe(d => this.arquivosPlano = d);
  }

  salvar() {
    this.save.emit(this.item);
  }

  excluir() {
    this.delete.emit(this.item);
  }

  setDataPlanoAcao($event: any, property: any) {
    if (!$event) {
      this.item.planoAcao[property] = null;
    }
    this.item.planoAcao[property] = new Date(`${$event} 00:00:00`);
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
}
