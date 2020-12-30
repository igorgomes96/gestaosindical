import { Injectable } from '@angular/core';
import html2canvas from 'html2canvas';
import { jsPDF } from 'jspdf';
import * as htmlToImage from 'html-to-image';
import { toJpeg } from 'html-to-image';
import { optimizeGroupPlayer } from '@angular/animations/browser/src/render/shared';

declare var $: any;

@Injectable({
  providedIn: 'root'
})
export class PdfGeneratorService {

  constructor() { }


  async htmltoPDF(headerSelector: string, groupSelector: string, filename: string) {
    const header = $(headerSelector)[0];
    const pdf = new jsPDF('p', 'px', 'a4');
    const proportion = 0.43;
    const margins = 20;
    const contentWidth = 408;
    const options = {
      pixelRatio: 1,
      backgroundColor: '#ffffff',
      quality: 1
    };
    let height = 10;

    const generateImage = (dataUrl: any) => {
      const img = new Image();
      img.src = dataUrl;
      return img;
    }

    const dataUrlHeader = await htmlToImage.toJpeg(header, options)
    const headerImage = generateImage(dataUrlHeader);

    headerImage.onload = () => {
      const headerHeight = headerImage.height * proportion;
      height += headerHeight;
      pdf.addImage(dataUrlHeader, 'JPEG', margins, margins, contentWidth + 3, headerHeight);
    }

    const grupos = $(groupSelector);
    const promises = [];
    for (let i = 0; i < grupos.length; i++) {
      const promise = htmlToImage.toJpeg(grupos[i], options)
        .then(dataUrl => {
          const image = generateImage(dataUrl);
          return new Promise((resolve) => {
            image.onload = () => resolve(image);
          })
        });
      promises.push(promise);
    }

    const images = await Promise.all(promises);

    for (let i = 0; i < images.length; i++) {
      const image = images[i];
      const groupHeight = image.height * proportion;
      height += groupHeight;
      if (height > 580) {
        pdf.addPage();
        height = groupHeight;
      }
      pdf.addImage(image.src, 'JPEG', margins, margins + height - groupHeight, contentWidth, groupHeight);
    }
    pdf.save(filename);
  }

}
