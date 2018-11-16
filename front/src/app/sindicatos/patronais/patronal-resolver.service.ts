import { HttpErrorResponse } from '@angular/common/http';
import { PatronaisApiService } from './patronais-api.service';
import { Observable, throwError, of } from 'rxjs';
import { SindicatoPatronal } from 'src/app/model/sindicato-patronal';
import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { tap, catchError } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class PatronalResolverService implements Resolve<SindicatoPatronal> {

  constructor(private patronaisApi: PatronaisApiService, private router: Router) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<SindicatoPatronal> {
    if (route.paramMap.has('id')) {
      const id: number = +route.paramMap.get('id');
      return this.patronaisApi.get(id).pipe(
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
