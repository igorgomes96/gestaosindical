import { StatusPlanoAcao } from 'src/app/model/plano-acao';
import { SindicatoPatronal } from 'src/app/model/sindicato-patronal';
import { SindicatoLaboral } from './sindicato-laboral';
import { Empresa } from './empresa';
import { PlanoAcao } from './plano-acao';

export enum ProcedimentoLitigio {
    MesaRedonda = 'Mesa Redonda',
    Audiencia = 'Audiência',
    ReclamacaoSindicato = 'Reclamação Sindicato',
    FiscalizacaoMPT = 'Fiscalização MPT',
    FiscalizacaoMTE = 'Fiscalização MTE'
}

export enum Referente {
    MTE = 'MTE',
    MPT = 'MPT',
    Laboral = 'Sindicato Laboral',
    Patronal = 'Sindicato Patronal'
}

export class ItemLitigio {
    id: number;
    litigioId: number;
    assuntos: string;
    planoAcaoId: number;
    possuiPlano: boolean;

    litigio: Litigio;
    planoAcao: PlanoAcao;
    statusPlanos: StatusPlanoAcao;
}

export class Litigio {
    id: number;
    empresaId: number;
    referente: Referente;
    laboralId: number;
    patronalId: number;
    procedimento: ProcedimentoLitigio;
    data: Date;
    assuntos: string;
    resumoAssuntos: string;
    participantes: string;
    estado: string;

    empresa: Empresa;
    laboral: SindicatoLaboral;
    patronal: SindicatoPatronal;

    itens: ItemLitigio[];
}

