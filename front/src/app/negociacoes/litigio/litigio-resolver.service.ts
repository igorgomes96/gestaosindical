import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';

import { LitigiosApiService } from './litigios-api.service';
import { Observable, of } from 'rxjs';
import { Litigio } from './../../model/litigio';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class LitigioResolverService implements Resolve<Litigio> {

  constructor(private api: LitigiosApiService, private router: Router) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Litigio> {
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
