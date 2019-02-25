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
    const quotes = document.querySelector(querySelector);
    html2canvas(quotes)
      .then((canvas) => {
        //! MAKE YOUR PDF
        const pdf = new jsPDF('p', 'pt', 'a4');

        for (let i = 0; i <= quotes.clientHeight / 1200; i++) {
          //! This is all just html2canvas stuff
          const srcImg = canvas;
          const sX = 0;
          const sY = 1200 * i; // start 1200 pixels down for every new page
          const sWidth = 1000;
          const sHeight = 1200;
          const dX = 0;
          const dY = 0;
          const dWidth = 1000;
          const dHeight = 1200;

          const onePageCanvas = document.createElement('canvas');
          onePageCanvas.setAttribute('width', sWidth.toString());
          onePageCanvas.setAttribute('height', sHeight.toString());
          const ctx = onePageCanvas.getContext('2d');
          // details on this usage of this function: 
          // https://developer.mozilla.org/en-US/docs/Web/API/Canvas_API/Tutorial/Using_images#Slicing
          ctx.drawImage(srcImg, sX, sY, sWidth, sHeight, dX, dY, dWidth, dHeight);

          // document.body.appendChild(canvas);
          const canvasDataURL = onePageCanvas.toDataURL('image/png', 1.0);

          const width = onePageCanvas.width;
          const height = onePageCanvas.clientHeight;

          //! If we're on anything other than the first page,
          // add another page
          if (i > 0) {
            pdf.addPage(595, 842); //8.5" x 11" in pts (in*72)
          }
          //! now we declare that we're working on that page
          pdf.setPage(i + 1);
          //! now we add content to that page!
          pdf.addImage(canvasDataURL, 'PNG', 20, 40, (width * .62), (height * .62));

        }
        //! after the for loop is finished running, we save the pdf.
        pdf.save(filename);

      });
  }
}
