import { ToastMessage, ToastType } from './toasts/toasts.component';
import { Injectable, EventEmitter } from '@angular/core';

declare var swal: any;

@Injectable({
  providedIn: 'root'
})
export class ToastsService {

  private _messageEmitter: EventEmitter<ToastMessage> = new EventEmitter<ToastMessage>();
  constructor() { }

  get messageEmitter() {
    return this._messageEmitter;
  }

  showMessage(message: ToastMessage) {
    this._messageEmitter.emit(message);
  }

  swalMessage(message: ToastMessage, handle: Function = null, handleError: Function = null) {
    swal({
      title: message.title,
      text: message.message,
      icon: ToastType[message.type],
      buttons: true,
      dangerMode: ToastType[message.type] === ToastType.warning,
    }).then((ok) => {
      if (ok) {
        handle();
      } else if (handleError !== null) {
        handleError();
      }
    });
  }

}
