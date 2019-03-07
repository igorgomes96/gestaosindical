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
  filteredList: any[];

  pageIndex = 1;
  pageSize = 25;
  nPages = 0;
  pages = [];
  list: any[];

  @Input() title: string;
  @Input() description: string;
  @Input() searchPlaceholder: string;
  @Input() newLink: string;
  @Input() referent: any;
  @Input() relatedLinks: RelatedLink[];
  @Input() onLoad: EventEmitter<any[]>;

  // tslint:disable-next-line: no-output-on-prefix
  @Output() onFilter: EventEmitter<any[]> = new EventEmitter<any[]>();
  @Input() filterParams = (v: string): any => ({ nome: v });

  ngOnInit() {

    if (this.onLoad) {
      this.onLoad.subscribe((list: any[]) => {
        this.list = this.filteredList = list;
        this.updateNPages();
        this.onFilter.emit(this.slice());
      });
    }

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
        this.filteredList = this.util.filtrar(this.list, this.filterParams(v));
        this.updateNPages();
        this.onFilter.emit(this.slice());
      });

  }

  slice(): any[] {
    const start = (this.pageIndex - 1) * this.pageSize;
    return this.filteredList.slice(start, start + this.pageSize);
  }

  updateNPages() {
    this.nPages = Math.ceil(this.filteredList.length / this.pageSize);
    if (this.nPages <= 0) {
      this.pageIndex = 1;
      this.pages = [];
    } else {
      this.pages = new Array(this.nPages);
      for (let i = 1; i <= this.nPages; i++) {
        this.pages[i - 1] = i;
      }
      if (this.pageIndex > this.nPages) {
        this.pageIndex = this.nPages;
      }
    }
  }

  loadPage(pageIndex: number) {
    this.pageIndex = pageIndex;
    this.onFilter.emit(this.slice());
  }

  nextPage() {
    if (this.pageIndex < this.nPages) {
      this.loadPage(this.pageIndex + 1);
    }
  }

  previousPage() {
    if (this.pageIndex > 1) {
      this.loadPage(this.pageIndex - 1);
    }
  }

}
