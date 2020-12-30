import { Referente, ProcedimentoLitigio } from './../../model/litigio';
import { StatusPlanoAcao } from './../../model/plano-acao';
import { Mes } from './../../model/sindicato-laboral';
import { StatusNegociacao } from 'src/app/model/negociacao';
import { Component, OnInit } from '@angular/core';
import { ChartsService } from '../charts.service';
import { map } from 'rxjs/operators';
import { Options, ChangeContext } from 'ng5-slider';
import { IntervalFilterService } from 'src/app/shared/interval-filter.service';
import { ActivatedRoute } from '@angular/router';
import { DashboardApiService } from 'src/app/shared/api/dashboard-api.service';
import { formatNumber, formatCurrency } from '@angular/common';

declare var Chart: any;
declare var $: any;

@Component({
  selector: 'app-dashboard-negociacoes',
  templateUrl: './dashboard-negociacoes.component.html',
  styleUrls: ['./dashboard-negociacoes.component.css']
})
export class DashboardNegociacoesComponent implements OnInit {

  StatusNegociacao: typeof StatusNegociacao = StatusNegociacao;
  StatusPlanoAcao: typeof StatusPlanoAcao = StatusPlanoAcao;
  Referente: typeof Referente = Referente;
  ProcedimentoLitigio: typeof ProcedimentoLitigio = ProcedimentoLitigio;
  charts: any[] = [];
  ano: number;
  options: Options;
  referent: any;
  reunioes: any[];

  constructor(private chartsService: ChartsService, private route: ActivatedRoute,
    private api: DashboardApiService, private intervalService: IntervalFilterService) { }

  ngOnInit() {
    this.intervalService.options
      .subscribe(opt => {
        this.options = opt;
      });

    this.route.queryParamMap
      .subscribe(d => {
        this.referent = {};
        Object.assign(this.referent, d['params']);
        if (this.referent.hasOwnProperty('ano')) {
          this.ano = this.intervalService.value = this.referent['ano'];
        } else {
          this.referent['ano'] = this.ano = this.intervalService.value;
        }
        this.load();
      });

  }

  private formatPercentCb = function(value: any) {
    return formatNumber(value, 'pt-BR', '1.0-2') + '%';
  };

  private formatCurrencyCb = function(value: any) {
    return formatCurrency(value, 'pt-BR', 'R$');
  };

  onUserChangeEnd(changeContext: ChangeContext): void {
    this.referent['ano'] = changeContext.value;
    this.intervalService.value = changeContext.value;
    this.load();
  }

  getOptionsPieChart(title: string = null, showLegendas = false, positionLegend = 'left', maintainAspectRatio = true) {
    return {
      title: {
        display: title != null,
        text: title
      },
      legend: {
        display: showLegendas,
        position: positionLegend
      },
      maintainAspectRatio: maintainAspectRatio,
      tooltips: {
        callbacks: {
          label: this.chartsService.labelPercent
        }
      }
    };
  }

  load() {

    this.charts.forEach(x => x.destroy());

    const plrMassaSalarial = (<HTMLCanvasElement>$('#plrMassaSalarial')[0]).getContext('2d');
    this.api.getPlrMassaSalarial(this.ano)
      .pipe(
        map(d => d.map(x => Object.assign(x, { label: StatusNegociacao[x['label']] }))),
        map(d => this.chartsService.mapChartData(d, this.chartsService.mapColorStatusNegociacao)))
      .subscribe(d => this.charts.push(this.chartsService
        .chart('pie', plrMassaSalarial, d, this.getOptionsPieChart('% Massa Salarial')))
      );

    const plrTrabalhadores = (<HTMLCanvasElement>$('#plrTrabalhadores')[0]).getContext('2d');
    this.api.getPlrTrabalhadores(this.ano)
      .pipe(
        map(d => d.map(x => Object.assign(x, { label: StatusNegociacao[x['label']] }))),
        map(d => this.chartsService.mapChartData(d, this.chartsService.mapColorStatusNegociacao)))
      .subscribe(d => this.charts.push(this.chartsService
        .chart('pie', plrTrabalhadores, d, this.getOptionsPieChart('% Qtda de Trabalhadores')))
      );

    const actMassaSalarial = (<HTMLCanvasElement>$('#actMassaSalarial')[0]).getContext('2d');
    this.api.getActMassaSalarial(this.ano)
      .pipe(
        map(d => d.map(x => Object.assign(x, { label: StatusNegociacao[x['label']] }))),
        map(d => this.chartsService.mapChartData(d, this.chartsService.mapColorStatusNegociacao)))
      .subscribe(d => this.charts.push(this.chartsService
        .chart('pie', actMassaSalarial, d, this.getOptionsPieChart('% Massa Salarial')))
      );

    const actTrabalhadores = (<HTMLCanvasElement>$('#actTrabalhadores')[0]).getContext('2d');
    this.api.getActTrabalhadores(this.ano)
      .pipe(
        map(d => d.map(x => Object.assign(x, { label: StatusNegociacao[x['label']] }))),
        map(d => this.chartsService.mapChartData(d, this.chartsService.mapColorStatusNegociacao)))
      .subscribe(d => this.charts.push(this.chartsService
        .chart('pie', actTrabalhadores, d, this.getOptionsPieChart('% Qtda de Trabalhadores')))
      );

    const custosViagens = (<HTMLCanvasElement>$('#custosViagens')[0]).getContext('2d');
    this.api.getCustosViagens(this.ano)
      .subscribe(d => {
        this.charts.push(this.chartsService.chart('line', custosViagens, {
          datasets: [
            {
              label: 'Custos',
              data: d.map(y => y.y),
              backgroundColor: '#d8e0f3'
            }
          ],
          labels: d.map(x => Mes[x.label])
        }, {
            legend: {
              display: false
            },
            maintainAspectRatio: false,
            scales: {
              yAxes: [{
                ticks: {
                  callback: this.formatCurrencyCb
                }
              }],
            },
            tooltips: {
              callbacks: {
                label: this.chartsService.labelRealSymbol
              }
            }
          }));
      });


    this.api.getProximasReunioes()
      .subscribe(d => this.reunioes = d);

    const mediaReajustes = (<HTMLCanvasElement>$('#mediaReajustes')[0]).getContext('2d');
    this.api.getMediaReajustes(this.ano)
      .subscribe(d => {
        if (d && d.length > 0) {
          this.charts.push(this.chartsService.chart('bar', mediaReajustes, {
            datasets: d.map((r, i) => {
              return {
                label: r.label,
                data: r.data.map(x => x.y),
                backgroundColor: this.chartsService.colors[i]
              };
            }),
            labels: d[0].data.map(x => x.label)
          }, {
              maintainAspectRatio: false,
              scales: {
                yAxes: [{
                  ticks: {
                    beginAtZero: true,
                    callback: this.formatPercentCb
                  }
                }]
              },
              tooltips: {
                callbacks: {
                  label: this.chartsService.labelPercentSymbol
                }
              }
            }));
        }
      });

    this.api.getQtdaReunioesEstado(this.ano)
      .subscribe(x => this.charts.push(this.chartHorizontalBar('reunioesEstado', x)));
    this.api.getQtdaReunioesLaboral(this.ano)
      .subscribe(x => this.charts.push(this.chartHorizontalBar('reunioesLaboral', x)));
    this.api.getQtdaReunioesPatronal(this.ano)
      .subscribe(x => this.charts.push(this.chartHorizontalBar('reunioesPatronal', x)));
    this.api.getQtdaReunioesEmpresa(this.ano)
      .subscribe(x => this.charts.push(this.chartHorizontalBar('reunioesEmpresa', x)));

    const statusPlanos = (<HTMLCanvasElement>$('#statusPlanos')[0]).getContext('2d');
    this.api.getStatusPlanosAcao(this.ano)
      .pipe(
        map(d => d.map(x => Object.assign(x, { label: StatusPlanoAcao[x['label']] }))),
        map(d => this.chartsService.mapChartData(d, this.chartsService.mapColorStatusPlanos))
      ).subscribe(d => this.charts.push(this.chartsService
        .chart('pie', statusPlanos, d, this.getOptionsPieChart(null, true)))
      );

    const referenciaPlanos = (<HTMLCanvasElement>$('#referenciaPlanos')[0]).getContext('2d');
    this.api.getPlanosAcaoReferenteA(this.ano)
      .pipe(
        map(d => d.map(x => Object.assign(x, { label: Referente[x['label']] }))),
        map(d => this.chartsService.mapChartData(d, this.chartsService.mapColorReferencia))
      ).subscribe(d => this.charts.push(this.chartsService
        .chart('pie', referenciaPlanos, d, this.getOptionsPieChart(null, true)))
      );

    const procedenciaPlanos = (<HTMLCanvasElement>$('#procedenciaPlanos')[0]).getContext('2d');
    this.api.getPlanosAcaoProcedencia(this.ano)
      .pipe(
        map(d => this.chartsService.mapChartData(d, this.chartsService.mapColorProcedenciaPlanos))
      ).subscribe(d => this.charts.push(this.chartsService
        .chart('pie', procedenciaPlanos, d, this.getOptionsPieChart(null, true)))
      );

    const referenciaLitigios = (<HTMLCanvasElement>$('#referenciaLitigios')[0]).getContext('2d');
    this.api.getLitigiosReferenteA(this.ano)
      .pipe(
        map(d => d.map(x => Object.assign(x, { label: Referente[x['label']] }))),
        map(d => this.chartsService.mapChartData(d, this.chartsService.mapColorReferencia))
      ).subscribe(d => this.charts.push(this.chartsService
        .chart('pie', referenciaLitigios, d, this.getOptionsPieChart(null, true, 'left', false)))
      );

    const litigiosProcedimento = (<HTMLCanvasElement>$('#litigiosProcedimento')[0]).getContext('2d');
    this.api.getLitigiosProcedimento(this.ano)
      .pipe(
        map(d => d.map(x => Object.assign(x, { label: ProcedimentoLitigio[x['label']] }))),
        map(d => this.chartsService.mapChartData(d, this.chartsService.mapColorProcedimentoLitigio))
      ).subscribe(d => this.charts.push(this.chartsService
        .chart('pie', litigiosProcedimento, d, this.getOptionsPieChart(null, true, 'top', false)))
      );

    this.api.getLitigiosEmpresa(this.ano)
      .subscribe(x => this.charts.push(this.chartHorizontalBar('litigiosEmpresa', x)));

    const litigiosMes = (<HTMLCanvasElement>$('#litigiosMes')[0]).getContext('2d');
    this.api.getLitigiosMes(this.ano)
      .subscribe(d => {
        this.charts.push(this.chartsService.chart('line', litigiosMes, {
          datasets: [
            {
              label: 'Litígios Mensais',
              data: d.map(y => y.y),
              backgroundColor: '#d8e0f3'
            }
          ],
          labels: d.map(x => Mes[x.label])
        }, {
            legend: {
              display: false
            },
            maintainAspectRatio: false,
            scales: {
              yAxes: [{
                ticks: {
                  beginAtZero: true,
                  stepSize: 1
                }
              }]
            }
          }));
      });

  }

  chartHorizontalBar(id: string, data: any, title: string = null) {
    const ctx = (<HTMLCanvasElement>$(`#${id}`)[0]).getContext('2d');
    return this.chartsService.chart('horizontalBar', ctx, {
      datasets: [{
        data: data.map(x => x.y),
        backgroundColor: data.map((_, i) => this.chartsService.colors[i % this.chartsService.colors.length])
      }],
      labels: data.map(x => x.label)
    }, {
        title: {
          display: title != null,
          text: title
        },
        legend: {
          display: false
        },
        maintainAspectRatio: false,
        responsive: true,
        scales: {
          xAxes: [{
            ticks: {
              beginAtZero: true,
              stepSize: 1
            }
          }]
        }
      });

  }

}
