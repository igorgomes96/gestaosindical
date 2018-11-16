import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, Input, OnDestroy } from '@angular/core';

declare var $: any;

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.css']
})
export class ModalComponent implements OnInit, OnDestroy {

  @Input() title: string;
  @Input() sizeClass: string;

  constructor(private router: Router, private route: ActivatedRoute) { }

  closeModal() {
    $('#modal-component').modal('hide');
    this.router.navigate(['../', { outlets: { popup: null }}]);
  }

  ngOnInit() {
    $('#modal-component').modal('show');
  }

  ngOnDestroy(): void {
    $('#modal-component').modal('hide');
  }
}
