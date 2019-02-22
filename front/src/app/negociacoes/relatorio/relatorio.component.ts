import { Component, OnInit } from '@angular/core';
import { filter, switchMap } from 'rxjs/operators';
import { Relatorio, GrupoPergunta, AplicacaoResposta, RespostaRelatorio } from 'src/app/model/relatorio';
import { ActivatedRoute, Router } from '@angular/router';
import { PdfGeneratorService } from 'src/app/shared/pdf-generator.service';

@Component({
  selector: 'app-relatorio',
  templateUrl: './relatorio.component.html',
  styleUrls: ['./relatorio.component.css']
})
export class RelatorioComponent implements OnInit {

  relatorio: Relatorio;
  AplicacaoResposta: typeof AplicacaoResposta = AplicacaoResposta;

  constructor(private route: ActivatedRoute, private router: Router,
    private pdfGeneratorService: PdfGeneratorService) { }

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
      pergunta: "Nova Pergunta",
      resposta: ''
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

  download() {
    this.pdfGeneratorService.htmltoPDF('#relatorio-container', 'teste.pdf');
  }

}
