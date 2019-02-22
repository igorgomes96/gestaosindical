import { Injectable } from '@angular/core';
import * as jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

@Injectable({
  providedIn: 'root'
})
export class PdfGeneratorService {

  constructor() { }

  htmltoPDF(querySelector: string, filename: string) {
    // parentdiv is the html element which has to be converted to PDF
    html2canvas(document.querySelector(querySelector)).then((canvas: any) => {

      const pdf = new jsPDF('p', 'pt', [canvas.width, canvas.height]);

      const imgData = canvas.toDataURL('image/jpeg', 1.0);
      pdf.addImage(imgData, 0, 0, canvas.width, canvas.height);
      pdf.save(filename);

    });
  }
}
