export enum TipoContato {
    Presidente,
    Negociador,
    Contato,
    Outro
}

export class Contato {
    id: number;
    nome: string;
    telefone1: string;
    telefone2: string;
    email: string;
    tipoContato: TipoContato;
}
