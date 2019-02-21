import { Mes } from './../../model/sindicato-laboral';
import { ParcelaReajuste } from './../../model/negociacao';
import { distinctUntilChanged, tap, filter } from 'rxjs/operators';
import { Component, OnInit, forwardRef, Input, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, AbstractControl, FormControl } from '@angular/forms';
import { formatNumber, formatDate } from '@angular/common';

export const CUSTOM_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => CustomInputComponent),
  multi: true,
};
@Component({
  selector: 'app-custom-input',
  templateUrl: './custom-input.component.html',
  styleUrls: ['./custom-input.component.css'],
  providers: [CUSTOM_VALUE_ACCESSOR]
})
export class CustomInputComponent implements OnInit, ControlValueAccessor {

  @Input() name: string;
  @Input() label: string;
  @Input() id: string;
  @Input() placeholder: string;
  @Input() type = 'text';
  @Input() control: AbstractControl;
  @Input() readOnly = false;
  @Input() addOnAfter: string = null;
  @Input() addOnBefore: string = null;
  @Input() parcelasReajuste = false;
  @Input() parcelasPopover: ParcelaReajuste[];
  @Input() formatNumber = '1.0-2';
  @Output() parcelasClick = new EventEmitter<boolean>();

  Mes: typeof Mes = Mes;
  private innerValue: any;
  private value: any;
  private innerType = 'text';
  public hasError = false;

  constructor() { }

  ngOnInit() {
    if (!this.placeholder) {
      this.placeholder = this.label;
    }

    this.innerType = this.type;
    if (this.type === 'number' || this.type === 'date') {
      this.innerType = 'text';
    }

    if (this.type === 'date') {
      this.placeholder = 'dd/mm/aaaa';
    }

    this.control.statusChanges
      .pipe(distinctUntilChanged(), filter(_ => this.control.dirty))
      .subscribe(s => this.hasError = (s === 'INVALID'));

  }

  onChange: (_: any) => void = () => { };
  onTouch: (_: any) => void = () => { };

  toNumber(valor: any): number {
    if (!valor) {
      return 0;
    }
    try {
      return parseFloat(valor.toString().replace(/\./g, '').replace(',', '.'));
    } catch {
      return 0;
    }
  }

  updateValue(valor: any) {
    if (valor !== null) {
      switch (this.type) {
        case 'number':
          this.innerValue = this.toNumber(valor);
          break;
        case 'date':
          if (valor && (<string>valor).length === 10) {
            const values: any[] = (<string>valor).split('/');
            if (values.length !== 3) {
              this.innerValue = valor;
            } else {
              this.innerValue = new Date(<number>values[2], <number>values[1] - 1, <number>values[0], 0, 0, 0);
            }
          } else {
            this.innerValue = valor;
          }
          break;
        default:
          this.innerValue = valor;
      }
      this.value = valor;
    } else {
      this.innerValue = '';
    }
  }

  pushChanges(valor: any) {
    this.updateValue(valor);
    this.onChange(this.innerValue);
  }

  showParcelasClick() {
    this.parcelasClick.emit(true);
  }

  writeValue(valor: any): void {
    if (valor !== null) {
      this.innerValue = valor;
      switch (this.type) {
        case 'number':
          this.value = !valor ? 0 : formatNumber(valor, 'pt-BR', this.formatNumber);
          break;
        case 'date':
          this.value = valor ? formatDate(<Date>valor, 'dd/MM/yyyy', 'pt-BR') : valor;
          break;
        default:
          this.value = valor;
      }
    } else {
      this.innerValue = '';
    }
  }
  registerOnChange(fn: any): void {
    this.onChange = fn;
    if (this.type === 'date') {
      setTimeout(() => {
        this.innerValue = new Date(this.innerValue);
        this.onChange(this.innerValue);
      }, 100);
    }
  }

  registerOnTouched(fn: any): void {
    this.onTouch = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    this.readOnly = isDisabled;
  }
}
