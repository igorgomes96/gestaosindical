import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';

import { Contato } from './../../model/contato';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-contato',
  templateUrl: './contato.component.html',
  styleUrls: ['./contato.component.css']
})
export class ContatoComponent implements OnInit {

  @Input() contatos$: Observable<Contato[]>;
  @Input() referenteA: string;
  @Input() referenteAId: number;

  @Output() novoContato = new EventEmitter<Contato>();
  @Output() deleteContato = new EventEmitter<number>();

  formContato: FormGroup;

  constructor(private formBuilder: FormBuilder) {
    this.formContato = this.formBuilder.group({
      nome: ['', Validators.required],
      telefone1: ['', Validators.required],
      telefone2: [''],
      email: ['', Validators.email],
      tipoContato: ['Presidente', Validators.required]
    });
  }

  hasError(control: AbstractControl) {
    return control.dirty && control.invalid;
  }

  addContato() {
    const contato: Contato = this.formContato.value;
    this.novoContato.emit(contato);
    this.formContato.reset();
    this.formContato.get('tipoContato').setValue('Presidente');
  }

  removeContato(id: number) {
    this.deleteContato.emit(id);
  }

  ngOnInit() {
  }

}
