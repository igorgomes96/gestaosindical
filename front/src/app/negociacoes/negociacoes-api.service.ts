import { take } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient, HttpEvent } from '@angular/common/http';

import { Negociacao, RodadaNegociacao, Concorrente } from './../model/negociacao';
import { GenericApi } from './../shared/generic-api';
import { environment } from 'src/environments/environment';
import { ArquivosApiService } from '../shared/arquivos/arquivos-api.service';
import { endpoints } from '../../environments/endpoints';
import { Arquivo } from '../model/arquivo';

@Injectable({
  providedIn: 'root'
})
export class NegociacoesApiService extends GenericApi<Negociacao> {

  constructor(private http: HttpClient, private arquivosApi: ArquivosApiService) {
    super(http, environment.api + endpoints.negociacoes);
  }

  getRodadas(id: number): Observable<RodadaNegociacao[]> {
    return this.http.get<RodadaNegociacao[]>(`${this.url}${id}/rodadas`).pipe(take(1));
  }

  postRodada(id: number): Observable<RodadaNegociacao> {
    return this.http.post<RodadaNegociacao>(`${this.url}${id}/rodadas`, null).pipe(take(1));
  }

  getConcorrentes(id: number): Observable<Concorrente[]> {
    return this.http.get<Concorrente[]>(`${this.url}${id}/concorrentes`).pipe(take(1));
  }

  postConcorrente(id: number, concorrente: Concorrente): Observable<Concorrente> {
    return this.http.post<Concorrente>(`${this.url}${id}/concorrentes`, concorrente).pipe(take(1));
  }

  deleteConcorrente(idNegociacao: number, id: number): Observable<Concorrente> {
    return this.http.delete<Concorrente>(`${this.url}${idNegociacao}/concorrentes/${id}`).pipe(take(1));
  }

  uploadFiles(idSindicato: number, files: FileList): Observable<HttpEvent<{}>> {
    return this.arquivosApi.uploadFiles(`${this.url}${idSindicato}/files`, files);
   }

   getArquivos(id: number): Observable<Arquivo[]> {
    return this.http.get<Arquivo[]>(`${this.url}${id}/files`)
    .pipe(take(1));
   }

   getIntervalYears(): Observable<any> {
    return this.http.get<any>(`${this.url}interval`)
    .pipe(take(1));
   }

}
