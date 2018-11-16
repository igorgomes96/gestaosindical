import { HttpClient, HttpEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { GenericApi } from './../../shared/generic-api';
import { Litigio } from './../../model/litigio';
import { environment } from 'src/environments/environment';
import { endpoints } from 'src/environments/endpoints';
import { ArquivosApiService } from 'src/app/shared/arquivos/arquivos-api.service';
import { Observable } from 'rxjs';
import { Arquivo } from 'src/app/model/arquivo';
import { take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class LitigiosApiService extends GenericApi<Litigio> {

  constructor(private http: HttpClient, private arquivosApi: ArquivosApiService) {
    super(http, environment.api + endpoints.litigios);
  }

  uploadFiles(idSindicato: number, files: FileList): Observable<HttpEvent<{}>> {
    return this.arquivosApi.uploadFiles(`${this.url}${idSindicato}/files`, files);
   }

   getArquivos(id: number): Observable<Arquivo[]> {
    return this.http.get<Arquivo[]>(`${this.url}${id}/files`)
    .pipe(take(1));
   }
}
