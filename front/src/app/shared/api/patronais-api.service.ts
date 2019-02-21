import { Arquivo } from './../../model/arquivo';
import { HttpClient, HttpEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';

import { SindicatoPatronal } from '../../model/sindicato-patronal';
import { endpoints } from '../../../environments/endpoints';
import { environment } from './../../../environments/environment';
import { Contato } from './../../model/contato';
import { GenericApi } from './generic-api';
import { ArquivosApiService } from './arquivos-api.service';

@Injectable({
  providedIn: 'root'
})
export class PatronaisApiService extends GenericApi<SindicatoPatronal> {

  constructor(private http: HttpClient, private arquivosApi: ArquivosApiService) {
    super(http, environment.api + endpoints.sindicatosPatronais);
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
}
