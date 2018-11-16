import { ArquivosApiService } from './arquivos-api.service';
import { Observable } from 'rxjs';

import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Arquivo } from './../../model/arquivo';

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

  constructor(private service: ArquivosApiService) { }

  ngOnInit() {
  }

  download(arquivo: Arquivo) {
    this.service.download(arquivo.id).subscribe(res => {
      var a = document.createElement('a');
      var binaryData = [];
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
    this.service.delete(id).subscribe(_ => this.delete.emit());
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
