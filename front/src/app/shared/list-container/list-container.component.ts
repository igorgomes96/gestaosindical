import { RelatedLink } from '../related-link/related-link';
import { queryParams } from './../../../environments/queryparams';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { UtilService } from '../util.service';
import { distinctUntilChanged, debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-list-container',
  templateUrl: './list-container.component.html',
  styleUrls: ['./list-container.component.css']
})
export class ListContainerComponent implements OnInit {

  constructor(private formBuilder: FormBuilder, private util: UtilService) { }

  queryParamsName = queryParams;
  objectKeys = Object.keys;
  formFiltro: FormGroup;


  @Input() list: any[];
  @Input() title: string;
  @Input() description: string;
  @Input() searchPlaceholder: string;
  @Input() newLink: string;
  @Input() referent: any;
  @Input() relatedLinks: RelatedLink[];

  // tslint:disable-next-line:no-output-on-prefix
  @Output() onFilter: EventEmitter<any[]> = new EventEmitter<any[]>();
  @Input() filterParams = (v: string): any => ({ nome: v });

  ngOnInit() {
    this.formFiltro = this.formBuilder.group({
      filtro: ['']
    });

    if (this.referent && this.objectKeys(this.referent).some(x => this.referent.hasOwnProperty(x))) {
      const len = this.description.length;
      if (this.description[len - 1] === '.') {
        this.description = this.description.slice(0, len - 1);
      }
      this.description = this.description + ', referentes a: ';
    }

    this.formFiltro.get('filtro').valueChanges
      .pipe(distinctUntilChanged(), debounceTime(500))
      .subscribe(v => {
        this.onFilter.emit(this.util.filtrar(this.list, this.filterParams(v)));
      });

  }

}
