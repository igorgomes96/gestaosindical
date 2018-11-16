import { Contato } from './contato';
export enum CCT_ACT {
    CCT = 'CCT',
    ACT = 'ACT'
}
export enum Mes {
    Janeiro = 'Janeiro',
    Fevereiro = 'Fevereiro',
    Marco = 'Março',
    Abril = 'Abril',
    Maio = 'Maio',
    Junho = 'Junho',
    Julho = 'Julho',
    Agosto = 'Agosto',
    Setembro = 'Setembro',
    Outubro = 'Outubro',
    Novembro = 'Novembro',
    Dezembro = 'Dezembro'
}

export class SindicatoLaboral {
    id: number;
    nome: string;
    cnpj: string;
    telefone1: string;
    telefone2: string;
    gestao: string;
    site: string;
    federacao: string;
    cct_act: CCT_ACT;
    database: Mes;
    presidente: Contato;
    negociador: Contato;
}
