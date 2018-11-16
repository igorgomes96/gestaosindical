import { Contato } from './contato';

export class SindicatoPatronal {
    id: number;
    nome: string;
    gestao: string;
    cnpj: string;
    telefone1: string;
    telefone2: string;
    site: string;
    presidente: Contato;
    negociador: Contato;
}
