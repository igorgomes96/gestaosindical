import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';

import { Observable, of } from 'rxjs';
import { Litigio } from './../../model/litigio';
import { catchError, tap } from 'rxjs/operators';
import { LitigiosApiService } from 'src/app/shared/api/litigios-api.service';

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
