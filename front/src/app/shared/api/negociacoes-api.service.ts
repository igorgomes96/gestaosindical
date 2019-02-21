import { take } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient, HttpEvent } from '@angular/common/http';

import { environment } from 'src/environments/environment';
import { GenericApi } from './generic-api';
import { Negociacao, RodadaNegociacao, Concorrente, ParcelaReajuste } from 'src/app/model/negociacao';
import { ArquivosApiService } from './arquivos-api.service';
import { endpoints } from 'src/environments/endpoints';
import { Arquivo } from 'src/app/model/arquivo';

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

  getCalendar(mes: number): Observable<any[]> {
    return this.http.get<RodadaNegociacao[]>(`${this.url}calendar/${mes}`).pipe(take(1));
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

   postParcela(id: number, Parcela: ParcelaReajuste): Observable<ParcelaReajuste> {
    return this.http.post<ParcelaReajuste>(`${this.url}${id}/parcelas`, Parcela).pipe(take(1));
  }

  deleteParcela(idNegociacao: number, id: number): Observable<ParcelaReajuste> {
    return this.http.delete<ParcelaReajuste>(`${this.url}${idNegociacao}/parcelas/${id}`).pipe(take(1));
  }

  postRelatorio(idNegociacao: number): Observable<void> {
    return this.http.post<void>(`${this.url}${idNegociacao}/relatorio`, null).pipe(take(1));
  }

}
