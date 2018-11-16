import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { PlanoAcao } from './../../model/plano-acao';
import { catchError } from 'rxjs/operators';
import { PlanoAcaoApiService } from './plano-acao-api.service';

@Injectable({
  providedIn: 'root'
})
export class PlanoAcaoResolverService {

  constructor(private api: PlanoAcaoApiService, private router: Router) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<PlanoAcao> {
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
