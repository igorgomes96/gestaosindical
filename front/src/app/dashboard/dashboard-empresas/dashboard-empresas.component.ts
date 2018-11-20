import { Location } from '@angular/common';
import { Concorrente } from './../../model/negociacao';
import { NegociacoesApiService } from './../../negociacoes/negociacoes-api.service';
import { ChartsService } from './../charts.service';
import { EmpresasApiService } from './../../empresas/empresas-api.service';
import { DashboardApiService } from './../dashboard-api.service';
import { distinctUntilChanged, switchMap, tap, filter } from 'rxjs/operators';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Empresa } from 'src/app/model/empresa';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { endpoints } from 'src/environments/endpoints';
import { ActivatedRoute } from '@angular/router';
import { Negociacao, StatusNegociacao } from 'src/app/model/negociacao';
import { NegociacoesService } from 'src/app/negociacoes/negociacoes.service';
import { of } from 'rxjs';
import { Options, ChangeContext } from 'ng5-slider';
import { IntervalFilterService } from 'src/app/shared/interval-filter.service';

declare var $: any;

@Component({
  selector: 'app-dashboard-empresas',
  templateUrl: './dashboard-empresas.component.html',
  styleUrls: ['./dashboard-empresas.component.css']
})
export class DashboardEmpresasComponent implements OnInit {

  urlEmpresaList = environment.api + endpoints.empresa;
  empresa: Empresa;
  form: FormGroup;
  negociacoes: Negociacao[];
  concorrentes: Concorrente[];
  StatusNegociacao: typeof StatusNegociacao = StatusNegociacao;
  ano: number;
  options: Options;
  charts: any[] = [];
  chartConcorrentes: any;

  constructor(private formBuilder: FormBuilder,
    private api: DashboardApiService,
    private route: ActivatedRoute,
    private empresasApi: EmpresasApiService,
    private negociacoesService: NegociacoesService,
    private negociacoesApi: NegociacoesApiService,
    private chartsService: ChartsService,
    private intervalService: IntervalFilterService,
    private location: Location) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      empresa: ['']
    });

    this.ano = this.intervalService.value;
    this.intervalService.options
      .subscribe(opt => {
        this.options = opt;
      });

    this.route.queryParamMap.pipe(
      switchMap(x => {
        if (x['params'].hasOwnProperty('empresaId')) {
          return this.empresasApi.get(x['params']['empresaId']);
        }
        return of(null);
      })
    ).subscribe((empresa: Empresa) => {
      if (empresa == null) {
        this.form.get('empresa').setValue(this.chartsService.ultimaEmpresa);
      } else {
        this.form.get('empresa').setValue(empresa);
      }
    });

    this.form.get('empresa').valueChanges
      .pipe(distinctUntilChanged())
      .subscribe(v => {
        this.chartsService.ultimaEmpresa = v;
        if (v) {
          this.empresasApi.get(v.id)
            .pipe(switchMap((empresa: Empresa) => {
              // window.history.replaceState({}, '', `/negociacoes/empresa?empresaId=${empresa.id}`);
              // this.location.replaceState(`/negociacoes/empresa?empresaId=${empresa.id}`);
              this.empresa = empresa;
              return this.api.getNegociacoes(v.id);
            })).subscribe(n => {
              this.charts.forEach(c => c.destroy());
              this.negociacoes = n;
              this.loadConcorrentes();
              this.charts.push(this.chartReajustesNegociados(n));
              this.charts.push(this.chartEvolucaoTrabalhadores(n));
              this.charts.push(this.chartEvolucaoSalario(n));
              this.charts.push(this.chartTaxasNegociais(n));
              this.charts.push(this.chartCustosViagens(n));
              this.charts.push(this.chartPLR(n));
              this.charts.push(this.chartOrcadoXNegociado(n, 'orcXnegSal', 'salario', 'Salário'));
              this.charts.push(this.chartOrcadoXNegociado(n, 'orcXnegPiso', 'piso', 'Piso'));
              this.charts.push(this.chartOrcadoXNegociado(n, 'orcXnegVaVr', 'vaVr', 'VA/VR'));
              this.charts.push(this.chartOrcadoXNegociado(n, 'orcXnegCreche', 'auxCreche', 'Aux. Creche'));
            });
        } else {
          this.empresa = null;
        }
      });
  }

  loadConcorrentes() {
    if (this.chartConcorrentes) {
      this.chartConcorrentes.destroy();
    }
    const negociacoesAno = this.negociacoes.filter(x => x.ano === this.ano);
    if (negociacoesAno && negociacoesAno.length > 0) {
      const negociacao = negociacoesAno[0];
      this.negociacoesApi.getConcorrentes(negociacao.id)
        .subscribe(c => {
          this.concorrentes = c;
          this.concorrentes.unshift(<Concorrente>{
            nome: this.empresa.nome,
            reajuste: negociacao.negociado
          });
          this.chartConcorrentes = this.chartConcorrentesSalario(this.concorrentes);
        });
    }
  }

  onUserChangeEnd(changeContext: ChangeContext): void {
    this.intervalService.value = changeContext.value;
    this.loadConcorrentes();
  }

  getDatasetsConcorrentes(concorrentes: Concorrente[]) {
    return concorrentes.map((c: Concorrente, i: number) => {
      return {
        label: c.nome,
        data: [c.reajuste.salario, c.reajuste.piso, c.reajuste.vaVr, c.reajuste.auxCreche],
        backgroundColor: this.chartsService.colors[i % this.chartsService.colors.length]
      };
    });
  }

  chartConcorrentesSalario(concorrentes: Concorrente[]) {
    const reajustes = (<HTMLCanvasElement>$('#concorrentes')[0]).getContext('2d');
    return this.chartsService.chart('bar', reajustes, {
      datasets: this.getDatasetsConcorrentes(concorrentes),
      labels: ['Salário', 'Piso', 'VA/VR', 'Aux. Creche']
    },
      {
        legend: {
          display: true
        },
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
            }
          }]
        },
        maintainAspectRatio: false
      });
  }

  chartEvolucaoTrabalhadores(n: Negociacao[]) {
    const evolucao = (<HTMLCanvasElement>$('#evolucaoTrabalhadores')[0]).getContext('2d');
    return this.chartsService.chart('line', evolucao, {
      datasets: [
        {
          data: n.map(y => y.qtdaTrabalhadores),
          backgroundColor: '#d8e0f3'
        }
      ],
      labels: n.map(x => x.ano)
    }, {
        legend: {
          display: false
        },
        title: {
          display: true,
          text: 'Qtda de Trabalhadores'
        },
        maintainAspectRatio: false
      });
  }

  chartEvolucaoSalario(n: Negociacao[]) {
    const evolucao = (<HTMLCanvasElement>$('#evolucaoSalario')[0]).getContext('2d');
    return this.chartsService.chart('line', evolucao, {
      datasets: [
        {
          data: n.map(y => y.massaSalarial),
          backgroundColor: '#d8e0f3'
        }
      ],
      labels: n.map(x => x.ano)
    }, {
        legend: {
          display: false
        },
        title: {
          display: true,
          text: 'Massa Salarial'
        },
        maintainAspectRatio: false
      });
  }

  chartCustosViagens(n: Negociacao[]) {
    const custosViagens = (<HTMLCanvasElement>$('#custosViagens')[0]).getContext('2d');
    return this.chartsService.chart('line', custosViagens, {
      datasets: [
        {
          label: 'Custos em Viagens',
          data: n.map(y => y.custosViagens),
          backgroundColor: '#d8e0f3'
        }
      ],
      labels: n.map(x => x.ano)
    },
      {
        legend: {
          display: false
        },
        title: {
          display: true,
          text: 'Custos em Viagens'
        },
        maintainAspectRatio: false
      });
  }

  chartTaxasNegociais(n: Negociacao[]) {
    const reajustes = (<HTMLCanvasElement>$('#taxasNegociais')[0]).getContext('2d');
    return this.chartsService.chart('line', reajustes, {
      datasets: [
        {
          label: 'Tx. Neg. Patronal',
          data: n.map(y => y.taxaPatronal),
          borderColor: this.chartsService.colors[0],
          fill: false
        },
        {
          label: 'Tx. Neg. Laboral',
          data: n.map(y => y.taxaLaboral),
          borderColor: this.chartsService.colors[1],
          fill: false
        }
      ],
      labels: n.map(x => x.ano)
    },
      {
        legend: {
          display: true
        },
        title: {
          display: true,
          text: 'Taxas Negociais'
        },
        maintainAspectRatio: false
      });
  }

  chartPLR(n: Negociacao[]) {
    const plr = (<HTMLCanvasElement>$('#plr')[0]).getContext('2d');
    return this.chartsService.chart('line', plr, {
      datasets: [
        {
          label: '1º Semestre',
          data: n.map(y => y.plr1Sem),
          borderColor: this.chartsService.colors[0],
          fill: false
        },
        {
          label: '2º Semestre',
          data: n.map(y => y.plr2Sem),
          borderColor: this.chartsService.colors[1],
          fill: false
        }
      ],
      labels: n.map(x => x.ano)
    },
      {
        legend: {
          display: true
        },
        title: {
          display: true,
          text: 'Valores pagos em PLR'
        },
        maintainAspectRatio: false
      });
  }

  chartOrcadoXNegociado(n: Negociacao[], id: string, property: string, title: string) {
    const reajustes = (<HTMLCanvasElement>$(`#${id}`)[0]).getContext('2d');
    return this.chartsService.chart('bar', reajustes, {
      datasets: [
        {
          label: 'Orçado',
          data: n.map(y => y.orcado[property]),
          backgroundColor: this.chartsService.colors[1],
        },
        {
          label: 'Negociado',
          data: n.map(y => y.negociado[property]),
          backgroundColor: this.chartsService.colors[2],
        }
      ],
      labels: n.map(x => x.ano)
    },
      {
        legend: {
          display: true
        },
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
            }
          }]
        },
        title: {
          display: true,
          text: title
        },
        maintainAspectRatio: false
      });
  }

  chartReajustesNegociados(n: Negociacao[]) {
    const reajustes = (<HTMLCanvasElement>$('#reajustesNegociados')[0]).getContext('2d');
    return this.chartsService.chart('bar', reajustes, {
      datasets: [
        {
          label: '% Salário',
          data: n.map(y => y.negociado.salario),
          backgroundColor: this.chartsService.colors[0]
        },
        {
          label: '% Piso',
          data: n.map(y => y.negociado.piso),
          backgroundColor: this.chartsService.colors[1]
        },
        {
          label: '% VA/VR',
          data: n.map(y => y.negociado.vaVr),
          backgroundColor: this.chartsService.colors[2]
        },
        {
          label: '% Aux. Creche',
          data: n.map(y => y.negociado.auxCreche),
          backgroundColor: this.chartsService.colors[3]
        }
      ],
      labels: n.map(x => x.ano)
    },
      {
        legend: {
          display: true
        },
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
            }
          }]
        },
        title: {
          display: true,
          text: 'Histórico de Reajustes'
        },
        maintainAspectRatio: false
      });
  }

  linkEmpresa(valor) {
    return `/empresas/${valor.id}`;
  }

  classStatus(status: StatusNegociacao) {
    return this.negociacoesService.classStatus(status);
  }

}
