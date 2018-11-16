import { Component, OnInit, EventEmitter, Input } from '@angular/core';

declare var toastr: any;

export enum ToastType {
  success = 'success',
  error = 'error',
  warning = 'warning',
  info = 'info'
}

export interface ToastMessage {
  message: string;
  title: string;
  type: ToastType;
}

@Component({
  selector: 'app-toasts',
  template: '',
  styleUrls: ['./toasts.component.css']
})
export class ToastsComponent implements OnInit {

  @Input() mensagem: EventEmitter<ToastMessage>;

  constructor() {
  }

  ngOnInit() {
    this.mensagem.subscribe((m: ToastMessage) => this.showMessage(m));
  }

  showMessage(message: ToastMessage) {
    toastr.options = {
      closeButton: true,
      progressBar: true,
      timeOut: message.type === ToastType.error ? 10000 : 4000
    };
    toastr[ToastType[message.type]](message.message, message.title);
  }


}
