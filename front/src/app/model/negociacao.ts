import { SindicatoPatronal } from './sindicato-patronal';
import { SindicatoLaboral } from './sindicato-laboral';
import { Empresa } from './empresa';

export enum StatusNegociacao {
    NaoIniciada = 'Não Iniciada',
    EmNegociacao = 'Em Negociação',
    Fechada = 'Fechada',
    Dissidio = 'Dissídio'
}


export class Negociacao {
    id: number;
    ano: number;
    empresaId: number;
    sindicatoLaboralId: number;
    sindicatoPatronalId: number;
    orcadoId: number;
    negociadoId: number;
    qtdaTrabalhadores: number;
    massaSalarial: number;
    taxaLaboral: number;
    taxaPatronal: number;
    statusACT_CCT: StatusNegociacao;
    statusPLR: StatusNegociacao;
    qtdaRodadas: number;
    plr1Sem: number;
    plr2Sem: number;

    empresa: Empresa;
    sindicatoLaboral: SindicatoLaboral;
    sindicatoPatronal: SindicatoPatronal;
    orcado: Reajuste;
    negociado: Reajuste;

    rodadasNegociacoes: RodadaNegociacao[];
}

export class RodadaNegociacao {
    id: number;
    negociacaoId: number;
    numero: number;
    data: Date;
    resumo: string;
    custosViagens: number;
    negociacao: Negociacao;
}

export class Reajuste {
    id: number;
    negociacaoId: number;
    salario: number;
    piso: number;
    auxCreche: number;
    vaVr: number;
    vaVrFerias: boolean;
    descontoVt: number;
}

export class Concorrente {
    id: number;
    negociacaoId: number;
    reajusteId: number;

    reajuste: Reajuste;
}
