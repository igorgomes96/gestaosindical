import { Injectable } from '@angular/core';

export enum MatchType {
  MatchSome,
  MatchAll
}

@Injectable({
  providedIn: 'root'
})
export class UtilService {

  constructor() { }

  private containsAll = (v1, v2) => {
    if (typeof v2 === 'object') {
      return Object.keys(v2).every(prop => {
        if (v1.hasOwnProperty(prop)) {
          return this.containsAny(v1[prop], v2[prop]);
        } else {
          return false;
        }
      });
    } else if ((typeof v2 === 'string') && (typeof v1 === 'string')) {
      const result = v1.toLowerCase().indexOf(v2.toLowerCase()) >= 0;
      return result;
    } else {
      return v1 === v2;
    }
  }

  private containsAny = (v1, v2) => {
    if (typeof v2 === 'object') {
      return Object.keys(v2).some(prop => {
        if (v1.hasOwnProperty(prop)) {
          const result = this.containsAny(v1[prop], v2[prop]);
          return result;
        } else {
          return false;
        }
      });
    } else if ((typeof v2 === 'string') && (typeof v1 === 'string')) {
      const result = v1.toLowerCase().indexOf(v2.toLowerCase()) >= 0;
      return result;
    } else {
      return v1 === v2;
    }
  }

  filtrar(lista: any[], filtro: any, matchType: MatchType = MatchType.MatchSome) {
    let containsFunc = this.containsAny;
    switch (matchType) {
      case MatchType.MatchAll:
        containsFunc = this.containsAll;
        break;
      case MatchType.MatchSome:
        containsFunc = this.containsAny;
        break;
    }
    return lista.filter(v => {
      return containsFunc(v, filtro);
    });
  }
}
