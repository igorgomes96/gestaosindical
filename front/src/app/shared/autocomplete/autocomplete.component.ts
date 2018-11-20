import { tap } from 'rxjs/operators';
import { Component, OnInit, Input, forwardRef, TemplateRef, ContentChild, ViewChild, ElementRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';


export const AUTOCOMPLETE_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => AutocompleteComponent),
  multi: true,
};
@Component({
  selector: 'app-autocomplete',
  templateUrl: './autocomplete.component.html',
  styleUrls: ['./autocomplete.component.css'],
  providers: [AUTOCOMPLETE_VALUE_ACCESSOR]
})
export class AutocompleteComponent implements OnInit, ControlValueAccessor {

  list: any[];
  value = '';  // Valor exibido no input
  objectValue = null;   // Valor original
  _mostraLista = false;
  activeItem = -1;
  enabledLink = false;

  @Input() urlSource: string;
  @Input() urlQueryParam = 'filter';
  @Input() readOnly = false;
  @Input() placeholder = '';
  @Input() linkFunction = null;

  @ContentChild(TemplateRef) templateItem: TemplateRef<any>;

  @ViewChild('listRef') listRef: ElementRef;
  @ViewChild('inputRef') inputRef: ElementRef;

  onChange = (_: any) => { };
  onTouched = () => { };

  @Input() mapValueFunction = function (obj: any) { return obj; };
  @Input() mapShowFunction = function (obj: any) {
    if (!obj || !obj.hasOwnProperty('nome')) {
      return null;
    }
    return obj['nome'];
  };
  @Input() classFunction = (_: any) => '';

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit() {
  }

  set mostraLista(value: boolean) {
    this._mostraLista = value;
  }

  get mostraLista() {
    return !this.readOnly && this._mostraLista;
  }

  navigate() {
    if (this.linkFunction == null) {
      return;
    }
    this.router.navigate([this.linkFunction(this.mapValueFunction(this.objectValue))]);
  }

  selectItem(i: number) {
    this.activeItem = i;
    if (this.activeItem >= 0 && this.activeItem < this.list.length) {
      this.updateValue(this.list[this.activeItem]);
    }
  }

  keypress($event: KeyboardEvent) {
    if ($event.key === 'Enter') {
      this.selectItem(this.activeItem);
      $event.preventDefault();
    }
  }

  keydown($event: KeyboardEvent) {
    switch ($event.key) {
      case 'ArrowDown':
        this.activeItem++;
        this.updateActiveItem();
        $event.preventDefault();
        break;
      case 'ArrowUp':
        this.activeItem--;
        this.updateActiveItem();
        $event.preventDefault();
        break;
    }
  }

  updateActiveItem(i = null) {
    if (i !== null) {
      this.activeItem = i;
    }
    if (this.activeItem >= this.list.length) {
      this.activeItem = 0;
    } else if (this.activeItem < 0) {
      this.activeItem = this.list.length - 1;
    }
  }

  pushChanges(value: any) {
    this.enabledLink = false;
    this.onChange(null);
    if (!value) {
      this.list = [];
      return;
    }
    const params = {};
    params[this.urlQueryParam] = value;
    this.http.get(this.urlSource, { params: params }).pipe(tap(_ => this.mostraLista = true))
      .subscribe((v: any[]) => this.list = v);
  }

  updateValue(value) {
    const v = this.mapValueFunction(value);
    this.onChange(v);
    if (v) {
      this.enabledLink = true;
    }
    this.objectValue = value;
    this.value = this.mapShowFunction(value);
    this.mostraLista = false;
  }

  writeValue(obj: any): void {
    this.updateValue(obj);
  }

  registerOnChange(fn: (_: any) => {}): void {
    this.onChange = fn;
    if (this.objectValue) {
      this.updateValue(this.objectValue);
    }
  }
  registerOnTouched(fn: () => {}): void { this.onTouched = fn; }
  setDisabledState?(isDisabled: boolean): void {
    this.readOnly = isDisabled;
  }

}
