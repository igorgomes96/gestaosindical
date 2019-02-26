import { Injectable } from '@angular/core';
import * as jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { Observable, of } from 'rxjs';

declare var $: any;

@Injectable({
  providedIn: 'root'
})
export class PdfGeneratorService {

  constructor() { }

  async htmltoPDF(headerSelector: string, groupSelector: string, filename: string) {
    const header = $(headerSelector)[0];
    const pdf = new jsPDF('p', 'px', 'a4');
    const scale = 1.8;
    const proportion = 0.43;
    const margins = 20;
    let height = margins;
    const options = {
      scale: scale,
      useCORS: true,
      logging: false,
      windowWidth: 1440,
      windowHeight: 600,
      onclone: function(document) {
        let el = $(document).find(headerSelector);
        el.width('1000px');
        el.css('display', 'block');
        el = $(document).find(groupSelector);
        el.css('display', 'block');
        el.width('1000px');
      }
    };
    await html2canvas(header, options)
      .then((canvas: any) => {
        const headerHeight = (canvas.height / scale) * proportion;
        height += headerHeight;
        pdf.addImage(canvas.toDataURL('image/png'), 'PNG', margins, margins, 450 - (margins * 2), headerHeight);
      });


    const grupos = $(groupSelector);
    for (let i = 0; i < grupos.length; i++) {
      await html2canvas(grupos[i], options)
        .then((canvas: any) => {
          const groupHeight = (canvas.height / scale) * proportion;
          height += groupHeight;
          if (height > 580) {
            pdf.addPage();
            height = groupHeight;
          }
          pdf.addImage(canvas.toDataURL('image/png'), 'PNG', margins, margins + height - groupHeight, 450 - (margins * 2), groupHeight);
        });
    }
    await pdf.save(filename);

  }

}
