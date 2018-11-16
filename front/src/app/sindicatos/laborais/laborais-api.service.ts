import { Observable } from 'rxjs';
import { HttpClient, HttpEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from './../../../environments/environment';
import { SindicatoLaboral } from './../../model/sindicato-laboral';
import { GenericApi } from './../../shared/generic-api';
import { endpoints } from '../../../environments/endpoints';
import { Arquivo } from 'src/app/model/arquivo';
import { take } from 'rxjs/operators';
import { Contato } from 'src/app/model/contato';
import { ArquivosApiService } from './../../shared/arquivos/arquivos-api.service';

@Injectable({
  providedIn: 'root'
})
export class LaboraisApiService extends GenericApi<SindicatoLaboral> {

  constructor(private http: HttpClient, private arquivosApi: ArquivosApiService) {
    super(http, environment.api + endpoints.sindicatosLaborais);
   }

   uploadFiles(idSindicato: number, files: FileList): Observable<HttpEvent<{}>> {
    return this.arquivosApi.uploadFiles(`${this.url}${idSindicato}/files`, files);
   }

   getArquivos(id: number): Observable<Arquivo[]> {
    return this.http.get<Arquivo[]>(`${this.url}${id}/files`)
    .pipe(take(1));
   }

   getContatos(idSindicato: number): Observable<Contato[]> {
    return this.http.get<Contato[]>(`${this.url}${idSindicato}/contatos`)
    .pipe(take(1));
   }

   addContato(idSindicato: number, contato: Contato): Observable<Contato> {
    return this.http.post<Contato>(`${this.url}${idSindicato}/contatos`, contato)
    .pipe(take(1));
   }

   deleteContato(idSindicato: number, idContato: number): Observable<Contato> {
    return this.http.delete<Contato>(`${this.url}${idSindicato}/contatos/${idContato}`)
    .pipe(take(1));
   }

  //  list(q: string) {
  //   return this.http.get<string[]>(`${this.url}/list`, { params: {q: q}})
  //   .pipe(take(1));
  //  }
}
