import { Injectable } from '@angular/core';
import { StatusNegociacao } from '../model/negociacao';

@Injectable({
  providedIn: 'root'
})
export class NegociacoesService {

  StatusNegociacao: typeof StatusNegociacao = StatusNegociacao;

  constructor() { }

  classStatus(status: StatusNegociacao) {
    switch (StatusNegociacao[status]) {
      case StatusNegociacao.Dissidio:
        return 'label-danger';
      case StatusNegociacao.EmNegociacao:
        return 'label-warning';
      case StatusNegociacao.Fechada:
        return 'label-primary';
      case StatusNegociacao.NaoIniciada:
        return 'label-success';
    }
  }
}
