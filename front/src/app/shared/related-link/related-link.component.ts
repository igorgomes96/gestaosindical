import { Component, OnInit, Input } from '@angular/core';
import { RelatedLink } from './related-link';

@Component({
  selector: 'app-related-link',
  templateUrl: './related-link.component.html',
  styleUrls: ['./related-link.component.css']
})
export class RelatedLinkComponent implements OnInit {

  @Input() relatedLinks: RelatedLink[];

  constructor() { }

  ngOnInit() {
  }

}
