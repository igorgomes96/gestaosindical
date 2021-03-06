import { Negociacao } from './negociacao';

export enum LayoutGrupo {
    Grupo1Coluna,
    Grupo3Colunas,
    GrupoSemCombo
}

export enum AplicacaoResposta {
    Sim,
    Nao,
    NA
}

export class Relatorio {
    id: number;
    negociacaoId: number;
    titulo: string;

    negociacao: Negociacao;
    gruposPerguntas: GrupoPergunta[];
}

export class GrupoPergunta {
    id: number;
    relatorioId: number;
    ordem: number;
    texto: string;
    layoutGrupo: LayoutGrupo;

    respostas: RespostaRelatorio[];
}

export class RespostaRelatorio {
    id: number;
    ordem: number;
    pergunta: string;
    resposta: string;
    aplicacaoResposta: AplicacaoResposta;
    grupoPerguntaId: number;
    numColunas: number;

    grupoPergunta: GrupoPergunta;

}
