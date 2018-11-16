import { ToastsService } from 'src/app/shared/toasts.service';
import { RodadasApiService } from './rodadas-api.service';
import { RodadaNegociacao } from './../../../model/negociacao';
import { Component, OnInit, Input, ChangeDetectorRef, AfterViewChecked, EventEmitter, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Arquivo } from 'src/app/model/arquivo';
import { switchMap } from 'rxjs/operators';
import { ToastType } from 'src/app/shared/toasts/toasts.component';

@Component({
  selector: 'app-rodada',
  templateUrl: './rodada.component.html',
  styleUrls: ['./rodada.component.css']
})
export class RodadaComponent implements OnInit {

  @Input() rodada: RodadaNegociacao;
  @Output() deleteRodada: EventEmitter<boolean> = new EventEmitter<boolean>();

  arquivos: Arquivo[];
  form: FormGroup;
  data: Date;

  constructor(private service: RodadasApiService, private toast: ToastsService) { }

  ngOnInit() {
    this.service.getArquivos(this.rodada.id)
    .subscribe(a => this.arquivos = a);
  }

  upload(files: FileList) {
    this.service.uploadFiles(this.rodada.id, files)
    .pipe(switchMap(_ => this.service.getArquivos(this.rodada.id)))
    .subscribe(d => this.arquivos = d);
  }

  deleteFile(event: any) {
    this.service.getArquivos(this.rodada.id).subscribe(d => this.arquivos = d);
  }

  salvar() {
    this.service.put(this.rodada.id, this.rodada)
    .subscribe(_ => this.toast.showMessage({ message: 'Rodada atualizada com sucesso!', title: 'Sucesso!', type: ToastType.success }));
  }

  excluir() {
    this.service.delete(this.rodada.id)
    .subscribe(_ => {
      this.toast.showMessage({ message: 'Rodada excluída com sucesso!', title: 'Sucesso!', type: ToastType.success });
      this.deleteRodada.emit(true);
    });
  }

}
