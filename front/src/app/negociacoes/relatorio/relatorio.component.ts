import { Component, OnInit } from '@angular/core';
import { filter, switchMap } from 'rxjs/operators';
import { Relatorio } from 'src/app/model/relatorio';
import { ActivatedRoute, Router } from '@angular/router';
import { PdfGeneratorService } from 'src/app/shared/pdf-generator.service';

@Component({
  selector: 'app-relatorio',
  templateUrl: './relatorio.component.html',
  styleUrls: ['./relatorio.component.css']
})
export class RelatorioComponent implements OnInit {

  relatorio: Relatorio;
  
  constructor(private route: ActivatedRoute, private router: Router,
    private pdfGeneratorService: PdfGeneratorService) { }

  ngOnInit() {
    this.route.data
      .pipe(filter(d => d.hasOwnProperty('relatorio')))
      .subscribe(d => {
        this.relatorio = d['relatorio'];
        console.log(this.relatorio);
      });
  }

  download() {
    this.pdfGeneratorService.htmltoPDF('#relatorio-container', 'teste.pdf');
  }

}
