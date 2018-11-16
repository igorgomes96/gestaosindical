import { ChartsApiService } from './../charts-api.service';
import { Component, OnInit } from '@angular/core';
import { ChartsService } from '../charts.service';
import { tap, map } from 'rxjs/operators';

declare var Chart: any;
declare var $: any;

@Component({
  selector: 'app-dashboard-negociacoes',
  templateUrl: './dashboard-negociacoes.component.html',
  styleUrls: ['./dashboard-negociacoes.component.css']
})
export class DashboardNegociacoesComponent implements OnInit {

  constructor(private chartsService: ChartsService,
    private api: ChartsApiService) { }

  ngOnInit() {
    const ctx = (<HTMLCanvasElement>$('#plrMassaSalarial')[0]).getContext('2d');
    this.api.getPlrMassaSalarial(2018)
      .pipe(map(d => this.chartsService.mapPieData(d, this.chartsService.mapColorStatus)), tap(console.log))
      .subscribe(d => this.chartsService.pieChart(ctx, d));
  }

}
