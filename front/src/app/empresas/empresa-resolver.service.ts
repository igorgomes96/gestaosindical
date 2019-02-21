import { Empresa } from './../model/empresa';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { EmpresasApiService } from '../shared/api/empresas-api.service';

@Injectable({
  providedIn: 'root'
})
export class EmpresaResolverService implements Resolve<Empresa> {

  constructor(private empresasApi: EmpresasApiService, private router: Router) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Empresa> {
    if (route.paramMap.has('id')) {
      const id: number = +route.paramMap.get('id');
      return this.empresasApi.get(id).pipe(
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
