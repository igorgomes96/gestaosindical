import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { endpoints } from 'src/environments/endpoints';
import { GenericApi } from './generic-api';
import { Usuario } from 'src/app/model/usuario';

@Injectable({
  providedIn: 'root'
})
export class UsuariosApiService extends GenericApi<Usuario> {

  constructor(private http: HttpClient) {
    super(http, environment.api + endpoints.usuarios);
  }

}
