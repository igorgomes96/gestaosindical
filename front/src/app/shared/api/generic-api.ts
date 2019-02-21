import { take } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export class GenericApi<T> {

    constructor(private httpClient: HttpClient, protected url: string) { }

    getAll(params: any = {}): Observable<T[]> {
        return this.httpClient.get<T[]>(this.url, {params: this.validParams(params)}).pipe(take(1));
    }

    get(id: number | string): Observable<T> {
        return this.httpClient.get<T>(`${this.url}${id}`).pipe(take(1));
    }

    post(data: T): Observable<T> {
        return this.httpClient.post<T>(`${this.url}`, data).pipe(take(1));
    }

    put(id: number | string, data: T): Observable<void | T> {
        return this.httpClient.put<void | T>(`${this.url}${id}`, data).pipe(take(1));
    }

    delete(id: number | string): Observable<T> {
        return this.httpClient.delete<T>(`${this.url}${id}`).pipe(take(1));
    }

    validParams(params: any = {}): any {
        const newParams: any = {};
        for (const param in params) {
            if (params[param] !== undefined && params[param] !== undefined) {
                newParams[param] = params[param];
            }
        }
        return newParams;
    }
}
