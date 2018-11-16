import { Component, OnInit, Input } from '@angular/core';

declare var $: any;

@Component({
  selector: 'app-panel',
  templateUrl: './panel.component.html',
  styleUrls: ['./panel.component.css']
})
export class PanelComponent implements OnInit {

  @Input() title: string;

  constructor() { }

  ngOnInit() {
    $('.collapse-link').on('click', function () {
      const ibox = $(this).closest('div.ibox');
      const button = $(this).find('i');
      const content = ibox.children('.ibox-content');
      content.slideToggle(200);
      button.toggleClass('fa-chevron-up').toggleClass('fa-chevron-down');
      ibox.toggleClass('').toggleClass('border-bottom');
      setTimeout(function () {
          ibox.resize();
          ibox.find('[id^=map-]').resize();
      }, 50);
  });
  }

}
