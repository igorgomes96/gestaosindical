import { StatusNegociacao } from 'src/app/model/negociacao';
import { ChartsApiService } from './../charts-api.service';
import { Component, OnInit } from '@angular/core';
import { ChartsService } from '../charts.service';
import { tap, map } from 'rxjs/operators';
import { devModeEqual } from '@angular/core/src/change_detection/change_detection';
import { Options, ChangeContext } from 'ng5-slider';
import { IntervalFilterService } from 'src/app/shared/interval-filter.service';
import { ActivatedRoute } from '@angular/router';

declare var Chart: any;
declare var $: any;

@Component({
  selector: 'app-dashboard-negociacoes',
  templateUrl: './dashboard-negociacoes.component.html',
  styleUrls: ['./dashboard-negociacoes.component.css']
})
export class DashboardNegociacoesComponent implements OnInit {

  StatusNegociacao: typeof StatusNegociacao = StatusNegociacao;
  ano: number;
  options: Options;
  referent: any;

  constructor(private chartsService: ChartsService, private route: ActivatedRoute,
    private api: ChartsApiService, private intervalService: IntervalFilterService) { }

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

  onUserChangeEnd(changeContext: ChangeContext): void {
    this.referent['ano'] = changeContext.value;
    this.intervalService.value = changeContext.value;
    this.load();
  }

  getOptionsChart(title: string) {
    return {
      title: {
        display: true,
        text: title
      },
      legend: {
        display: false
      },
      tooltips: {
        callbacks: {
          label: function (tooltipItem, data) {
            let label = data.labels[tooltipItem.index] || '';

            if (label) {
              label += ': ';
            }
            const total = (<number[]>data.datasets[tooltipItem.datasetIndex].data)
              .reduce((p, c) => p + c, 0);
            const valor: number = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
            label += `${Math.round((valor / total) * 100)}%`;
            label += ` (${valor})`;
            return label;
          }
        }
      }
    };
  }

  load() {
    const plrMassaSalarial = (<HTMLCanvasElement>$('#plrMassaSalarial')[0]).getContext('2d');
    this.api.getPlrMassaSalarial(this.ano)
      .pipe(
        map(d => d.map(x => Object.assign(x, { label: StatusNegociacao[x['label']] }))),
        map(d => this.chartsService.mapPieData(d, this.chartsService.mapColorStatus)))
      .subscribe(d => this.chartsService
        .pieChart(plrMassaSalarial, d, this.getOptionsChart('% Massa Salarial'))
      );

    const plrTrabalhadores = (<HTMLCanvasElement>$('#plrTrabalhadores')[0]).getContext('2d');
    this.api.getPlrTrabalhadores(this.ano)
      .pipe(
        map(d => d.map(x => Object.assign(x, { label: StatusNegociacao[x['label']] }))),
        map(d => this.chartsService.mapPieData(d, this.chartsService.mapColorStatus)))
      .subscribe(d => this.chartsService
        .pieChart(plrTrabalhadores, d, this.getOptionsChart('% Qtda de Trabalhadores'))
      );

    const actMassaSalarial = (<HTMLCanvasElement>$('#actMassaSalarial')[0]).getContext('2d');
    this.api.getActMassaSalarial(this.ano)
      .pipe(
        map(d => d.map(x => Object.assign(x, { label: StatusNegociacao[x['label']] }))),
        map(d => this.chartsService.mapPieData(d, this.chartsService.mapColorStatus)))
      .subscribe(d => this.chartsService
        .pieChart(actMassaSalarial, d, this.getOptionsChart('% Massa Salarial'))
      );

    const actTrabalhadores = (<HTMLCanvasElement>$('#actTrabalhadores')[0]).getContext('2d');
    this.api.getActTrabalhadores(this.ano)
      .pipe(
        map(d => d.map(x => Object.assign(x, { label: StatusNegociacao[x['label']] }))),
        map(d => this.chartsService.mapPieData(d, this.chartsService.mapColorStatus)))
      .subscribe(d => this.chartsService
        .pieChart(actTrabalhadores, d, this.getOptionsChart('% Qtda de Trabalhadores'))
      );
  }

}
