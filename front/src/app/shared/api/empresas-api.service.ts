import { HttpClient, HttpEvent } from '@angular/common/http';
import { Empresa } from 'src/app/model/empresa';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { GenericApi } from './generic-api';
import { ArquivosApiService } from './arquivos-api.service';
import { endpoints } from 'src/environments/endpoints';
import { Arquivo } from 'src/app/model/arquivo';
import { Contato } from 'src/app/model/contato';
import { Usuario } from 'src/app/model/usuario';

@Injectable({
  providedIn: 'root'
})
export class EmpresasApiService extends GenericApi<Empresa>  {

  constructor(private http: HttpClient, private arquivosApi: ArquivosApiService) {
    super(http, environment.api + endpoints.empresa);
   }

   uploadFiles(idEmpresa: number, files: FileList): Observable<HttpEvent<{}>> {
    return this.arquivosApi.uploadFiles(`${this.url}${idEmpresa}/files`, files);
   }

   getArquivos(id: number): Observable<Arquivo[]> {
    return this.http.get<Arquivo[]>(`${this.url}${id}/files`)
    .pipe(take(1));
   }

   getContatos(idEmpresa: number): Observable<Contato[]> {
    return this.http.get<Contato[]>(`${this.url}${idEmpresa}/contatos`)
    .pipe(take(1));
   }

   addContato(idEmpresa: number, contato: Contato): Observable<Contato> {
    return this.http.post<Contato>(`${this.url}${idEmpresa}/contatos`, contato)
    .pipe(take(1));
   }

   deleteContato(idEmpresa: number, idContato: number): Observable<Contato> {
    return this.http.delete<Contato>(`${this.url}${idEmpresa}/contatos/${idContato}`)
    .pipe(take(1));
   }

   getRelated(userName: string) {
    return this.http.get<Empresa[]>(`${this.url}usuarios/${userName}`)
    .pipe(take(1));
   }

   allowUser(idEmpresa: number, user: Usuario) {
    return this.http.post(`${this.url}${idEmpresa}/allowuser`, user)
    .pipe(take(1));
   }

   disallowUser(idEmpresa: number, user: Usuario) {
    return this.http.post(`${this.url}${idEmpresa}/disallowuser`, user)
    .pipe(take(1));
   }
}
