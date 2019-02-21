import { Mes } from 'src/app/model/sindicato-laboral';
import { Component, OnInit, Input } from '@angular/core';
import { ParcelaReajuste } from 'src/app/model/negociacao';

@Component({
  selector: 'app-popover-template',
  templateUrl: './popover-template.component.html',
  styleUrls: ['./popover-template.component.css']
})
export class PopoverTemplateComponent implements OnInit {

  @Input() parcelasPopover: ParcelaReajuste[];
  Mes: typeof Mes = Mes;

  constructor() { }

  ngOnInit() {
  }

}
