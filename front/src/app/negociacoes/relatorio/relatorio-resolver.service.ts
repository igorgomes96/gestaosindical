import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';

import { Observable, of } from 'rxjs';
import { Relatorio } from './../../model/relatorio';
import { catchError, tap } from 'rxjs/operators';
import { NegociacoesApiService } from 'src/app/shared/api/negociacoes-api.service';

@Injectable({
  providedIn: 'root'
})
export class RelatorioResolverService implements Resolve<Relatorio> {

  constructor(private api: NegociacoesApiService, private router: Router) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Relatorio> {
    if (route.paramMap.has('id')) {
      const id: number = +route.paramMap.get('id');
      return this.api.getRelatorio(id).pipe(
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
