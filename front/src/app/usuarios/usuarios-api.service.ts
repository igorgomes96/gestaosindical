import { take } from 'rxjs/operators';
import { Usuario } from './../model/usuario';
import { Injectable } from '@angular/core';
import { GenericApi } from '../shared/generic-api';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { endpoints } from 'src/environments/endpoints';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UsuariosApiService extends GenericApi<Usuario> {

  constructor(private http: HttpClient) {
    super(http, environment.api + endpoints.usuarios);
  }

}
