import { SindicatoPatronal } from './sindicato-patronal';
import { Referente } from './litigio';
import { SindicatoLaboral } from './sindicato-laboral';

export enum StatusPlanoAcao {
    NoPrazo = 'No Prazo',
    Vencido = 'Vencido',
    AVencer = 'A vencer',  // 10 dias antes da data
    Solucionado = 'Solucionado'
}

export class PlanoAcao {
    id: number;
    referente: Referente;
    laboralId: number;
    patronalId: number;
    data: string;
    estado: string;
    reclamacoes: string;
    reclamante: string;
    procedencia: boolean;
    responsavelAcao: string;
    dataSolucao: string;
    status: StatusPlanoAcao;

    laboral: SindicatoLaboral;
    patronal: SindicatoPatronal;
}
