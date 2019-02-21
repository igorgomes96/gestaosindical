import { endpoints } from 'src/environments/endpoints';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { take } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { RodadaNegociacao, Negociacao } from 'src/app/model/negociacao';

@Injectable({
  providedIn: 'root'
})
export class DashboardApiService {

  url = environment.api + endpoints.dashboard;
  constructor(private httpClient: HttpClient) { }

  getPlrMassaSalarial(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}plr/massasalarial/${ano}`).pipe(take(1));
  }

  getPlrTrabalhadores(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}plr/trabalhadores/${ano}`).pipe(take(1));
  }

  getActMassaSalarial(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}act/massasalarial/${ano}`).pipe(take(1));
  }

  getActTrabalhadores(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}act/trabalhadores/${ano}`).pipe(take(1));
  }

  getCustosViagens(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}custosviagens/${ano}`).pipe(take(1));
  }

  getProximasReunioes(): Observable<RodadaNegociacao[]> {
    return this.httpClient.get<RodadaNegociacao[]>(`${this.url}proximasviagens`).pipe(take(1));
  }

  getMediaReajustes(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}mediareajustes/${ano}`).pipe(take(1));
  }

  getNegociacoes(empresaId: number): Observable<Negociacao[]> {
    return this.httpClient.get<any[]>(`${this.url}negociacoes/${empresaId}`).pipe(take(1));
  }

  getQtdaReunioesLaboral(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}qtdareunioes/${ano}/laboral`).pipe(take(1));
  }

  getQtdaReunioesPatronal(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}qtdareunioes/${ano}/patronal`).pipe(take(1));
  }

  getQtdaReunioesEstado(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}qtdareunioes/${ano}/estado`).pipe(take(1));
  }

  getQtdaReunioesEmpresa(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}qtdareunioes/${ano}/empresa`).pipe(take(1));
  }

  getStatusPlanosAcao(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}planosacao/${ano}/status`).pipe(take(1));
  }

  getPlanosAcaoReferenteA(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}planosacao/${ano}/referente`).pipe(take(1));
  }

  getPlanosAcaoProcedencia(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}planosacao/${ano}/procedencia`).pipe(take(1));
  }

  getLitigiosReferenteA(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}litigios/${ano}/referente`).pipe(take(1));
  }

  getLitigiosEmpresa(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}litigios/${ano}/empresa`).pipe(take(1));
  }

  getLitigiosMes(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}litigios/${ano}/mes`).pipe(take(1));
  }

  getLitigiosProcedimento(ano: number): Observable<any[]> {
    return this.httpClient.get<any[]>(`${this.url}litigios/${ano}/procedimento`).pipe(take(1));
  }
}
