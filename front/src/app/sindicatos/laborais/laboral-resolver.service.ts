import { SindicatoLaboral } from './../../model/sindicato-laboral';
import { Observable, of } from 'rxjs';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot, Resolve } from '@angular/router';
import { LaboraisApiService } from './laborais-api.service';
import { Injectable } from '@angular/core';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class LaboralResolverService implements Resolve<SindicatoLaboral> {

  constructor(private laboraisApi: LaboraisApiService, private router: Router) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<SindicatoLaboral> {
    if (route.paramMap.has('id')) {
      const id: number = +route.paramMap.get('id');
      return this.laboraisApi.get(id).pipe(
        catchError((err) => {
          this.router.navigate(['/not-found']);
          return of(null);
        })
      );
    } else {
      this.router.navigate(['/not-found']);
    }
  }
}
