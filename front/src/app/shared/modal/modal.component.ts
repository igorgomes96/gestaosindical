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
  }

  ngOnInit() {
    $('#modal-component').modal('show');
    $('#modal-component').on('hidden.bs.modal', () => this.router.navigate(['../', { outlets: { popup: null }}]));
  }


  ngOnDestroy(): void {
    $('#modal-component').modal('hide');
  }
}
