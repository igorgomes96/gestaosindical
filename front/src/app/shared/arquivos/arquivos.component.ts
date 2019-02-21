import { ToastsService } from './../toasts.service';

import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Arquivo } from './../../model/arquivo';
import { ToastType } from '../toasts/toasts.component';
import { ArquivosApiService } from '../api/arquivos-api.service';

@Component({
  selector: 'app-arquivos',
  templateUrl: './arquivos.component.html',
  styleUrls: ['./arquivos.component.css']
})
export class ArquivosComponent implements OnInit {

  @Input() arquivos: Arquivo[];
  @Output() upload = new EventEmitter<FileList>();
  @Output() delete = new EventEmitter<number>();
  @Input() dropzoneColor = 'white';
  @Input() spinner = false;

  constructor(private service: ArquivosApiService, private toast: ToastsService) { }

  ngOnInit() {
  }

  download(arquivo: Arquivo) {
    this.service.download(arquivo.id).subscribe(res => {
      const a = document.createElement('a');
      const binaryData = [];
      binaryData.push(res);
      a.href = window.URL.createObjectURL(new Blob(binaryData, { type: arquivo.contentType }));
      document.body.appendChild(a);
      a.setAttribute('style', 'display: none');
      a.download = arquivo.nome;
      a.click();
      window.URL.revokeObjectURL(a.href);
      a.remove(); // remove the element

    });

  }

  deleteFile(id: string) {
    this.toast.swalMessage({
      title: 'Confirma exlusão?',
      message: 'Essa ação não poderá ser desfeita!',
      type: ToastType.warning
    }, () => this.service.delete(id).subscribe(_ => {
      this.delete.emit();
      this.toast.showMessage({
        message: 'Arquivo excluído com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      });
    }));
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.upload.emit(event.dataTransfer.files);
  }

  onInputChange(event: any) {
    this.upload.emit(event.target.files);
  }

  onDragOver(event) {
    event.stopPropagation();
    event.preventDefault();
  }

}
