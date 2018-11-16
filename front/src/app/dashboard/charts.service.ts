import { Injectable } from '@angular/core';

declare var $: any;
declare var Chart: any;

@Injectable({
  providedIn: 'root'
})
export class ChartsService {

  constructor() { }

  mapColorStatus(data: any) {
    const colorStatus = {
      NaoIniciada: '#1c84c6',
      EmNegociacao: '#f8ac59',
      Fechada: '#1ab394',
      Dissidio: '#ed5565'
    };
    return colorStatus[data['label']];
  }

  mapPieData(data: any, mapColor: Function = (x: any) => x['label']) {
    return {
      datasets: [
        {
          data: data.map(x => x.y),
          backgroundColor: data.map(x => mapColor(x))
        }
      ],
      labels: data.map(x => x.label)
     };
  }

  pieChart(ctx: CanvasRenderingContext2D, data: any, options: any = null) {
    return new Chart(ctx, {
      type: 'pie',
      data: data,
      options: options
    });
  }

}
