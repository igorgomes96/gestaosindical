import { SindicatoPatronal } from 'src/app/model/sindicato-patronal';
import { SindicatoLaboral } from './sindicato-laboral';
import { Empresa } from './empresa';

export enum ProcedimentoLitigio {
    MesaRedonda = 'Mesa Redonda',
    Audiencia = 'Audiência',
    ReclamacaoSindicato = 'Reclamação Sindicato'
}

export enum Referente {
    MTE = 'MTE',
    MPT = 'MPT',
    Laboral = 'Sindicato Laboral',
    Patronal = 'Sindicato Patronal'
}

export class Litigio {
    id: number;
    empresaId: number;
    referente: Referente;
    laboralId: number;
    patronalId: number;
    procedimento: ProcedimentoLitigio;
    data: string;
    assuntos: string;
    resumoAssuntos: string;
    participantes: string;

    empresa: Empresa;
    laboral: SindicatoLaboral;
    patronal: SindicatoPatronal;
}

