import { Router } from '@angular/router';
import { ToastsService } from './../shared/toasts.service';
import { filter, tap, switchMap } from 'rxjs/operators';
import { AuthService } from './../login/auth.service';
import { Injectable, NgModule } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, empty, of } from 'rxjs';
import { ToastType } from '../shared/toasts/toasts.component';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private toast: ToastsService, private router: Router) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request)
      .pipe(
        tap(_ => {}, (event: HttpEvent<any>) => {
          if (event instanceof HttpErrorResponse) {
            if (event.status === 401) {
              this.router.navigate(['login']);
              return Observable.create(empty);
            } else if (event.status === 403) {
              this.router.navigate(['forbidden']);
              return Observable.create(empty);
            }
            this.toast.showMessage({
              message: event.error || event.message,
              title: 'Erro ao processar requisição!',
              type: ToastType.error
            });
            return Observable.create(empty);
          }
          return of(event);
        })
      );
  }
}

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.authService.authInfo) {
      return next.handle(req);
    }

    const dupReq = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${this.authService.authInfo.accessToken}`),
    });
    return next.handle(dupReq);
  }

}

@NgModule({
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true,
    }
  ]
})
export class InterceptorModule { }
