import { Component, OnInit } from '@angular/core';
import { filter, switchMap, finalize, map } from 'rxjs/operators';
import { Relatorio, GrupoPergunta, AplicacaoResposta, RespostaRelatorio, LayoutGrupo } from 'src/app/model/relatorio';
import { ActivatedRoute, Router } from '@angular/router';
import { PdfGeneratorService } from 'src/app/shared/pdf-generator.service';
import { NegociacoesApiService } from 'src/app/shared/api/negociacoes-api.service';
import { ToastsService } from 'src/app/shared/toasts.service';
import { ToastType } from 'src/app/shared/toasts/toasts.component';
import * as Ladda from 'ladda';
declare var $: any;

@Component({
  selector: 'app-relatorio',
  templateUrl: './relatorio.component.html',
  styleUrls: ['./relatorio.component.css']
})
export class RelatorioComponent implements OnInit {

  relatorio: Relatorio;
  AplicacaoResposta: typeof AplicacaoResposta = AplicacaoResposta;
  LayoutGrupo: typeof LayoutGrupo = LayoutGrupo;
  saveLoadBtn: Ladda.LaddaButton;
  downloadLoadBtn: Ladda.LaddaButton;

  // grupo3Colunas = [
  //   'Proporção do Banco', 'Percentual de Horas Extras', 'Adicional Noturno',
  //   'Percentual de sobreaviso', 'Percentual intrajornada', 'Percentual interjornada',
  //   'Falta desconta DSR e feriado', 'Carência de intervalo', 'Tempo para não considerar intervalo'
  // ];
  grupoSemCombo = [
    'Ponto', 'Proporção do Banco', 'Percentual de Horas Extras', 'Adicional Noturno',
    'Percentual de sobreaviso', 'Percentual intrajornada', 'Percentual interjornada',
    'Falta desconta DSR e feriado', 'Carência de intervalo', 'Tempo para não considerar intervalo'
  ];

  constructor(private route: ActivatedRoute, private router: Router,
    private pdfGeneratorService: PdfGeneratorService,
    private service: NegociacoesApiService,
    private toasts: ToastsService) { }

  ngOnInit() {
    this.saveLoadBtn = Ladda.create(document.querySelector('#save-btn'));
    this.downloadLoadBtn = Ladda.create(document.querySelector('#download-btn'));

    this.route.data
      .pipe(
        filter(d => d.hasOwnProperty('relatorio')),
        map((d: any) => {
          const relatorio = d['relatorio'];
          if (relatorio) {
            relatorio.gruposPerguntas.forEach(g => {
              // if (this.grupo3Colunas.indexOf(g.texto) > -1) {
              //   g.layoutGrupo = LayoutGrupo.Grupo3Colunas;
              // } else
              if (this.grupoSemCombo.indexOf(g.texto) > -1) {
                g.layoutGrupo = LayoutGrupo.GrupoSemCombo;
              } else {
                g.layoutGrupo = LayoutGrupo.Grupo1Coluna;
              }
            });
          }
          return relatorio;
        })
      ).subscribe(d => {
        this.relatorio = d;
      });
  }

  addPergunta(grupo: GrupoPergunta) {
    grupo.respostas.push({
      id: undefined,
      aplicacaoResposta: AplicacaoResposta.Sim,
      grupoPergunta: null,
      grupoPerguntaId: grupo.id,
      numColunas: 12,
      ordem: grupo.respostas.length + 1,
      pergunta: 'Nova Pergunta',
      resposta: ''
    });
  }

  salvar() {
    this.saveLoadBtn.start();
    this.service.putRelatorio(this.relatorio.negociacaoId, this.relatorio)
      .pipe(finalize(() => this.saveLoadBtn.stop()))
      .subscribe((_: void) => {
        this.toasts.showMessage({
          message: 'Relatório salvo com sucesso!',
          title: 'Sucesso!',
          type: ToastType.success
        });
      });
  }

  addGrupo() {
    this.relatorio.gruposPerguntas.push({
      id: undefined,
      ordem: this.relatorio.gruposPerguntas.length,
      relatorioId: this.relatorio.id,
      respostas: [],
      layoutGrupo: LayoutGrupo.Grupo1Coluna,
      texto: 'Novo Grupo'
    });
  }

  deleteResposta(grupo: GrupoPergunta, resposta: RespostaRelatorio) {
    const index = grupo.respostas.indexOf(resposta);
    if (resposta.id != null) {
      this.service.deleteRespostaRelatorio(this.relatorio.negociacaoId, resposta.id)
        .subscribe();
    }
    grupo.respostas.splice(index, 1);
  }

  deleteGrupo(grupo: GrupoPergunta) {
    this.toasts.swalMessage({
      title: 'Confirma exlusão?',
      message: 'Todas as perguntas desse grupo serão excluídas!',
      type: ToastType.warning
    }, () => this.service.deleteGrupoRelatorio(this.relatorio.negociacaoId, grupo.id).subscribe(_ => {
      this.relatorio.gruposPerguntas.splice(this.relatorio.gruposPerguntas.indexOf(grupo), 1);
      this.toasts.showMessage({
        message: 'Grupo de Perguntas excluído com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      });
    }));
  }


  async download() {
    this.downloadLoadBtn.start();
    try {
      // tslint:disable-next-line:max-line-length
      await this.pdfGeneratorService.htmltoPDF('#relatorio .header', '#relatorio .group', `${this.relatorio.negociacao.empresa.nome} - ${this.relatorio.negociacao.ano}.pdf`);
    } catch (err) {
      console.error(err);
    } finally {
      this.downloadLoadBtn.stop();
    }
  }

}
