import { Component, OnInit } from '@angular/core';
import { filter, switchMap } from 'rxjs/operators';
import { Relatorio, GrupoPergunta, AplicacaoResposta, RespostaRelatorio } from 'src/app/model/relatorio';
import { ActivatedRoute, Router } from '@angular/router';
import { PdfGeneratorService } from 'src/app/shared/pdf-generator.service';
import { NegociacoesApiService } from 'src/app/shared/api/negociacoes-api.service';
import { ToastsService } from 'src/app/shared/toasts.service';
import { ToastType } from 'src/app/shared/toasts/toasts.component';

@Component({
  selector: 'app-relatorio',
  templateUrl: './relatorio.component.html',
  styleUrls: ['./relatorio.component.css']
})
export class RelatorioComponent implements OnInit {

  relatorio: Relatorio;
  AplicacaoResposta: typeof AplicacaoResposta = AplicacaoResposta;

  constructor(private route: ActivatedRoute, private router: Router,
    private pdfGeneratorService: PdfGeneratorService,
    private service: NegociacoesApiService,
    private toasts: ToastsService) { }

  ngOnInit() {
    this.route.data
      .pipe(filter(d => d.hasOwnProperty('relatorio')))
      .subscribe(d => {
        this.relatorio = d['relatorio'];
      });
  }

  addPergunta(grupo: GrupoPergunta) {
    grupo.respostas.push({
      id: undefined,
      aplicacaoResposta: AplicacaoResposta.NA,
      grupoPergunta: null,
      grupoPerguntaId: grupo.id,
      numColunas: 12,
      ordem: grupo.respostas.length + 1,
      pergunta: 'Nova Pergunta',
      resposta: ''
    });
  }

  salvar() {
    this.service.putRelatorio(this.relatorio.negociacaoId, this.relatorio)
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
    this.service.deleteGrupoRelatorio(this.relatorio.negociacaoId, grupo.id)
      .subscribe();
    this.relatorio.gruposPerguntas.splice(this.relatorio.gruposPerguntas.indexOf(grupo), 1);
  }


  download() {
    this.pdfGeneratorService.htmltoPDF('#relatorio', 'teste.pdf');
  }

}
