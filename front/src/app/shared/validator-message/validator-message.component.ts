import { Component, OnInit, Input } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Component({
  selector: 'app-validator-message',
  templateUrl: './validator-message.component.html',
  styleUrls: ['./validator-message.component.css']
})
export class ValidatorMessageComponent implements OnInit {

  @Input() controlName: string;
  @Input() control: AbstractControl;

  constructor() { }

  ngOnInit() {

  }

  get mensagem(): string {
    if (this.control.pristine) {
      return null;
    }
    for (const property in this.control.errors) {
      if (this.control.errors.hasOwnProperty(property)) {
        switch (property) {
          case 'required':
            return `${this.controlName} é obrigatório!`;
          case 'minlength':
            return `${this.controlName} deve ter no mínimo ${this.control.errors[property].requiredLength} caracteres!`;
          case 'maxlength':
            return `${this.controlName} deve ter no máximo ${this.control.errors[property].requiredLength} caracteres!`;
          case 'email':
            return 'Email inválido!';
          case 'equalsTo':
            return 'Os valores não correspondem!';
          default:
            return null;
        }
      }
    }
    return null;
  }

}
