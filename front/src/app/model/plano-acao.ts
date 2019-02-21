import { ItemLitigio } from "./litigio";

export enum StatusPlanoAcao {
    NoPrazo = 'No Prazo',
    Vencido = 'Vencido',
    AVencer = 'A vencer',  // 10 dias antes da data
    Solucionado = 'Solucionado'
}

export class PlanoAcao {
    id: number;
    data: Date;
    procedencia: boolean;
    responsavelAcao: string;
    dataPrevista: Date;
    dataSolucao: Date;
    status: StatusPlanoAcao;
    itemLitigioId: number;

    ItemLitigio: ItemLitigio;
}
