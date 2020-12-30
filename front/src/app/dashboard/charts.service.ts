import { Reajuste } from './../model/negociacao';
import { Injectable } from '@angular/core';
import { StatusNegociacao } from '../model/negociacao';
import { formatNumber } from '@angular/common';

declare var $: any;
declare var Chart: any;

@Injectable({
  providedIn: 'root'
})
export class ChartsService {

  StatusNegociacao: typeof StatusNegociacao = StatusNegociacao;
  colors = ['#1c84c6', '#f8ac59', '#1ab394', '#ed5565', '#23c6c8', '#590C0C', '#A6A437', '#A62626', '#166EA6', '#103D59', '#3D9BBD', '#DEBCB0', '#045252', '#DA641A', '#001D23'];
  ultimaEmpresa = null;

  constructor() { }

  mapColorStatusNegociacao(data: any) {
    const colorStatus = {
      'Não Iniciada': '#1c84c6',
      'Em Negociação': '#f8ac59',
      'Fechada': '#1ab394',
      'Dissídio': '#ed5565'
    };
    return colorStatus[data['label']];
  }

  mapColorStatusPlanos(data: any) {
    const colorStatus = {
      'No Prazo': '#1c84c6',
      'A vencer': '#f8ac59',
      'Solucionado': '#1ab394',
      'Vencido': '#ed5565'
    };
    return colorStatus[data['label']];
  }

  mapColorProcedimentoLitigio(data: any) {
    const colorStatus = {
      'Mesa Redonda': '#1c84c6',
      'Audiência': '#f8ac59',
      'Reclamação Sindicato': '#1ab394',
      'Fiscalização MPT': '#ed5565',
      'Fiscalização MTE': '#23c6c8'
    };
    return colorStatus[data['label']];
  }

  mapColorReferencia(data: any) {
    const colorStatus = {
      'MTE': '#1c84c6',
      'MPT': '#f8ac59',
      'Sindicato Laboral': '#1ab394',
      'Sindicato Patronal': '#ed5565'
    };
    return colorStatus[data['label']];
  }

  mapColorProcedenciaPlanos(data: any) {
    const colorStatus = {
      'Procedente': '#1ab394',
      'Improcedente': '#ed5565'
    };
    return colorStatus[data['label']];
  }

  labelPercent(tooltipItem, data) {
    let label = data.labels[tooltipItem.index] || '';

    if (label) {
      label += ': ';
    }
    const total = (<number[]>data.datasets[tooltipItem.datasetIndex].data)
      .reduce((p, c) => p + c, 0);
    const valor: number = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
    label += `${Math.round((valor / total) * 100)}%`;
    label += ` (${formatNumber(valor, 'pt-BR', '1.0-2')})`;
    return label;
  }

  labelPercentSymbol(tooltipItem, data) {
    let label = data.labels[tooltipItem.index] || '';
    const valor: number = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
    label += `: ${formatNumber(valor, 'pt-BR', '1.0-2')}%`;
    return label;
  }

  labelRealSymbol(tooltipItem, data) {
    // let label = data.labels[tooltipItem.index] || '';
    const valor: number = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
    const label = `R$ ${formatNumber(valor, 'pt-BR', '1.2')}`;
    return label;
  }

  mapChartData(data: any, mapColor: Function = null) {
    return {
      datasets: [
        {
          data: data.map(x => x.y),
          backgroundColor: mapColor == null ? null : data.map(x => mapColor(x))
        }
      ],
      labels: data.map(x => x.label)
     };
  }

  getDatabasetReajuste(reajuste: Reajuste): any[] {
    return null;
  }

  chart(type: string, ctx: CanvasRenderingContext2D, data: any, options: any = null) {
    return new Chart(ctx, {
      type: type,
      data: data,
      options: options
    });
  }

}
