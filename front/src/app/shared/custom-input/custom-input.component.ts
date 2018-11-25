import { distinctUntilChanged, tap, filter } from 'rxjs/operators';
import { Component, OnInit, forwardRef, Input, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, AbstractControl } from '@angular/forms';

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
  @Output() parcelasClick = new EventEmitter<boolean>();

  private innerValue: any;
  public hasError = false;

  constructor() { }

  ngOnInit() {
    if (!this.placeholder) {
      this.placeholder = this.label;
    }

    this.control.statusChanges
    .pipe(distinctUntilChanged(), filter(_ => this.control.dirty))
    .subscribe(s => this.hasError = (s === 'INVALID'));
  }

  onChange: (_: any) => void = () => {};
  onTouch: (_: any) => void = () => {};


  get value() {
    return this.innerValue;
  }

  set value(v: any) {
    if (v !== null) {
      this.innerValue = v;
      this.onChange(v);
    } else {
      this.innerValue = '';
    }
  }

  showParcelasClick() {
    this.parcelasClick.emit(true);
  }

  writeValue(obj: any): void {
    this.value = obj;
  }
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this.onTouch = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    this.readOnly = isDisabled;
  }
}
