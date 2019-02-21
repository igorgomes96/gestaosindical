import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { endpoints } from 'src/environments/endpoints';
import { Estado } from 'src/app/model/estado';

@Injectable({
  providedIn: 'root'
})
export class EstadosApiService {

  url: string;
  constructor(private httpCliente: HttpClient) {
    this.url = endpoints.estados;
  }

  getEstados(): Observable<Estado[]> {
    return this.httpCliente.get<Estado[]>(this.url);
  }

}
