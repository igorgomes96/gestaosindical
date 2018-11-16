import { map } from 'rxjs/operators';
import { NegociacoesApiService } from './../negociacoes/negociacoes-api.service';
import { Injectable } from '@angular/core';
import { Options } from 'ng5-slider/options';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class IntervalFilterService {

  value: number = new Date().getFullYear();
  private _options: Options;

  constructor(private api: NegociacoesApiService) { }

  get options(): Observable<Options> {
    if (!this._options) {
      return this.api.getIntervalYears()
        .pipe(map(d => {
          let acc = 0;
          if (d['ceil'] - d['floor'] < 5) {
            acc = 2;
          }
          const opt: Options = {
            floor: d['floor'] - acc,
            ceil: d['ceil'] + acc,
            showTicks: true
          };
          return opt;
        }));
    }
    return of(this._options);
  }

}
