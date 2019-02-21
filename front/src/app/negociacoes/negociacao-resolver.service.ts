import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';

import { Negociacao } from 'src/app/model/negociacao';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { NegociacoesApiService } from '../shared/api/negociacoes-api.service';

@Injectable({
  providedIn: 'root'
})
export class NegociacaoResolverService implements Resolve<Negociacao> {

  constructor(private api: NegociacoesApiService, private router: Router) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Negociacao> { 
    if (route.paramMap.has('id')) {
      const id: number = +route.paramMap.get('id');
      return this.api.get(id).pipe(
        catchError(_ => {
          this.router.navigate(['/not-found']);
          return of(null);
        })
      );
    } else {
      this.router.navigate(['/not-found']);
    }
  }

}
